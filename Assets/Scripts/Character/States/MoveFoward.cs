﻿/*
 * Class: MoveVertical
 * Date: 2020.8.10
 * Last Modified : 2020.8.11
 * Author: Hyukin Kwon 
 * Description: 이동 에니메이션 스크립터블오브젝트
 *              캐릭터는 항상 바라보는 방향기준 앞으로 이동
 *              다른 방향으로 이동하면 회전 후 이동
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
            float power = character.runVelocity.normalized.magnitude;
            animator.SetFloat("RunningVeritical", power);
            //지정된 방향 기준을 중심으로 이동 
            character.GetRigidbody().MovePosition(character.transform.position + character.transform.forward * power * runSpeed * Time.deltaTime);
            //회전
            Vector3 targetDirection = character.runVelocity.normalized;
            targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
            targetDirection.y = 0f;
            character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(character.transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0f)));
        }
    }
}