/*
 * Class: Attack
 * Date: 2020.8.12
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: 공격 
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Attack")]
    public class Attack : StateData
    {
        public float damage;
        public float turnSpeed;
        private float time; //duration만 회전 가능

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            time = 0f;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            if (character.isAttacking)
            {
                animator.SetBool("SwingSword", true);
                if(time < duration)
                {
                    time += Time.deltaTime;

                    //회전
                    Vector3 targetDirection = character.runVelocity.normalized;
                    targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
                    targetDirection.y = 0f;
                    character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(character.transform.forward,
                        targetDirection, turnSpeed * Time.fixedDeltaTime, 0f)));
                }
            }
            else
            {
                animator.SetBool("SwingSword", false);
            }          
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }
    }

}