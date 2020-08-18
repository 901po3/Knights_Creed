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
        private float statePickFrequency;
        private bool stateFrequencyOnce = false;
        private float disToTarget;
        private float dampSpeed = 4;

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
                disToTarget = Vector3.Distance(character.transform.position, character.targetEnemy.transform.position);

                if (character.runVelocity != Vector3.zero)
                {
                    character.isDodging = false;
                    if(disToTarget <= character.chargeDis)
                    {
                        StopMoving();
                    }
                }
             
                if (character.isBattleModeOn && character.isDrawingWeapon) //전투중인지 먼저 체크 후 
                {
                    if (disToTarget < character.chargeDis - 1) //너무 가까우면 뒤로 이동
                    {
                        character.runVelocity.z = -1;
                    }
                    if (character.runVelocity.z < 0) //뒤로 이동중
                    {
                        if (character.targetEnemy.GetComponent<CharacterControl>().isAttacking) //뒤로 이동중 공격이 들어오면 무조건 회피
                        {
                            character.isDodging = true;
                            if (character.runVelocity.z <= 0)
                            {
                                character.runVelocity = Vector3.zero;
                                character.isDodging = true;
                                character.GetAnimator().SetBool("Dodge", true);
                            }
                        }
                        else if (disToTarget > character.chargeDis)
                        {
                            character.runVelocity.z = 0;
                        }
                    }

                    if (!statePicked) //false면 다음 state를 정한다
                    {
                        MoveToTarget();
                        if (disToTarget <= character.chargeDis) //공격 범위 안에 적이 있으면 공격 || 피하기
                        {
                            if (!character.isAttacking && character.runVelocity == Vector3.zero) //공격중이 아니고 속도가 0이면 확률적으로 피하기 실행
                            {
                                DodgeInput(0.2f); //딜레이 재생                              
                            }
                            if (!character.isDodging) //피하는 중이 아니면 확률적으로 공격 실행
                            {
                                AttackInput();
                            }
                        }
                        else if(disToTarget > character.chargeDis) //공격 범위 안에 적이 없으면 공격 취소
                        {
                            character.isAttacking = false;
                            if(disToTarget > character.targetEnemy.GetComponent<CharacterControl>().chargeDis + 0.25f) //'적'의 공격 범위안에 없으면 피하기 취소
                            {
                                character.isDodging = false;
                                character.GetAnimator().SetBool("Dodge", false);
                            }
                        }
                    }
                    else //true면 statePickFrequency 시간후 다시 state를 정한다.
                    {
                        if(!stateFrequencyOnce)
                        {
                            stateFrequencyOnce = true;
                            StartCoroutine(ResetPickedState());
                        }
                    }
                }
                else
                {
                    StopMoving();
                    statePicked = false;
                }
            }
            else
            {
                character.runVelocity.z = 0;
                statePicked = false;
                character.isAttacking = false;
                character.isDodging = false;
                character.GetAnimator().SetBool("Dodge", false);
            }
        }

        private void MoveToTarget() //에비 함수
        {
            if(character.targetEnemy == null) //타겟이 없어지면 움직임을 멈춘다
            {
                StopMoving();
                statePicked = false;
                return;
            }

            //다른 행동을 수행중이 아니면 확률적으로 이동 실행
            if (!character.isAttacking && !character.isDodging) 
            {         
                if (disToTarget > character.chargeDis) //이동하기에 거리가 충분한지 체크
                {
                    if(Random.Range(0, 2) == 0) // 1/5 확률도 이동 실행
                    {
                        character.runVelocity.z = 1;
                        statePickFrequency = Random.Range(3f, 7f); //이동을 시작하면 3~7초간 이동한다.
                    }
                    else
                    {
                        statePickFrequency = 1f; //이동 실행을 안하면 일정시간 후 다시 선택
                    }
                    statePicked = true;
                }
            }
        }


        //멈출때까지 속도 줄이기
        private void StopMoving()
        {
            if (character.runVelocity.z > 0)
            {
                character.runVelocity.z -= dampSpeed * Time.deltaTime;
                if (character.runVelocity.z < 0)
                    character.runVelocity.z = 0;
            }
        }

        private bool DodgeInput(float delay)
        {
            CharacterControl targetScript = character.targetEnemy.GetComponent<CharacterControl>();
            if (!character.targetEnemy.GetComponent<CharacterControl>().isAttacking)
            {
                character.isDodging = false;
                character.GetAnimator().SetBool("Dodge", false);
            }

            if (Vector3.Distance(transform.position, character.targetEnemy.transform.position) <= targetScript.chargeDis - 0.25f)
            {
                if (targetScript.isAttacking)
                {
                    statePicked = true;

                    if (Random.Range(0, 2) == 0) // 1/2 확률도 피하기
                    {
                        StartCoroutine(PlayDodgeDelay(delay));
                        statePickFrequency = 2.5f;
                        return true;
                    }
                    else
                    {
                        statePickFrequency = 1.5f;
                        return false;
                    }
                }               
            }
            return false;
        }

        private void AttackInput()
        {
            CharacterControl targetScript = character.targetEnemy.GetComponent<CharacterControl>();
            if (Vector3.Distance(transform.position, character.targetEnemy.transform.position) < character.chargeDis)
            {
                                            //적도 공격중이라면 일정 확률로 피하기 
                if (Random.Range(0, 10) <= 7 && !DodgeInput(0.15f)) // 1/4 확률도 공격
                {
                    if (character.runVelocity.z < 0)
                    {
                        character.runVelocity = Vector3.zero;
                    }

                    character.curUndetectedTimer = 0; //공격 -> 전투 해제 타이머 리셋
                    character.isAttacking = true;
                    statePickFrequency = 2.5f;
                    character.isDodging = false;
                    character.GetAnimator().SetBool("Dodge", false);
                    statePicked = true;
                }
            }
        }
        
        IEnumerator PlayDodgeDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if(character.targetEnemy != null)
            {
                if (character.targetEnemy.GetComponent<CharacterControl>().isAttacking) //아직까지 공격중인지 체크
                {
                    yield return new WaitForSeconds(delay);
                    character.isAttacking = false;
                    character.isDodging = true;
                    if (character.runVelocity.z <= 0)
                    {
                        character.runVelocity = Vector3.zero;
                        character.GetAnimator().SetBool("Dodge", true);
                    }
                }
            }
        }

        IEnumerator ResetPickedState()
        {
            yield return new WaitForSeconds(statePickFrequency);
            statePicked = false;
            stateFrequencyOnce = false;
        }
    }

}
