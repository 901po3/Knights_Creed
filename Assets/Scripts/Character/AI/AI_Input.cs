/*
 * Class: AI_Input
 * Date: 2020.8.13
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: AI 인풋을 받는 클래스
 *              AI 인풋으로 CharacterControl 상태 변수를 제어한다.
*/
using System.Collections;
using UnityEngine;

namespace HyukinKwon
{
    public class AI_Input : MonoBehaviour
    {
        private CharacterControl character; //캐릭터 Gameobject의 CharacterControl 스크립트
        private bool statePicked = false; //행동을 한번만 선택한다.

        private void Awake()
        {
            character = GetComponent<CharacterControl>();
            //자기 자신으로 기준
            character.facingStandardTransfom = character.transform;
        }

        private void Update()
        {
            if (character.targetEnemy != null)
            {
                if (character.isBattleModeOn && character.isDrawingWeapon) //전투중인지 먼저 체크 후 
                {
                    if(!statePicked)
                    {
                        if(character.runVelocity == Vector3.zero)
                        {
                            if (!character.isAttacking)
                            {
                                DodgeInput();
                            }
                            if (!character.isDodging)
                            {
                                AttackInput();
                            }
                        }
                        MoveToTarget();
                    }
                    else
                    {
                        StartCoroutine(ResetPickedState(1.5f));
                    }
                }
            }
        }

        private void MoveToTarget() //에비 함수
        {
            if (!character.isAttacking && !character.isDodging)
            {
                float dis = Vector3.Distance(character.transform.position, character.targetEnemy.transform.position);
                if (dis >= character.chargeDis)
                {
                    Vector3 dir = (character.targetEnemy.transform.position - character.transform.position).normalized;
                    dir.y = 0;

                    if (dir.z > 0)
                    {
                        character.runVelocity.z += Time.deltaTime;
                    }
                    else if (dir.z < 0)
                    {
                        character.runVelocity.z -= Time.deltaTime;
                    }
                    else
                    {
                        if (character.runVelocity.z > 0)
                        {
                            character.runVelocity.z -= Time.deltaTime;
                            if (character.runVelocity.z < 0)
                                character.runVelocity.z = 0;
                        }
                        else if (character.runVelocity.z < 0)
                        {
                            character.runVelocity.z += Time.deltaTime;
                            if (character.runVelocity.z > 0)
                                character.runVelocity.z = 0;
                        }
                    }
                    character.runVelocity.z = Mathf.Clamp(character.runVelocity.z, 0, 1);
                }           
            }

        }

        private void DodgeInput()
        {
            CharacterControl targetScript = character.targetEnemy.GetComponent<CharacterControl>();
            if (!character.targetEnemy.GetComponent<CharacterControl>().isAttacking && character.isDodging)
            {
                character.isDodging = false;
                character.GetAnimator().SetBool("Dodge", false);
            }

            if (Vector3.Distance(transform.position, character.targetEnemy.transform.position) < targetScript.chargeDis - 0.3f)
            {
                if (targetScript.isAttacking)
                {
                    statePicked = true;

                    if (Random.Range(0, 3) == 0) // 1/3 확률도 피하기
                    {
                        StartCoroutine(PlayDodgeDelay());
                    }
                }               
            }
        }

        private void AttackInput()
        {
            CharacterControl targetScript = character.targetEnemy.GetComponent<CharacterControl>();
            if (Vector3.Distance(transform.position, character.targetEnemy.transform.position) < character.chargeDis)
            {
                if (Random.Range(0, 5) == 0) // 1/5 확률도 공격
                {
                    character.curUndetectedTimer = 0; //공격 -> 전투 해제 타이머 리셋
                    character.isAttacking = true;
                    character.isDodging = false;
                    character.GetAnimator().SetBool("Dodge", false);
                }
            }
        }
        
        IEnumerator PlayDodgeDelay()
        {
            yield return new WaitForSeconds(0.2f);
            if(character.targetEnemy.GetComponent<CharacterControl>().isAttacking) //아직까지 공격중인지 체크
            {
                yield return new WaitForSeconds(0.3f);
                character.isAttacking = false;
                character.isDodging = true;
                character.GetAnimator().SetBool("Dodge", true);
            }
        }

        IEnumerator ResetPickedState(float frequency)
        {
            yield return new WaitForSeconds(frequency);
            statePicked = false;
        }
    }

}
