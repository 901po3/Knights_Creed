/*
 * Class: Idle
 * Date: 2020.8.10
 * Last Modified : 2020.8.15
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
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.isChangingMode = false;

            //전투 비전투 모드에 따라 다른 애니메이션 재생
            if(!character.isBattleModeOn)
            {
                animator.SetFloat("RandomIdle", (float)character.NormalIdleType);
            }
            else
            {
                animator.SetFloat("RandomIdle", (float)character.BattleIdleType);
            }
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            //제자리 회전 시작
            Vector3 targetDirection = character.runVelocity.normalized;
            targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
            targetDirection.y = 0f;
            if(!character.isBattleModeOn)
            {
                if (Vector3.Angle(character.transform.forward, targetDirection) > 160) //빠른 180도 회전
                {
                    Vector3 cross = Vector3.Cross(character.transform.rotation * Vector3.forward, Quaternion.Euler(targetDirection) * Vector3.forward);
                    if (cross.y > 0)
                    {
                        animator.SetBool("TurnRight", true);
                    }
                    else
                    {
                        animator.SetBool("TurnLeft", true);
                    }
                    character.turning = true;
                    return;
                }
            }

            //이동 중이면 이동 애니메이션 재생
            //방향은 무관하게 앞으로 이동 속도만 전달
            if (!character.turning)
            {
                animator.SetFloat("RunningVeritical", Mathf.Abs(character.runVelocity.normalized.magnitude));
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
        }
    }

}