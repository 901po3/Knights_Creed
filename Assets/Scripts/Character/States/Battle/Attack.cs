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

        public override void StartAbility(CharacterState characterState, Animator animator)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            if (character.isAttacking)
            {
                animator.SetBool("SwingSword", true);
                //기준 방향 정면으로 회전
                Vector3 targetDirection = character.runVelocity.normalized;
                targetDirection.y = 0f;
                character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(character.transform.forward,
                    targetDirection, turnSpeed * Time.fixedDeltaTime, 0f)));
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