/*
 * Class: FreezePosition
 * Date: 2020.8.13
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description:  AI 이동잠금
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/FreezePosition")]
    public class FreezePosition : StateData
    {
        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            if (character.tag != "Player")
            {
                character.GetRigidbody().constraints = RigidbodyConstraints.FreezePositionX
                    | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                character.GetRigidbody().freezeRotation = true;
            }
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            if (character.tag != "Player")
            {
                character.GetRigidbody().constraints = RigidbodyConstraints.None;
                character.GetRigidbody().freezeRotation = true;
            }
        }
    }

}