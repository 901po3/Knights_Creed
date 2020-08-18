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
            if(character.currentState == CURRENT_STATE.HURT || character.currentState == CURRENT_STATE.DEAD || character.currentState == CURRENT_STATE.BLOCKED)
            {
                return;
            }

            statePicked = false;
            if (character.targetEnemy != null)
            {
                CharacterControl targetScript = character.targetEnemy.GetComponent<CharacterControl>();
                disToTarget = Vector3.Distance(character.transform.position, character.targetEnemy.transform.position);

                if (character.runVelocity != Vector3.zero)
                {
                    if(disToTarget <= character.chargeDis)
                    {
                        StopMoving();
                    }
                }
             
                if (character.isDrawingWeapon) //전투중인지 먼저 체크 후 
                {
                    if (disToTarget < character.chargeDis - 1) //너무 가까우면 뒤로 이동
                    {
                        character.runVelocity.z = -1;
                        statePicked = true;
                    }
                    else if (character.runVelocity.z < 0) //뒤로 이동중
                    {
                        if (targetScript.currentState == CURRENT_STATE.ATTACK && disToTarget < targetScript.chargeDis) //뒤로 이동중 공격이 들어오면 무조건 회피
                        {
                            character.currentState = CURRENT_STATE.DODGE;
                            statePicked = true;
                        }
                        else if (disToTarget > character.chargeDis - 0.1f)
                        {
                            character.runVelocity.z = 0;
                            statePicked = false;
                            statePickFrequency = 2f;
                        }
                    }

                    statePickFrequency = 1.5f;
                    if(!statePicked)
                    {
                        MoveToTarget();
                    }
                    if (disToTarget <= character.chargeDis) //공격 범위 안에 적이 있으면 공격 || 피하기
                    {
                        bool dodgeResult = false;
                        if (character.currentState == CURRENT_STATE.NONE)
                        {
                            if(!statePicked)
                            {
                                dodgeResult = DodgeInput();
                            }                         
                        }
                        if (!dodgeResult) //피하는 중이 아니면 확률적으로 공격 실행
                        {
                            AttackInput();
                        }
                    }
                    if(statePicked) //true면 statePickFrequency 시간후 다시 state를 정한다.
                    {
                        if(!stateFrequencyOnce)
                        {
                            stateFrequencyOnce = true;
                            StartCoroutine(ResetPickedState());
                        }

                        switch (character.currentState)
                        {
                            case CURRENT_STATE.ATTACK:
                                character.runVelocity = Vector3.zero;
                                character.GetAnimator().SetBool("Attack", true);
                                break;
                            case CURRENT_STATE.DODGE:
                                character.runVelocity = Vector3.zero;
                                character.GetAnimator().SetBool("Dodge", true);
                                break;
                        }
                    }
                    else
                    {
                        character.currentState = CURRENT_STATE.NONE;
                    }
                }
                else
                {
                    character.currentState = CURRENT_STATE.NONE;
                    StopMoving();
                }
            }
            else
            {
                character.currentState = CURRENT_STATE.NONE;
                StopMoving();
            }
        }

        private void MoveToTarget() //에비 함수
        {
            //다른 행동을 수행중이 아니면 확률적으로 이동 실행
            if (character.currentState == CURRENT_STATE.NONE) 
            {         
                if (disToTarget > character.chargeDis) //이동하기에 거리가 충분한지 체크
                {
                    if(Random.Range(0, 2) == 0) // 확률로 이동 실행
                    {
                        character.runVelocity.z = 1;
                        statePickFrequency = Random.Range(3f, 7f); //이동을 시작하면 3~7초간 이동한다.
                        statePicked = true;
                    }
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

        private bool DodgeInput()
        {
            CharacterControl targetScript = character.targetEnemy.GetComponent<CharacterControl>();

            if (disToTarget <= targetScript.chargeDis)
            {
                if (targetScript.currentState == CURRENT_STATE.ATTACK && disToTarget < targetScript.chargeDis)
                {
                    if (Random.Range(0, 2) == 0) // 1/2 확률도 피하기
                    {
                        float delay = targetScript.attackEnableTime - targetScript.attackTimer + 0.1f;
                        if(delay < 0)
                        {
                            delay = 0;
                        }
                        StartCoroutine(PlayDodgeDelay(delay));
                        statePickFrequency = targetScript.curAimTime + 0.5f;
                        statePicked = true;
                        return true;
                    }
                }               
            }
            return false;
        }

        private void AttackInput()
        {
            CharacterControl targetScript = character.targetEnemy.GetComponent<CharacterControl>();
            if (disToTarget < character.chargeDis)
            {
                if (Random.Range(0, 10) <= 7) 
                {
                    if (character.runVelocity.z < 0)
                    {
                        character.runVelocity = Vector3.zero;
                    }

                    character.curUndetectedTimer = 0; //공격 -> 전투 해제 타이머 리셋
                    character.currentState = CURRENT_STATE.ATTACK;
                    statePickFrequency = 2.5f;
                    statePicked = true;
                }
            }
        }
        
        IEnumerator PlayDodgeDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if(character.targetEnemy != null)
            {
                if (character.targetEnemy.GetComponent<CharacterControl>().currentState == CURRENT_STATE.ATTACK) //아직까지 공격중인지 체크
                {
                    character.currentState = CURRENT_STATE.DODGE;
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
