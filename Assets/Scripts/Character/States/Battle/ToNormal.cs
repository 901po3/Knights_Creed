/*
 * Class: ToNormal
 * Date: 2020.8.12
 * Last Modified : 2020.8.12
 * Author: Hyukin Kwon 
 * Description: 전투모드 해제 체크
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/ToNormal")]
    public class ToNormal : StateData
    {
        public override void StartAbility(CharacterState characterState, Animator animator)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            if(!character.isDetected)
            {
                character.curUndetectedTime += Time.deltaTime;
                if(character.curUndetectedTime >= character.undetectedTime)
                {
                    character.curUndetectedTime = 0;
                    character.isBattleModeOne = false;
                    TurnOffBattleMode(character);
                }
            }
            else
            {
                character.curUndetectedTime = 0f;
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            Debug.Log("Drawing Weapon");
            Equipment.ToogleWeapon(character);
        }

        private void TurnOffBattleMode(CharacterControl character)
        {
            //전투모드로 해제
            if (!character.isBattleModeOne)
            {
                if (character.isDrawingWeapon)
                {
                    character.isDrawingWeapon = false;
                    character.GetAnimator().SetBool("BattleModeOn", false);
                }
                return;
            }
        }
    }
}
