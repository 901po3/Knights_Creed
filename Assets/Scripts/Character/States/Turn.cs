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
        float time = 0;
        float speed;
        Vector3 targetDirection;
        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            time = 0;
            CharacterControl character = characterState.GetCharacterControl(animator);
            speed = 4;
            targetDirection = targetDirection = character.runVelocity.normalized;
            targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
            targetDirection.y = 0f;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards
                        (character.transform.forward, targetDirection, speed * Time.fixedDeltaTime, 0f)));

            time += Time.deltaTime;
            if(time > animator.GetCurrentAnimatorStateInfo(0).length - Time.deltaTime)
            {
                character.turning = false;
                animator.SetBool("Turn", false);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.GetRigidbody().rotation = Quaternion.LookRotation(targetDirection);
        }
    }

}