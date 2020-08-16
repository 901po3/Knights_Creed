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
            //CharacterControl character = characterState.GetCharacterControl(animator);
            //if(!character.isDetected)
            //{
            //    character.curUndetectedTimer += Time.deltaTime;
            //    if(character.curUndetectedTimer >= character.undetectedTime)
            //    {
            //        character.curUndetectedTimer = 0;
            //        character.isBattleModeOne = false;
            //        character.isChangingMode = true;
            //        TurnOffBattleMode(character);
            //    }
            //}
            //else
            //{
            //    character.curUndetectedTimer = 0f;
            //}
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            //CharacterControl character = characterState.GetCharacterControl(animator);
            //Equipment.ToogleWeapon(character);
        }

        private void TurnOffBattleMode(CharacterControl character)
        {
            //전투모드로 해제
            if (!character.isBattleModeOn)
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
