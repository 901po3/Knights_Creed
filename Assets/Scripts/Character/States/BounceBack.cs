/*
 * Class: BounceBack
 * Date: 2020.8.12
 * Last Modified : 2020.8.18
 * Author: Hyukin Kwon 
 * Description: 애니메이션 시작에 AddForce
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/BounceBack")]
    public class BounceBack : StateData
    {
        public enum DIRECTION
        {
            FOWARD, BACKWARD, LEFT, RIGHT
        }

        [Range(0, 1000)]
        public float power;
        public DIRECTION direction;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            character.GetRigidbody().MovePosition(character.transform.position - character.transform.forward * power);
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {

        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            
        }
    }

}
