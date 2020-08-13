/*
 * Class: ToBattle
 * Date: 2020.8.12
 * Last Modified : 2020.8.12
 * Author: Hyukin Kwon 
 * Description: 전투모드 돌입 체크
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/ToBattle")]
    public class ToBattle : StateData
    {
        public override void StartAbility(CharacterState characterState, Animator animator)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            //전투모드로 돌입하고 무기 뽑기 애니메이션으로 이동
            CharacterControl character = characterState.GetCharacterControl(animator);
            if (character.isBattleModeOne)
            {
                if(!character.isDrawingWeapon)
                {
                    character.isDrawingWeapon = true;
                    animator.SetBool("BattleModeOn", true);
                    animator.SetTrigger("DrawingWeapon");
                }
                return;
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            Equipment.ToogleWeapon(character);
        }
    }
}
