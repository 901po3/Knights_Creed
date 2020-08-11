/*
 * Class: Idle
 * Date: 2020.8.10
 * Last Modified : 2020.8.10
 * Author: Hyukin Kwon 
 * Description:  서 있는 애니메이션 스크립터블오브젝트
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Idle")]
    public class Idle : StateData
    {
        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            if (character.runningState > 0.1f) //이동 중이면 이동 애니메이션 재생
            {
                animator.SetFloat("RunningVeritical", character.runningState);
            }
        } 
    }

}