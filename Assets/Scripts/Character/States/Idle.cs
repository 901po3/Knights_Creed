/*
 * Class: Idle
 * Date: 2020.8.10
 * Last Modified : 2020.8.12
 * Author: Hyukin Kwon 
 * Description:  서 있는 애니메이션 스크립터블오브젝트
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Idle")]
    public class Idle : StateData
    {
        public override void StartAbility(CharacterState characterState, Animator animator)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            //이동 중이면 이동 애니메이션 재생
            //방향은 무관하게 앞으로 이동 속도만 전달
            animator.SetFloat("RunningVeritical", Mathf.Abs(character.runVelocity.normalized.magnitude));
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }
    }

}