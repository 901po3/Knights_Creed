/*
 * Class: CharacterState
 * Date: 2020.8.10
 * Last Modified : 2020.8.12
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

        //애니메이션이 시작할때 능력 전부 업데이트
        public void StartAll(CharacterState characterState, Animator animator)
        {
            foreach (StateData data in abilityDataList)
            {
                data.StartAbility(characterState, animator);
            }
        }

        //매 업테이트마다 능력 전부 업테이트
        public void UpdateAll(CharacterState characterState, Animator animator)
        {
            foreach(StateData data in abilityDataList)
            {
                data.UpdateAbility(characterState, animator);
            }
        }

        //애니메이션이 끝날때 능력 전부 업데이트
        public void ExitAll(CharacterState characterState, Animator animator)
        {
            foreach (StateData data in abilityDataList)
            {
                data.ExitAbility(characterState, animator);
            }
        }

        //MonoBehaviour의 Start 동일한 함수
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StartAll(this, animator);
        }

        //MonoBehaviour의 Update와 동일한 함수
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            UpdateAll(this, animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ExitAll(this, animator);
        }

    }
}