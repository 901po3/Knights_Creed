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
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Dodge")]
    public class Dodge : StateData
    {
        public float dodgeDuration; //피하기 지속시간
        public float dodgeTime = 0f; //파하기 타이머

        public override void StartAbility(CharacterState characterState, Animator animator)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            //dodgeDuration 이후에 피하기 상태 해제
            CharacterControl character = characterState.GetCharacterControl(animator);
            if (character.isDodging)
            {
                if (dodgeTime < dodgeDuration)
                {
                    dodgeTime += Time.deltaTime;
                    if (dodgeTime >= dodgeDuration)
                    {
                        dodgeTime = 0f;
                        character.isDodging = false;
                        character.GetAnimator().SetBool("Dodging", false);
                    }
                }
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }
    }
}
