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
            character.dodgeTimer = 0;
            character.isDodging = true;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            //시간 업데이트
            character.dodgeTimer += Time.deltaTime;

            //지정된 방향 기준을 중심으로 이동
            Vector3 curRunVelocity = character.runVelocity;
            Vector3 forward = character.transform.forward;
            Vector3 right = character.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 dir = forward * curRunVelocity.z + right * curRunVelocity.x;

            if (character.dodgeTimer < character.curAimTime - 0.2f)
            {
                if(character.dodgeTimer > 0.3f)
                {
                    character.isDodging = false;
                }
                character.GetRigidbody().MovePosition(character.transform.position + dir * speed * Time.fixedDeltaTime);
            }

            animator.SetFloat("BattleMoveHorizontal", curRunVelocity.x);
            animator.SetFloat("BattleMoveVertical", curRunVelocity.z);

            //dodgeDuration 이후에 피하기 상태 해제
            if (character.dodgeTimer >= character.curAimTime - Time.deltaTime)
            {
                character.isDodging = false;
                character.GetAnimator().SetBool("MoveDodge", false);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }
    }
}
