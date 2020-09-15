﻿/*
 * Class: WeaponScript
 * Date: 2020.8.16
 * Last Modified : 2020.8.22
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
                owner.attacker.attacker = owner;
                owner.canComboAttacking = true;
                if (owner.attacker.targetEnemy == null)
                    owner.attacker.targetEnemy = owner.gameObject;
                owner.attacker.currentState = CURRENT_STATE.BLOCKED;
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
                        ApplyDamage(targetScript);

                        //Hurt Anim
                        if(targetScript.health > 0)
                        {
                            MoveToHurt(targetScript);
                        }
                        else //Death
                        {
                            if (targetScript.attacker == null)
                                targetScript.attacker = owner;
                            GoToDeath(targetScript);
                        }
                    }
                }
            }              
        }

        private void ApplyDamage(CharacterControl targetScript)
        {
            if(targetScript.health > 0)
            {
                if (!damageOnce)
                {
                    damageOnce = true;
                    //때린 대상을 무기 타겟으로 변경
                    if (owner.tag == "Player")
                    {
                        if (owner.targetEnemy != targetScript.gameObject)
                        {
                            owner.targetEnemy = targetScript.gameObject;
                        }
                    }

                    //데미지 적용
                    if (owner.medAttackType == MED_ATTACK_TYPE.COMBO)
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

        private void MoveToHurt(CharacterControl targetScript)
        {
            if (!targetScript.hurtAnimOnce)
            {
                targetScript.hurtAnimOnce = true;

                //일정 시간마다 Hurt 애니메이션 재생
                StartCoroutine(HurtPlayFrequency(targetScript));

                //공격중에 피해를 받으면 미리 다음 공격을 정해둔다
                if (targetScript.medAttackType == MED_ATTACK_TYPE.COMBO)
                {
                    targetScript.PickFirstNextAttack();
                }
                else if (targetScript.currentState == CURRENT_STATE.ATTACK)
                {
                    targetScript.PickNextAttack(false);
                }

                targetScript.currentState = CURRENT_STATE.HURT;
                targetScript.hurtTimer = 0f;
                targetScript.GetAnimator().SetBool("Hurt", true);
                targetScript.GetAnimator().SetBool("Dead", false);
                targetScript.GetAnimator().SetTrigger("HurtEnterOnce");
            }
        }

        IEnumerator HurtPlayFrequency(CharacterControl targetScript)
        {
            yield return new WaitForSeconds(2f);
            targetScript.hurtAnimOnce = false;
        }

        private void GoToDeath(CharacterControl targetScript)
        {
            if (targetScript.medAttackType == MED_ATTACK_TYPE.COMBO)
            {
                targetScript.PickFirstNextAttack();
            }
            else if (targetScript.currentState == CURRENT_STATE.ATTACK)
            {
                targetScript.PickNextAttack(false);
            }
            targetScript.deathTimer = 0f;
            targetScript.currentState = CURRENT_STATE.DEAD;
            targetScript.GetAnimator().SetBool("Dead", true);
            targetScript.GetAnimator().SetTrigger("DeadEnterOnce");
        }
    }
}

