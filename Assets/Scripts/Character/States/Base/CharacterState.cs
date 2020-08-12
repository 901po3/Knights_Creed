/*
 * Class: CharacterState
 * Date: 2020.8.10
 * Last Modified : 2020.8.11
 * Author: Hyukin Kwon 
 * Description: 애니메이션에 들어가는 스크립트로
 *              사용 중인 모든 애니메이션 스크립터블오브젝트를 재생
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyukinKwon
{
    public class CharacterState : StateMachineBehaviour
    {
        //애니메이션별로 필요한 애니메이션 스크립터블오브젝트들을 골라 사용
        public List<StateData> abilityDataList = new List<StateData>();

        //캐릭터 Gameobject의 CharacterControl 스크립트
        private CharacterControl characterControl;
        public CharacterControl GetCharacterControl(Animator animator)
        {
            if (characterControl == null)
            {
                characterControl = animator.GetComponentInParent<CharacterControl>();
            }
            return characterControl;
        }

        //포함된 모든 애니메이션 스크립터블오브젝트들을 재생
        public void UpdateAll(CharacterState characterState, Animator animator)
        {
            foreach(StateData data in abilityDataList)
            {
                data.UpdateAbility(characterState, animator);
            }
        }

        //MonoBehaviour의 Update와 동일한 함수
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            UpdateAll(this, animator);
        }

    }
}