﻿/*
 * Class: Turn
 * Date: 2020.8.14
 * Last Modified : 2020.8.15
 * Author: Hyukin Kwon 
 * Description:  급회전 애니메이션
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Turn")]
    public class Turn : StateData
    {
        public enum DIRECTION
        {
            LEFT, RIGHT
        }

        public DIRECTION direction;
        [Range(0, 200)]
        public float speed;
        Vector3 targetDirection;
        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            //목표 방향값 저장 
            CharacterControl character = characterState.GetCharacterControl(animator); 
            character.turnTimer = 0f;
            targetDirection = targetDirection = character.runVelocity.normalized;
            targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
            targetDirection.y = 0f;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            //천천히 목표 방향값으로 회전
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.turnTimer += Time.deltaTime;
            if(character.turnTimer > duration)
            {
                character.turning = false;
                animator.SetBool("TurnLeft", false);
                animator.SetBool("TurnRight", false);
            }

            if (Vector3.Angle(character.transform.forward, targetDirection) > 2.5f) //제안두기
            {
                if (direction == DIRECTION.LEFT)
                {
                    character.transform.Rotate(Vector3.up * speed * Time.fixedDeltaTime);
                }
                else
                {
                    character.transform.Rotate(Vector3.up * -speed * Time.fixedDeltaTime);
                }
            }
            else
            {
                character.turning = false;
                animator.SetBool("TurnLeft", false);
                animator.SetBool("TurnRight", false);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            //목표 방향값으로 회전
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.GetRigidbody().rotation = Quaternion.LookRotation(targetDirection);
        }
    }

}