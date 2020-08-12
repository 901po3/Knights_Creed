/*
 * Class: AttackTransition
 * Date: 2020.8.12
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: 공격 활성
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/AttackTransition")]
    public class AttackTransition : StateData
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