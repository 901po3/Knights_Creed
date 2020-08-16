﻿/*
 * Class: MoveBattleMode
 * Date: 2020.8.15
 * Last Modified : 2020.8.15
 * Author: Hyukin Kwon 
 * Description: 전투모드 이동
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/MoveBattleMode")]
    public class MoveBattleMode : StateData
    {
        public float runSpeed;
        public float turnSpeed;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            if (character.runVelocity.normalized.magnitude == 0) //서있기로 전환
            {
                animator.SetFloat("RunningVeritical", 0);
                return;
            }

            Move(character, animator); //이동 함수
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }

        //정면 이동 함수
        private void Move(CharacterControl character, Animator animator)
        {
            Vector3 curRunVelocity = character.runVelocity;
            //속도 적용
            float power = curRunVelocity.normalized.magnitude;
            animator.SetFloat("RunningVeritical", power);
            //지정된 방향 기준을 중심으로 이동 
            Vector3 forward = character.facingStandardTransfom.transform.forward;
            Vector3 right = character.facingStandardTransfom.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 dir = forward * curRunVelocity.z + right * curRunVelocity.x;
            character.GetRigidbody().MovePosition(character.transform.position + dir * power * runSpeed * Time.fixedDeltaTime);
            animator.SetFloat("BattleMoveHorizontal", curRunVelocity.x);
            animator.SetFloat("BattleMoveVertical", curRunVelocity.z);

            //회전
            Vector3 targetDirection = character.facingStandardTransfom.forward;
            targetDirection.y = 0f;

            float rotSpeed = turnSpeed;

            character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards
                (character.transform.forward, targetDirection, rotSpeed * Time.fixedDeltaTime, 0f)));


            character.prevRunVelocity = curRunVelocity;
        }
    }
}