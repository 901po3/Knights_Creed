/*
 * Class: Hurt
 * Date: 2020.8.13
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description:  피해 애니메이션
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Hurt")]
    public class Hurt : StateData
    {
        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            if(character.health > 0)
            {
                if (character.hurtTimer < duration)
                {
                    character.hurtTimer += Time.deltaTime;
                    if (character.hurtTimer >= duration)
                    {
                        character.hurtTimer = 0f;
                        character.isHurt = false;
                        animator.SetBool("HurtRight", false);
                        character.isBattleModeOne = true;
                    }
                }
            }
            else
            {
                character.hurtTimer = 0f;
                character.isHurt = false;
                animator.SetBool("HurtRight", false);
                animator.SetBool("Dead", true);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
           
        }
    }

}