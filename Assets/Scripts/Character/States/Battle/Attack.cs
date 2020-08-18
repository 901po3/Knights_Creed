/*
 * Class: Attack
 * Date: 2020.8.12
 * Last Modified : 2020.8.19
 * Author: Hyukin Kwon 
 * Description: 공격 
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Attack")]
    public class Attack : StateData
    {
        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            
            //공격 타입에 맞게 시간 설정 
            animator.SetFloat("RandomAttack", (float)character.medAttackType);
            animator.SetBool("Parry", false);

            //공격 시도시 타겟과의 거리를 계산에둔다
            character.attackChargeDes = new Vector3(0, 10000, 0);
            if (character.targetEnemy != null)
            {
                float dis = Vector3.Distance(character.transform.position, character.targetEnemy.transform.position);
                if (dis < character.chargeDis && dis > character.attackRange)
                {
                    Vector3 dir = (character.targetEnemy.transform.position - character.transform.position).normalized;
                    character.attackChargeDes = character.transform.position + dir * (dis - character.attackRange - 0.2f);
                }
            }
            character.drawedWeapon[(int)character.weapon].GetComponent<WeaponScript>().damageOnce = false;
            character.attackTimer = 0;
                     
            if(character.currentState == CURRENT_STATE.COMBO_ATTACK)
            {
                character.wasComboAttack = true;
            }
            character.cancelAttackAvailable = true; 
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            WeaponScript weapon = character.drawedWeapon[(int)character.weapon].GetComponent<WeaponScript>();
            character.GetRigidbody().velocity = Vector3.zero;

            //적의 앞으로 이동하며 공격
            if(character.attackChargeDes != new Vector3(0, 10000, 0))
            {
                character.transform.position = Vector3.Slerp(character.transform.position, character.attackChargeDes, 1f * Time.fixedDeltaTime);
            }

            character.attackTimer += Time.deltaTime;
            if (character.currentState == CURRENT_STATE.ATTACK || character.currentState == CURRENT_STATE.COMBO_ATTACK)
            {
                if(character.attackTimer > character.attackEnableTime) //공격 포인트에 돌입시 무기 공격 활성화
                {
                    if (!weapon.damageOnce)
                    {
                        weapon.ToggleCollision(true);
                    }
                    else
                    {
                        weapon.ToggleCollision(false);
                    }
                }
                if(character.attackTimer >= character.attackEndTime) //공격 포인트를 지나면 무기 공격 비활성화
                {
                    weapon.ToggleCollision(false);                    
                }
            }          

            if(character.currentState != CURRENT_STATE.ATTACK && character.currentState != CURRENT_STATE.COMBO_ATTACK)
            {
                if(character.currentState == CURRENT_STATE.DODGE)
                {
                    animator.SetBool("Dodge", true);
                }
                else if(character.currentState == CURRENT_STATE.PARRY)
                {
                    animator.SetBool("Parry", true);
                }
                else if(character.currentState == CURRENT_STATE.BLOCKED)
                {
                    animator.SetBool("Blocked", true);
                }
                animator.SetBool("Attack", false);
            }

            //애니메이션 시간 끝
            if (character.attackTimer > character.curAimTime - Time.deltaTime)
            {
                animator.SetBool("Attack", false);
                if (character.currentState == CURRENT_STATE.ATTACK || character.currentState == CURRENT_STATE.COMBO_ATTACK)
                {
                    character.currentState = CURRENT_STATE.NONE;
                }
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            if (character.wasComboAttack)
            {
                character.wasComboAttack = false;
                character.PickFirstNextAttack();
            }
            else
            {
                character.PickNextAttack(false);
            }
        }
    }
}