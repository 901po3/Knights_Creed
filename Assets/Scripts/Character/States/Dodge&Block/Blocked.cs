/*
 * Class: Blocked
 * Date: 2020.8.18
 * Last Modified : 2020.8.18
 * Author: Hyukin Kwon 
 * Description: 공격막힘 애니메이션 조정
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Blocked")]
    public class Blocked : StateData
    {
        public float speed;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.curUndetectedTimer = 0; //피하기 시도하면-> 전투 해제 시간 리셋

            // 종류 선택
            switch (character.medAttackType)
            {
                case MED_ATTACK_TYPE.HIGH:
                    character.curAimTime = 1.3f;
                    animator.SetFloat("RandomHit", 0);
                    break;
                case MED_ATTACK_TYPE.MIDDLE:
                    character.curAimTime = 1.8f;
                    animator.SetFloat("RandomHit", 1);
                    break;
                case MED_ATTACK_TYPE.LOW:
                    character.curAimTime = 1.8f;
                    animator.SetFloat("RandomHit", 2);
                    break;
            }
            //공격에서 넘어왔을때를 대비 무기 콜라이더 비활성
            character.drawedWeapon[(int)character.weapon].GetComponent<WeaponScript>().ToggleCollision(false);

            Transform attackerTrans = character.targetEnemy.transform;
            attackerTrans.position = new Vector3(attackerTrans.transform.position.x,
                character.transform.position.y, attackerTrans.transform.position.z);
            character.transform.LookAt(attackerTrans);

            character.curAnimSpeed = speed;
            character.blockedTimer = 0f;
            character.isAttacking = false;
            character.attacker.isComboAttacking = true;
            animator.SetBool("Attack", false);
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);


            //자연스러운 애니메이션을 위한 속도 조정
            if (character.curAnimSpeed > 0)
            {
                character.curAnimSpeed -= (speed * 4) * Time.deltaTime;
            }

            //뒤로 이동
            character.GetRigidbody().MovePosition(character.transform.position - character.transform.forward * character.curAnimSpeed * Time.fixedDeltaTime);

            //dodgeDuration 이후에 피하기 상태 해제
            character.blockedTimer += Time.deltaTime;
            if (character.blockedTimer >= character.parryDodgeEndTime)
            {
                if (character.blockedTimer >= character.curAimTime - Time.deltaTime)
                {                                  
                    character.isBlocked = false;
                    character.GetAnimator().SetBool("Blocked", false);
                }
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
;
        }
    }

}