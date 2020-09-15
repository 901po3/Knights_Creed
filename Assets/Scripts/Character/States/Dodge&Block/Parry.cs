/*
 * Class: Parry
 * Date: 2020.8.18
 * Last Modified : 2020.8.18
 * Author: Hyukin Kwon 
 * Description: 막기 애니메이션 조정
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Parry")]
    public class Parry : StateData
    {
        public float speed;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.curUndetectedTimer = 0; //피하기 시도하면-> 전투 해제 시간 리셋

            //피하기 종류 선택
            if(character.targetEnemy != null)
            {
                switch (character.targetEnemy.GetComponent<CharacterControl>().medAttackType)
                {
                    case MED_ATTACK_TYPE.HIGH:
                        character.curAimTime = 1.2f;
                        character.parryDodgeEndTime = 1f;
                        animator.SetFloat("RandomHit", 0);
                        break;
                    case MED_ATTACK_TYPE.MIDDLE:
                        character.curAimTime = 0.9f;
                        character.parryDodgeEndTime = 0.8f;
                        animator.SetFloat("RandomHit", 1);
                        break;
                    case MED_ATTACK_TYPE.LOW:
                        character.curAimTime = 1.2f;
                        character.parryDodgeEndTime = 0.7f;
                        animator.SetFloat("RandomHit", 2);
                        break;
                }
            }
            else
            {
                //가장 가까운 적을 targetEnemy로 설정
            }
           
            //공격에서 넘어왔을때를 대비 무기 콜라이더 비활성
            character.drawedWeapon[(int)character.weapon].GetComponent<WeaponScript>().ToggleCollision(false);

            Transform attackerTrans = character.targetEnemy.transform;
            attackerTrans.position = new Vector3(attackerTrans.transform.position.x,
                character.transform.position.y, attackerTrans.transform.position.z);
            character.transform.LookAt(attackerTrans);

            character.invincible = true;
            character.parryDodgeTimer = 0;
            character.curAnimSpeed = speed;

            //무기의 막기 전용 콜리션 킨다
            character.drawedWeapon[(int)character.weapon].GetComponent<CapsuleCollider>().isTrigger = false;
            character.drawedWeapon[(int)character.weapon].GetComponent<CapsuleCollider>().enabled = true;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            WeaponScript weapon = character.drawedWeapon[(int)character.weapon].GetComponent<WeaponScript>();

            //자연스러운 애니메이션을 위한 속도 조정
            if (character.curAnimSpeed > 0)
            {
                character.curAnimSpeed -= (speed * 4) * Time.deltaTime;
            }

            //뒤로 이동
            character.GetRigidbody().MovePosition(character.transform.position - character.transform.forward * character.curAnimSpeed * Time.fixedDeltaTime);

            //parryDodgeTimer 이후에 피하기 상태 해제
            character.parryDodgeTimer += Time.deltaTime;
            if (character.parryDodgeTimer >= character.parryDodgeEndTime)
            {
                weapon.GetComponent<CapsuleCollider>().isTrigger = true;
                weapon.GetComponent<CapsuleCollider>().enabled = false;
                character.invincible = false;

                if (character.parryDodgeTimer >= character.curAimTime - Time.deltaTime)
                {
                    character.currentState = CURRENT_STATE.NONE;
                    character.GetAnimator().SetBool("Parry", false);
                    weapon.parryOnce = false;
                }
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            WeaponScript weapon = character.drawedWeapon[(int)character.weapon].GetComponent<WeaponScript>();
            weapon.GetComponent<CapsuleCollider>().isTrigger = true;
            weapon.GetComponent<CapsuleCollider>().enabled = false;
            character.invincible = false;
        }
    }

}