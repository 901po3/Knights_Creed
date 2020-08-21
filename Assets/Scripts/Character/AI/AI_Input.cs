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
        public enum DISTANCE_STATE
        {
            FAR, CLOSE, GOOD
        }
        public enum DECICION
        {
            //거리가 멀때 선택 가능
            MOVE_TO_TARGET, IDLE_01,
            //거리가 가까울때 선택 가능
            MOVE_BACK_DODGE, MOVE_BACK,
            //거리가 적당할때 적이 공격중이면
            DODGE_01, DODGE_02, PARRY_01, IDLE_02,
            //거리가 적당할때 적이 공격중이 아니면
            ATTACK_01, IDLE_03
        }

        private CharacterControl character; // 캐릭터 Gameobject의 CharacterControl 스크립트
        private CharacterControl targetScript;
        private float disToTarget;
        [SerializeField] private DISTANCE_STATE distanceState;
        [SerializeField] private DECICION decicion;
        private Vector3 movingDirection = Vector3.zero;
        private float dampSpeed = 2f;
        [SerializeField] private float nextDecicionTime = 0f; // 다음 행동 정하기까지의 시간
        private bool decided = false; // nextDecicionTime 중복 설정 방지 
        private bool once = false;
        public GameObject nearestEnemy; //타겟으로 삼을 적

        private void Awake()
        {
            character = GetComponent<CharacterControl>();
            //자기 자신으로 기준
            character.facingStandardTransfom = character.transform;
        }

        private void Update()
        {
            SetTarget();

            if (!character.isDrawingWeapon) return;
            if(decided)
            {
                // 1. 이동 방향 체크
                if (character.targetEnemy != null) // 계속 이동
                {
                    if(movingDirection == Vector3.right || movingDirection == Vector3.left)
                    {
                        StartMoving(movingDirection);
                        character.runVelocity.z = -character.targetEnemy.GetComponent<CharacterControl>().runVelocity.z;
                    }
                    else if(movingDirection == Vector3.forward || movingDirection == Vector3.back)
                    {
                        StartMoving(movingDirection);
                    }

                    //멈추기
                    if(decicion == DECICION.IDLE_01)
                    {
                        StopMoving();
                    }
                    else
                    {
                        //거리 체크 후 멈춤
                        CheckDistance();
                        if ((distanceState == DISTANCE_STATE.GOOD && character.runVelocity != Vector3.zero) ||
                            (distanceState == DISTANCE_STATE.CLOSE && character.runVelocity.z > 0) ||
                            (distanceState == DISTANCE_STATE.FAR && character.currentState != CURRENT_STATE.MOVE_DODGE && character.runVelocity.z < 0))
                        {
                            movingDirection = Vector3.zero;
                            decicion = DECICION.IDLE_01;
                            nextDecicionTime = 1f;
                        }

                        //AI 행동 예외처리 
                        if (targetScript.tag != "Player" && character.runVelocity.z != 0)
                        {
                            if ((movingDirection == Vector3.forward && targetScript.GetComponent<AI_Input>().movingDirection == Vector3.back) ||
                                (movingDirection == Vector3.back && targetScript.GetComponent<AI_Input>().movingDirection == Vector3.forward))
                            {
                                movingDirection = Vector3.zero;
                                decicion = DECICION.IDLE_01;
                                nextDecicionTime = 1f;
                            }
                        }
                    }


                    //공격이 들어오면 추가 판단을 한다
                    if((decicion == DECICION.IDLE_01 || decicion == DECICION.MOVE_TO_TARGET || decicion == DECICION.MOVE_BACK) && !once)
                    {
                        if (disToTarget <= targetScript.chargeDis)
                        {
                            // DODGE, PARRY, IDLE_02 중에서 선택
                            if (targetScript.currentState == CURRENT_STATE.ATTACK && targetScript.attackTimer < targetScript.attackEndTime - 0.1f)
                            {
                                once = true;
                                movingDirection = Vector3.zero;
                                StopMoving();

                                decicion = (DECICION)Random.Range((int)DECICION.DODGE_01, (int)DECICION.IDLE_02 + 1);
                                UpdateStateByDecicion();
                                character.ApplyCurrentState();
                            }
                        }
                    }

                    // 2. 다른 행동
                    if(nextDecicionTime > 0 && character.runVelocity == Vector3.zero)
                    {
                        nextDecicionTime -= Time.deltaTime;
                        if(nextDecicionTime <= 0)
                        {
                            movingDirection = Vector3.zero;
                            decicion = DECICION.IDLE_01;
                            StopMoving();
                            if(character.runVelocity == Vector3.zero)
                            {
                                decided = false;
                                once = false;
                            }
                        }
                    }
                }
                else // 멈춤
                {
                    StopMoving();
                    nextDecicionTime = 1f;
                }
            }
            else
            {
                // 1. AI 자신의 상태 체크
                if (character.currentState != CURRENT_STATE.NONE || character.runVelocity != Vector3.zero)
                {
                    // 외부에서 정해지는 State 상태
                    if (!decided)
                    {
                        if (character.currentState == CURRENT_STATE.HURT || character.currentState == CURRENT_STATE.BLOCKED)
                        {
                            nextDecicionTime = character.curAimTime + 0.5f;
                            decided = true;
                        }
                    }
                    return;
                }

                // 2. 타겟이 있는지 체크
                if (character.targetEnemy == null) return;

                // 3. 거리 체크
                CheckDistance();

                // 4. 확률로 다음 상태 정하기
                PickDecicion();

                // 5. 선택된 행동에 맞는 값 넣기
                UpdateStateByDecicion();
                decided = true;

                // 6. 결정 적용
                character.ApplyCurrentState();
            }          
        }

        private void SetTarget()
        {
            if (nearestEnemy == null)
            {
                character.targetEnemy = null;
                character.attacker = null;
                character.isBattleModeOn = false;
                return;
            }

            if (character.attacker == null) //캐릭터를 때린 적이 없으면
            {
                if (character.targetEnemy == null) //현재 타겟이 없다면
                {
                    if (nearestEnemy != null) //가장 가까운 적을 찾는다
                    {
                        character.targetEnemy = nearestEnemy;
                        //NearestEnemy의 TargetOnMe에 자신을 추가한다
                        bool check = false;
                        for(int i = 0; i < nearestEnemy.GetComponent<CharacterControl>().targetOnMe.Count; i++)
                        {
                            if(character == nearestEnemy.GetComponent<CharacterControl>().targetOnMe[i])
                            {
                                check = true;
                                break;
                            }
                        }
                        if(!check)
                        {
                            nearestEnemy.GetComponent<CharacterControl>().targetOnMe.Add(character);
                        }
                    }
                }
                else //이미 타겟이 있다면
                {
                    if (character.targetEnemy != nearestEnemy) //현재 타겟과 가장 가까운 적이 다르다면
                    {
                        //현재 Target의 TargetOnMe에 자신을 뺀다
                        character.targetEnemy.GetComponent<CharacterControl>().targetOnMe.Remove(character);
                        bool check = false;
                        for (int i = 0; i < nearestEnemy.GetComponent<CharacterControl>().targetOnMe.Count; i++)
                        {
                            if (character == nearestEnemy.GetComponent<CharacterControl>().targetOnMe[i])
                            {
                                check = true;
                                break;
                            }
                        }
                        if (!check)
                        {
                            nearestEnemy.GetComponent<CharacterControl>().targetOnMe.Add(character);
                        }
                        character.targetEnemy = nearestEnemy; //가장 가까운 적으로 타겟 변경
                    }
                }
            }
            else  //캐릭터를 때린 적이 있으면
            {
                //타겟이 없거나 때린자라 다르면 타겟 변경
                if (character.targetEnemy == null || character.targetEnemy != character.attacker) 
                {
                    bool check = false;
                    for (int i = 0; i < character.attacker.GetComponent<CharacterControl>().targetOnMe.Count; i++)
                    {
                        if (character == character.attacker.GetComponent<CharacterControl>().targetOnMe[i])
                        {
                            check = true;
                            break;
                        }
                    }
                    if (!check)
                    {
                        character.attacker.GetComponent<CharacterControl>().targetOnMe.Add(character);
                    }

                    if(character.targetEnemy != character.attacker)
                    {
                        character.targetEnemy.GetComponent<CharacterControl>().targetOnMe.Remove(character);
                    }

                    character.targetEnemy = character.attacker.gameObject;
                }
            }

            if(character.targetEnemy != null)
            {
                character.isBattleModeOn = true;
                targetScript = character.targetEnemy.GetComponent<CharacterControl>();
            }
        }

        private void CheckDistance()
        {
            disToTarget = Vector3.Distance(character.transform.position, targetScript.transform.position);

            if (disToTarget > character.chargeDis) // 3-1. 거리가 멀다
            {
                distanceState = DISTANCE_STATE.FAR;
            }
            else if (disToTarget < character.chargeDis - 1) // 3-2. 거리가 짧다
            {
                distanceState = DISTANCE_STATE.CLOSE;
            }
            else // 3-3. 적당한 거리
            {
                distanceState = DISTANCE_STATE.GOOD;
            }
        }

        private void PickDecicion()
        {
            switch (distanceState)
            {
                case DISTANCE_STATE.FAR:
                    // 4-1. MOVE_TO_TARGET, IDLE_01 중에서 선택
                    decicion = (DECICION)Random.Range((int)DECICION.MOVE_TO_TARGET, (int)DECICION.IDLE_01 + 1); 
                    break;
                case DISTANCE_STATE.CLOSE:
                    // 4-2. MOVE_BACK_DODGE, MOVE_BACK 중에서 선택
                    decicion = (DECICION)Random.Range((int)DECICION.MOVE_BACK_DODGE, (int)DECICION.MOVE_BACK + 1); 
                    break;
                case DISTANCE_STATE.GOOD:
                    // 4- 3 ATTACK_01, IDLE_03 중에서 선택
                    if (targetScript.currentState != CURRENT_STATE.ATTACK) 
                    {
                        decicion = (DECICION)Random.Range((int)DECICION.ATTACK_01, (int)DECICION.IDLE_03 + 1);
                    }
                    break;
            }
            if(disToTarget <= targetScript.chargeDis)
            {
                // DODGE, PARRY, IDLE_02 중에서 선택
                if (targetScript.currentState == CURRENT_STATE.ATTACK)
                {
                    decicion = (DECICION)Random.Range((int)DECICION.DODGE_01, (int)DECICION.IDLE_02 + 1);
                }
            }
        }

        private void UpdateStateByDecicion()
        {
            switch (decicion)
            {
                case DECICION.IDLE_01:
                case DECICION.IDLE_02:
                case DECICION.IDLE_03:
                    //2~4초간 대기
                    decicion = DECICION.IDLE_01;
                    nextDecicionTime = Random.Range(1f, 3f);
                    break;
                case DECICION.MOVE_TO_TARGET:
                    //최대 3~7초간 앞으로 이동
                    movingDirection = Vector3.forward;
                    nextDecicionTime = Random.Range(3f, 7f);
                    break;
                case DECICION.MOVE_BACK:
                    //거리가 적당해질떄까지 뒤로 이동
                    movingDirection = Vector3.back;
                    nextDecicionTime = 99999f;
                    break;
                case DECICION.MOVE_BACK_DODGE:
                    //방향 선택
                    character.runVelocity.z = Random.Range(-0.75f, -1f);
                    int ran = Random.Range(0, 3);
                    if(ran == 0)
                    {
                        character.runVelocity.x = Random.Range(-0.75f, -1f);
                    }
                    else if(ran == 1)
                    {
                        character.runVelocity.x = Random.Range(0.75f, 1f);
                    }

                    character.currentState = CURRENT_STATE.MOVE_DODGE;
                    nextDecicionTime = 2f;
                    break;
                case DECICION.DODGE_01:
                case DECICION.DODGE_02:
                    decicion = DECICION.DODGE_01;
                    character.currentState = CURRENT_STATE.DODGE;
                    nextDecicionTime = 2f;
                    break;
                case DECICION.PARRY_01:
                    character.currentState = CURRENT_STATE.PARRY;
                    nextDecicionTime = 1.5f;
                    break;
                case DECICION.ATTACK_01:
                    character.currentState = CURRENT_STATE.ATTACK;
                    nextDecicionTime = 2.2f;
                    break;
            }
        }

        private void StartMoving(Vector3 dir)
        {
            if(dir == Vector3.forward)
            {
                if (character.runVelocity.z < 1) // 앞으로 이동
                {
                    character.runVelocity.z += dampSpeed * Time.deltaTime;
                    if (character.runVelocity.z > 1)
                    {
                        character.runVelocity.z = 1f;
                    }
                }
            }
            else if(dir == Vector3.back)
            {
                if (character.runVelocity.z > -1) // 좌측으로 회전 이동
                {
                    character.runVelocity.z -= dampSpeed * Time.deltaTime;
                    if (character.runVelocity.z < -1)
                    {
                        character.runVelocity.z = -1f;
                    }
                }
            }
            else if(dir == Vector3.right)
            {
                if (character.runVelocity.x < 1) // 앞으로 이동
                {
                    character.runVelocity.x += dampSpeed * Time.deltaTime;
                    if (character.runVelocity.x > 1)
                    {
                        character.runVelocity.x = 1f;
                    }
                }
            }
            else if(dir == Vector3.left)
            {
                if (character.runVelocity.x > -1) // 좌측으로 회전 이동
                {
                    character.runVelocity.x -= dampSpeed * Time.deltaTime;
                    if (character.runVelocity.x < -1)
                    {
                        character.runVelocity.x = -1f;
                    }
                }
            }
        }

        private void StopMoving()
        {
            if(character.runVelocity.z > 0)
            {
                character.runVelocity.z -= dampSpeed * Time.deltaTime;
                if(character.runVelocity.z < 0)
                {
                    character.runVelocity.z = 0;
                }
            }
            else if(character.runVelocity.z < 0)
            {
                character.runVelocity.z += dampSpeed * Time.deltaTime;
                if(character.runVelocity.z > 0)
                {
                    character.runVelocity.z = 0;
                }
            }
            if (character.runVelocity.x > 0)
            {
                character.runVelocity.x -= dampSpeed * Time.deltaTime;
                if (character.runVelocity.x < 0)
                {
                    character.runVelocity.x = 0;
                }
            }
            else if (character.runVelocity.x < 0)
            {
                character.runVelocity.x += dampSpeed * Time.deltaTime;
                if (character.runVelocity.x > 0)
                {
                    character.runVelocity.x = 0;
                }
            }
        }

    }

}
