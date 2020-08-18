/*
 * Class: AddForce
 * Date: 2020.8.12
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: 애니메이션 시작에 AddForce
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/AddForce")]
    public class AddForce : StateData
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

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            switch (direction) //방향에 맞게 포스 추가
            {
                case DIRECTION.FOWARD:
                    character.GetRigidbody().AddForce(character.transform.position + character.transform.forward * power);
                    break;
                case DIRECTION.BACKWARD:
                    character.GetRigidbody().AddForce(character.transform.position - character.transform.forward * power);
                    break;
                case DIRECTION.LEFT:
                    character.GetRigidbody().AddForce(character.transform.position - character.transform.right * power);
                    break;
                case DIRECTION.RIGHT:
                    character.GetRigidbody().AddForce(character.transform.position + character.transform.right * power);
                    break;
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            
        }
    }

}
