/*
 * Class: StateData
 * Date: 2020.8.10
 * Last Modified : 2020.8.12
 * Author: Hyukin Kwon 
 * Description:  모든 스크립터블오브젝트 스테이트들의 최상위 클래스
*/
using UnityEngine;

namespace HyukinKwon
{
    public abstract class StateData : ScriptableObject
    {
        public float duration; //애니메이션 지속시간

        //각 스테이트 오브젝트들의 스타트 함수
        public abstract void StartAbility(CharacterState characterState, Animator animator);

        //각 스테이트 오브젝트들의 업데이트 함수
        public abstract void UpdateAbility(CharacterState characterState, Animator animator);

        //각 스테이트 오브젝트들의 나가기 함수
        public abstract void ExitAbility(CharacterState characterState, Animator animator);
    }

}