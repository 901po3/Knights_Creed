﻿/*
 * Class: Dodge
 * Date: 2020.8.13
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: Dodge 상태 조정
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Dodge")]
    public class Dodge : StateData
    {
        public float turnSpeed;
        public float speed;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.curUndetectedTimer = 0; //피하기 시도하면-> 전투 해제 시간 리셋

            //피하기 종류 선택
            switch (character.attacker.medAttackType)
            {
                case MED_ATTACK_TYPE.HIGH:
                    character.curAimTime = 1.5f;
                    animator.SetFloat("RandomHit", 0);
                    break;
                case MED_ATTACK_TYPE.MIDDLE:
                    character.curAimTime = 1.73f;
                    animator.SetFloat("RandomHit", 1);
                    break;
                case MED_ATTACK_TYPE.LOW:
                    character.curAimTime = 1.5f;
                    animator.SetFloat("RandomHit", 2);
                    break;
            }
            Transform attackerTrans = character.attacker.transform;
            attackerTrans.position = new Vector3(character.attacker.transform.position.x,
                character.transform.position.y, character.attacker.transform.position.z);
            character.transform.LookAt(attackerTrans);

            character.dodgeTimer = 0;
            character.curAnimSpeed = speed;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            if (character.targetEnemy != null)
            {
                //적과 거리가 짧고 중단 공격이면 피해를 받는다 
                if (Vector3.Distance(character.transform.position, character.targetEnemy.transform.position) < 0.5f
                    && character.attacker.medAttackType == MED_ATTACK_TYPE.MIDDLE)
                {
                    character.isDodging = false;
                    character.GetAnimator().SetBool("Dodge", false);
                    return;
                }
            }

            //자연스러운 애니메이션을 위한 속도 조정
            character.dodgeTimer += Time.deltaTime;
            if (character.curAnimSpeed > 0)
            {
                character.curAnimSpeed -= (speed * 4) * Time.deltaTime;
            }

            //뒤로 이동
            character.GetRigidbody().MovePosition(character.transform.position - character.transform.forward * character.curAnimSpeed * Time.fixedDeltaTime);

            //dodgeDuration 이후에 피하기 상태 해제
            if (character.dodgeTimer >= character.curAimTime - Time.deltaTime)
            {
                character.isDodging = false;
                character.GetAnimator().SetBool("Dodge", false);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }
    }
}
