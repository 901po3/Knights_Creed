/*
 * Class: Turn
 * Date: 2020.8.14
 * Last Modified : 2020.8.14
 * Author: Hyukin Kwon 
 * Description:  급회전 애니메이션
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Turn")]
    public class Turn : StateData
    {
        [Range(0, 10)]
        public float speed;
        Vector3 cross;
        Vector3 targetDirection;
        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            //목표 방향값 저장 
            CharacterControl character = characterState.GetCharacterControl(animator); 
            character.turnTimer = 0f;
            targetDirection = targetDirection = character.runVelocity.normalized;
            targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
            targetDirection.y = 0f;

            cross = Vector3.Cross(character.transform.rotation * Vector3.forward, Quaternion.Euler(targetDirection) * Vector3.forward);
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            //천천히 목표 방향값으로 회전
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.turnTimer += Time.deltaTime;
            if(character.turnTimer > duration)
            {
                character.turning = false;
                animator.SetBool("TurnLeft", false);
                animator.SetBool("TurnRight", false);
            }
            character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards
                    (character.transform.forward, targetDirection, speed * Time.fixedDeltaTime, 0)));
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            //목표 방향값으로 회전
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.GetRigidbody().rotation = Quaternion.LookRotation(targetDirection);
        }
    }

}