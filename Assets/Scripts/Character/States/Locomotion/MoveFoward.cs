/*
 * Class: MoveVertical
 * Date: 2020.8.10
 * Last Modified : 2020.8.11
 * Author: Hyukin Kwon 
 * Description: 이동 에니메이션 스크립터블오브젝트
 *              캐릭터는 항상 바라보는 방향기준 앞으로 이동
 *              다른 방향으로 이동하면 회전 후 이동
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/MoveVertical")]
    public class MoveFoward : StateData
    {
        public float runSpeed;
        public float turnSpeed;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            if (character.runVelocity.normalized.magnitude == 0) //서있기로 전환
            {
                animator.SetFloat("RunningVeritical", 0);
                return;
            }

            Move(character, animator); //이동 함수
            RotateToForward(character, animator); //회전 함수 (정면으로)
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }

        //정면 이동 함수
        private void Move(CharacterControl character, Animator animator)
        {
            Vector3 curRunVelocity = character.runVelocity;
            //속도 적용
            float power = curRunVelocity.normalized.magnitude;
            animator.SetFloat("RunningVeritical", power);
            //지정된 방향 기준을 중심으로 이동 
            character.GetRigidbody().MovePosition(character.transform.position + character.transform.forward * power * runSpeed * Time.fixedDeltaTime);

            float dampSpeed = 3;
            if (curRunVelocity.x != 0 && curRunVelocity.z != 0)
            {
                Vector3 cross = Vector3.Cross(curRunVelocity.normalized, character.prevRunVelocity.normalized);
                if(curRunVelocity.x == curRunVelocity.z)
                {
                    character.horizontalV = Mathf.Lerp(character.horizontalV, 0, 0.5f);
                }
                else if (cross.y < 0) //오른쪽 회전 중이면서 이동
                {
                    character.horizontalV += Time.deltaTime * dampSpeed;
                }
                else if (cross.y > 0) //왼쪽 회전 중이면서 이동
                {
                    character.horizontalV -= Time.deltaTime * dampSpeed;
                }
                character.horizontalV = Mathf.Clamp(character.horizontalV, -1, 1);
            }
            else
            {
                if(character.horizontalV > 0)
                {
                    character.horizontalV -= Time.deltaTime * dampSpeed;
                    if(character.horizontalV < 0)
                    {
                        character.horizontalV = 0;
                    }
                }
                else if(character.horizontalV < 0)
                {
                    character.horizontalV += Time.deltaTime * dampSpeed;
                    if (character.horizontalV > 0)
                    {
                        character.horizontalV = 0;
                    }
                }
            }
            animator.SetFloat("RunningHorizontal", character.horizontalV);

            character.prevRunVelocity = curRunVelocity;
        }

        //기준 정면으로 회전 함수
        private void RotateToForward(CharacterControl character, Animator animator)
        {
            Vector3 curRunVelocity = character.runVelocity;
            //회전
            Vector3 targetDirection = curRunVelocity.normalized;
            targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
            targetDirection.y = 0f;

            //Debug.Log(Vector3.Angle(character.transform.forward, targetDirection));
            float rotSpeed = turnSpeed;

            //빠른 180도 회전
            if (Vector3.Angle(character.transform.forward, targetDirection) > 160)
            {
                if (curRunVelocity.x > 0.5f || curRunVelocity.z < -0.5f)
                {
                    animator.SetBool("TurnRight", true);
                }
                else if (curRunVelocity.x < -0.5f || curRunVelocity.z > 0.5f)
                {
                    animator.SetBool("TurnLeft", true);
                }
                character.turning = true;
                return;
            }

            character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards
                (character.transform.forward, targetDirection, rotSpeed * Time.fixedDeltaTime, 0f)));
        }
    }
}
