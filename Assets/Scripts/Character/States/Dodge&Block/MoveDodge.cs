/*
 * Class: MoveDodge
 * Date: 2020.8.13
 * Last Modified : 2020.8.17
 * Author: Hyukin Kwon 
 * Description: Dodge 상태 조정
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/MoveDodge")]
    public class MoveDodge : StateData
    {
        public float turnSpeed;
        public float speed;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            character.curUndetectedTimer = 0; //피하기 시도하면-> 전투 해제 시간 리셋
            //피하기 종류 선택
            if (character.isAttacking)
            {
                character.isAttacking = false;
                animator.SetBool("SwingSword", false);
            }
            if(character.runVelocity.normalized.magnitude >= 0.1f)
            {
                character.curAimTime = 1.2f;
            }
            character.parryDodgeTimer = 0;
            character.isDodging = true;
            character.moveParryDodgeVec = new Vector3(character.runVelocity.x, 0, character.runVelocity.z);
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            //시간 업데이트
            character.parryDodgeTimer += Time.deltaTime;

            //지정된 방향 기준을 중심으로 이동
            Vector3 curRunVelocity = character.runVelocity;
            Vector3 forward = character.transform.forward;
            Vector3 right = character.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 dir = forward * character.moveParryDodgeVec.z + right * character.moveParryDodgeVec.x;

            if (character.parryDodgeTimer < character.curAimTime - 0.6f)
            {
                if(character.parryDodgeTimer > 0.3f)
                {
                    character.isDodging = false;
                }
                if(character.moveParryDodgeVec.z == 0 && character.moveParryDodgeVec.x != 0)
                {
                    character.GetRigidbody().MovePosition(character.transform.position + dir * speed * 1.5f * Time.fixedDeltaTime);
                }
                else
                {
                    character.GetRigidbody().MovePosition(character.transform.position + dir * speed * Time.fixedDeltaTime);
                }
            }

            animator.SetFloat("BattleMoveHorizontal", character.moveParryDodgeVec.x);
            animator.SetFloat("BattleMoveVertical", character.moveParryDodgeVec.z);

            //dodgeDuration 이후에 피하기 상태 해제
            if (character.parryDodgeTimer >= character.curAimTime - Time.deltaTime)
            {
                character.GetAnimator().SetBool("MoveDodge", false);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.moveParryDodgeVec = Vector3.zero;
        }
    }
}
