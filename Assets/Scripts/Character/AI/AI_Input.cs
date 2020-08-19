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
            MOVE_TO_TARGET, IDLE_01, REVOVLE_LEFT_TARGET, REVOVLE_RIGHT_TARGET,

            //거리가 가까울때 선택 가능
            MOVE_BACK_DODGE, MOVE_BACK, IDLE_02,

            //거리가 적당할때 선택 가능
            DODGE, PARRY, ATTACK
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
        private bool runCoroutineOnce = false;

        private void Awake()
        {
            character = GetComponent<CharacterControl>();
            //자기 자신으로 기준
            character.facingStandardTransfom = character.transform;
        }

        private void Update()
        {
            if(movingDirection != Vector3.zero)
            {
                StartMoving(movingDirection);
            }
            else
            {
                StopMoving();
            }

            if (character.runVelocity.z != 0 && decided && character.currentState != CURRENT_STATE.MOVE_DODGE) //뒤로 이동중
            {
                if (character.targetEnemy != null) //타겟이 있다면 거리 체크
                {
                    CheckDistance();
                    if (distanceState == DISTANCE_STATE.GOOD)
                    {
                        movingDirection = Vector3.zero;
                        nextDecicionTime = 1f;
                        StartCoroutine(NextDecicionDelay());
                        return;
                    }
                }
                else //타겟이 없으면 멈춤
                {
                    movingDirection = Vector3.zero;
                    nextDecicionTime = 1f;
                    StartCoroutine(NextDecicionDelay());
                    return;
                }
            }

            if (!character.isDrawingWeapon && !decided) return;

            if (decided)
            {
                if(!runCoroutineOnce)
                {
                    runCoroutineOnce = true;
                    StartCoroutine(NextDecicionDelay());
                }
                return;
            }

            // 1. AI 자신의 상태 체크
            if (character.currentState != CURRENT_STATE.NONE || character.runVelocity != Vector3.zero)
            {
                // 외부에서 정해지는 State 상태
                if(!decided)
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
            targetScript = character.targetEnemy.GetComponent<CharacterControl>();

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

        private void CheckDistance()
        {
            disToTarget = Vector3.Distance(character.transform.position, targetScript.transform.position);

            if (disToTarget > character.chargeDis) // 3-1. 거리가 멀다
            {
                distanceState = DISTANCE_STATE.FAR;
            }
            else if (disToTarget < character.chargeDis - 0.5f) // 3-2. 거리가 짧다
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
                    decicion = (DECICION)Random.Range(0, 4); // 4-1. MOVE_TO_TARGET, IDLE, REVOVLE_TARGER 중에서 선택
                    break;
                case DISTANCE_STATE.CLOSE:
                    decicion = (DECICION)Random.Range(4, 7); // 4-2. MOVE_BACK_DODGE, MOVE_BACK 중에서 선택
                    break;
                case DISTANCE_STATE.GOOD:
                    decicion = (DECICION)Random.Range(7, 10); // 4-3. DODGE, PARRY, ATTACK 중에서 선택
                    break;
            }
            // 4.1 AI가 타겟인 경우 회전 이동 방향 수정
            if (targetScript.tag == "AI")
            {
                if (targetScript.GetComponent<AI_Input>().decicion == DECICION.REVOVLE_LEFT_TARGET
                    && decicion == DECICION.REVOVLE_LEFT_TARGET)
                {
                    decicion = DECICION.REVOVLE_RIGHT_TARGET;
                }
                else if (targetScript.GetComponent<AI_Input>().decicion == DECICION.REVOVLE_RIGHT_TARGET
                    && decicion == DECICION.REVOVLE_RIGHT_TARGET)
                {
                    decicion = DECICION.REVOVLE_LEFT_TARGET;
                }
            }
        }

        private void UpdateStateByDecicion()
        {
            switch (decicion)
            {
                case DECICION.MOVE_TO_TARGET:
                    //최대 3~7초간 앞으로 이동
                    movingDirection = Vector3.forward;
                    nextDecicionTime = Random.Range(3f, 7f);
                    break;
                case DECICION.IDLE_01:
                case DECICION.IDLE_02:
                    //2~4초간 대기
                    nextDecicionTime = Random.Range(2f, 4f);
                    break;
                case DECICION.REVOVLE_LEFT_TARGET:
                    //2~5초간 좌로 이동
                    movingDirection = Vector3.left;
                    nextDecicionTime = Random.Range(2f, 5f);
                    break;
                case DECICION.REVOVLE_RIGHT_TARGET:
                    //2~5초간 우로 이동
                    movingDirection = Vector3.right;
                    nextDecicionTime = Random.Range(2f, 5f);
                    break;
                case DECICION.MOVE_BACK:
                    //거리가 적당해질떄까지 뒤로 이동
                    movingDirection = Vector3.back;
                    nextDecicionTime = 99999f;
                    break;
                case DECICION.MOVE_BACK_DODGE:
                    character.runVelocity.z = -1f;
                    character.currentState = CURRENT_STATE.MOVE_DODGE;
                    nextDecicionTime = 3.5f;
                    break;
                case DECICION.DODGE:
                    character.currentState = CURRENT_STATE.DODGE;
                    nextDecicionTime = 3f;
                    break;
                case DECICION.PARRY:
                    character.currentState = CURRENT_STATE.PARRY;
                    nextDecicionTime = 3f;
                    break;
                case DECICION.ATTACK:
                    character.currentState = CURRENT_STATE.ATTACK;
                    nextDecicionTime = 3f;
                    break;
            }
        }

        IEnumerator NextDecicionDelay()
        {
            yield return new WaitForSeconds(nextDecicionTime);
            runCoroutineOnce = false;
            decided = false;
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
