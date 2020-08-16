/*
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
        public enum DODGE_TPYE
        {
            DASH, ATTACK, BACK
        }

        public float turnSpeed;
        public float speed;
        private DODGE_TPYE dodgeType;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.curUndetectedTimer = 0; //피하기 시도하면-> 전투 해제 시간 리셋
            //피하기 종류 선택
            if (character.isAttacking)
            {
                character.curAimTime = 0.8f;
                dodgeType = DODGE_TPYE.BACK;

                character.isAttacking = false;
                animator.SetBool("SwingSword", false);
            }
            else if (character.runVelocity.normalized.magnitude < 0.1f && !character.isAttacking)
            {
                character.curAimTime = 0.8f;
                dodgeType = DODGE_TPYE.BACK;
            }
            else if(character.runVelocity.normalized.magnitude >= 0.1f)
            {
                character.curAimTime = 1.2f;
                dodgeType = DODGE_TPYE.DASH;
            }
            character.dodgeTimer = 0;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.dodgeTimer += Time.deltaTime;

            if (character.dodgeTimer < character.curAimTime - 0.3f)
            {
                switch (dodgeType) //타입에 맞게 이동
                {
                    case DODGE_TPYE.DASH:
                        character.GetRigidbody().MovePosition(character.transform.position + character.transform.forward * speed * 1.3f * Time.fixedDeltaTime);
                        break;
                    case DODGE_TPYE.BACK:
                        character.GetRigidbody().MovePosition(character.transform.position - character.transform.forward * speed * 0.3f * Time.fixedDeltaTime);
                        break;
                }
            }

            //dodgeDuration 이후에 피하기 상태 해제
            if (character.dodgeTimer >= character.curAimTime - Time.deltaTime)
            {
                character.isDodging = false;
                character.GetAnimator().SetBool("Dodging", false);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }
    }
}
