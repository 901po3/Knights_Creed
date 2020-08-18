/*
 * Class: WeaponScript
 * Date: 2020.8.16
 * Last Modified : 2020.8.17
 * Author: Hyukin Kwon 
 * Description:  무기에 들어가는 스크립트
 *             
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyukinKwon
{
    public class WeaponScript : MonoBehaviour
    {
        CharacterControl owner; //무기의 소유자

        //무기의 공격력
        public int damage;
        public bool damageOnce = false;

        //칼 위치가 벗어나면 고쳐주는 용도
        private Vector3 originalPos;
        private Quaternion originalRot;

        //검 마찰 이펙트
        public GameObject flashParticle;
        private List<GameObject> flashEffectList;
        private int maxFlashNum = 5;
        private int curFlashNum = 0;

        public bool parryOnce = false;

        private void Start()
        {
            owner = GetComponentInParent<CharacterControl>();
            if(owner.weapon == Equipment.WEAPON.ONE_HANDED_SWORD)
            {
                originalPos = new Vector3(8.1f, 5.5f, -1.9f);
                originalRot = Quaternion.Euler(69.806f, -1.447f, 196.297f);
            }

            //파티클 초기화
            flashEffectList = new List<GameObject>();
            for (int i = 0; i < maxFlashNum; i++) //피 이펙트 3개
            {
                GameObject flash = Instantiate(flashParticle);
                flash.transform.parent = transform;
                flashEffectList.Add(flash);
                flash.SetActive(false);
            }
        }

        public void FixTransform()
        {
            transform.localPosition = originalPos;
            transform.localRotation = originalRot;
        }

        public void ToggleCollision(bool b)
        {
            GetComponent<BoxCollider>().isTrigger = !b;
            GetComponent<BoxCollider>().enabled = b;
        }

        IEnumerator TurnOffFlashEffect(GameObject flash)
        {
            yield return new WaitForSeconds(0.6f);
            flash.SetActive(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            //막기 성공 했는지 판별
            if(collision.transform.tag == "Weapon" && owner.currentState == CURRENT_STATE.PARRY && !parryOnce)
            {
                parryOnce = true;
                Debug.Log(collision.transform.tag);
                owner.attacker = collision.gameObject.GetComponentInParent<CharacterControl>();
                owner.attacker.currentState = CURRENT_STATE.BLOCKED;
                owner.attacker.attacker = owner;
                owner.canComboAttacking = true;
                owner.attacker.GetAnimator().SetBool("Blocked", true);

                //막기 이팩트 재생
                GameObject flash = flashEffectList[curFlashNum];
                if (!flash.activeSelf)
                {
                    curFlashNum = (curFlashNum + 1) % maxFlashNum;
                    flash.transform.position = collision.contacts[0].point;
                    flash.SetActive(true);
                    StartCoroutine(TurnOffFlashEffect(flash)); //일정 시간후 비활성
                }
                return;
            }

            if(collision.gameObject.GetComponent<CharacterControl>() != null)
            {
                CharacterControl targetScript = collision.gameObject.GetComponent<CharacterControl>();
                

                //충돌시 발생하는 힘을 제거한다;
                owner.GetRigidbody().velocity = Vector3.zero;
                collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

                if(targetScript.tag == "Player" || targetScript.tag == "AI")
                {
                    //적이고, 적이 피하는중이 아니고, 내가 막는중이 아니면 공격 적용
                    if (targetScript.team != owner.team && !targetScript.invincible)
                    {
                        //충돌 직후 콜라이더 비활성
                        //한명만 떄린다
                        GetComponent<Collider>().enabled = false;

                        //데미지 적용
                        if (!damageOnce)
                        {
                            damageOnce = true;
                            //때린 대상의을 무기 타겟으로 변경
                            if (owner.targetEnemy != collision.gameObject)
                            {
                                owner.targetEnemy = collision.gameObject;
                            }

                            //공격한 적이 이미 전에 떄린적이 아니면 AttackList에 추가한다
                            //옹도: 무기 소유자가 죽었을때 소유자를 타겟으로 갖고있는 대상의 타겟을 초기화
                            bool isAlreadyAdded = false;
                            foreach (CharacterControl c in owner.attackList)
                            {
                                if (c.gameObject == targetScript.gameObject)
                                {
                                    isAlreadyAdded = true;
                                    break;
                                }

                            }
                            if (!isAlreadyAdded)
                            {
                                owner.attackList.Add(targetScript);
                            }

                            //데미지 적용
                            if(owner.medAttackType == MED_ATTACK_TYPE.COMBO)
                            {
                                targetScript.health -= (damage * 2);
                                Debug.Log("Combo!");
                            }
                            else
                            {
                                targetScript.health -= damage;
                            }
                            Debug.Log(targetScript.gameObject + "'s health: " + targetScript.health);
                        }
                    }
                }
            }              
        }
    }
}

