/*
 * Class: MoveVertical
 * Date: 2020.8.10
 * Last Modified : 2020.8.10
 * Author: Hyukin Kwon 
 * Description:  앞뒤 이동 에니메이션 스크립터블오브젝트
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/MoveVertical")]
    public class MoveFoward : StateData
    {
        public float runSpeed;
        public float turnSpeed;

        //속도에 따라 앞뒤 이동과 맞는 애니메이션 재생
        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            if (character.runVelocity == Vector3.zero) //서있기로 전환
            {
                animator.SetFloat("RunningVeritical", 0);
                return;
            }

            //속도 적용
            float dir = character.runVelocity.normalized.magnitude;
            Debug.Log(dir);
            animator.SetFloat("RunningVeritical", dir);
            character.transform.Translate(Vector3.forward * runSpeed * dir * Time.deltaTime);
            //회전
            character.transform.rotation = 
                Quaternion.LookRotation(Vector3.RotateTowards(character.transform.forward, character.runVelocity, turnSpeed * Time.deltaTime, 0f));
        }
    }
}
