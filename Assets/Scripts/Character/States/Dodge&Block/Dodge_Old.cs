/*
 * Class: Dodge
 * Date: 2020.8.13
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: Dodge 상태 조정
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Dodge_Old")]
    public class Dodge_Old : StateData
    {
        public float turnSpeed;
        public float speed;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.curUndetectedTimer = 0; //피하기 시도하면-> 전투 해제 시간 리셋
            //피하기 종류 선택
            if (character.isAttacking)
            {
                character.isAttacking = false;
                animator.SetBool("SwingSword", false);
            }
            if(character.runVelocity.normalized.magnitude >= 0.1f)
            {
                character.curAimTime = 1.2f;
            }
            character.dodgeTimer = 0;           
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.dodgeTimer += Time.deltaTime;

            if (character.dodgeTimer < character.curAimTime - 0.3f)
            {
                if(character.dodgeTimer < 0.2f)
                {
                    RotateToForward(character, animator);
                }
                character.GetRigidbody().MovePosition(character.transform.position + character.transform.forward * speed * 1.3f * Time.fixedDeltaTime);
            }

            //dodgeDuration 이후에 피하기 상태 해제
            if (character.dodgeTimer >= character.curAimTime - Time.deltaTime)
            {
                character.isDodging = false;
                character.GetAnimator().SetBool("Dodge", false);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

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
            if (curRunVelocity.x > 0.5f || curRunVelocity.z < -0.5f)
            {
                animator.SetBool("TurnRight", true);
            }
            else if (curRunVelocity.x < -0.5f || curRunVelocity.z > 0.5f)
            {
                animator.SetBool("TurnLeft", true);
            }
            character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards
                (character.transform.forward, targetDirection, rotSpeed * Time.fixedDeltaTime, 0f)));
        }
    }
}
