/*
 * Class: Turn
 * Date: 2020.8.14
 * Last Modified : 2020.8.15
 * Author: Hyukin Kwon 
 * Description:  급회전 애니메이션
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Turn")]
    public class Turn : StateData
    {
        public enum DIRECTION
        {
            LEFT, RIGHT
        }

        public DIRECTION direction;
        [Range(0, 200)]
        public float speed;
        Vector3 targetDirection;
        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            //목표 방향값 저장 
            CharacterControl character = characterState.GetCharacterControl(animator); 
            character.turnTimer = 0f;
            targetDirection = targetDirection = character.runVelocity.normalized;
            targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
            targetDirection.y = 0f;

            //character.curAimTime 시간 이후에 회전 시작
            //용도: 자연스러운 애니메이션 재생    
            character.curAimTime = duration;
            if(duration == 1)
            {
                character.startTurnTimer = 0.13f;
            }
            else
            {
                character.startTurnTimer = 0.3f;
            }
            character.curAnimSpeed = speed;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            //천천히 목표 방향값으로 회전
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.turnTimer += Time.deltaTime;

            //회전 시간이 끝나면 아웃
            if(character.turnTimer > character.curAimTime - Time.deltaTime)
            {
                character.GetRigidbody().rotation = Quaternion.LookRotation(targetDirection);
                character.turning = false;
                animator.SetBool("TurnLeft", false);
                animator.SetBool("TurnRight", false);
            }

            Debug.Log(character.curAnimSpeed);

            if (Vector3.Angle(character.transform.forward, targetDirection) > 2.5f) //제안두기
            {
                if(character.turnTimer > character.startTurnTimer)
                {
                    if (direction == DIRECTION.LEFT)
                    {
                        character.transform.Rotate(Vector3.up * character.curAnimSpeed * Time.fixedDeltaTime);
                    }
                    else
                    {
                        character.transform.Rotate(Vector3.up * -character.curAnimSpeed * Time.fixedDeltaTime);
                    }
                }
            }
            else
            {
                //시간이 끝나기전에 회전이 끝나면 아웃
                character.GetRigidbody().rotation = Quaternion.LookRotation(targetDirection);
                character.turning = false;
                animator.SetBool("TurnLeft", false);
                animator.SetBool("TurnRight", false);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
        }
    }

}