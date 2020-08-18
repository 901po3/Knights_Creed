/*
 * Class: WeaponScript
 * Date: 2020.8.16
 * Last Modified : 2020.8.17
 * Author: Hyukin Kwon 
 * Description:  무기에 들어가는 스크립트
 *             
*/
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

        public GameObject flashParticle;

        private void Start()
        {
            owner = GetComponentInParent<CharacterControl>();
            originalPos = transform.localPosition;
            originalRot = transform.localRotation;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.transform.tag == "Weapon" && owner.isParrying)
            {
                Debug.Log(collision.transform.tag);
                GameObject obj = Instantiate(flashParticle);
                obj.transform.position = collision.contacts[0].point;
                owner.attacker = collision.gameObject.GetComponentInParent<CharacterControl>();
                owner.attacker.GetAnimator().SetTrigger("KnockDown");
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
                    if (targetScript.team != owner.team && !targetScript.isDodging && !targetScript.isDead && !owner.isParrying)
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
                            targetScript.health -= damage;
                            //Debug.Log(targetScript.gameObject + "'s health: " + targetScript.health);
                        }
                    }
                }
                transform.localPosition = originalPos;
                transform.localRotation = originalRot;
            }              
        }
    }
}

