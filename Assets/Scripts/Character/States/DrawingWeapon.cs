/*
 * Class: DrawingWeapon
 * Date: 2020.8.12
 * Last Modified : 2020.8.12
 * Author: Hyukin Kwon 
 * Description: 무기를 뺴고 전투에 돌입
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/DrawingWeapon")]
    public class DrawingWeapon : StateData
    {
        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            Debug.Log("Drawing Weapon");
            Equipment.ToogleWeapon(character);
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
        }
    }

}