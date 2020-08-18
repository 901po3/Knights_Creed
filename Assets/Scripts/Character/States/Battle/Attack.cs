/*
 * Class: Attack
 * Date: 2020.8.12
 * Last Modified : 2020.8.17
 * Author: Hyukin Kwon 
 * Description: 공격 
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Attack")]
    public class Attack : StateData
    {
        public float attackEnableTime;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            
            //공격 타입에 맞게 시간 설정 
            switch(character.medAttackType)
            {
                case MED_ATTACK_TYPE.HIGH:
                    character.curAimTime = 1.783f;
                    character.attackEndTime = 1f;
                    break;
                case MED_ATTACK_TYPE.MIDDLE:
                    character.curAimTime = 1.5f;
                    character.attackEndTime = 0.75f;
                    break;
                case MED_ATTACK_TYPE.LOW:
                    character.curAimTime = 1.9f;
                    character.attackEndTime = 1f;
                    break;
            }
            animator.SetFloat("RandomAttack", (float)character.medAttackType);


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

            character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().isTrigger = false; 
            character.drawedWeapon[(int)character.weapon].GetComponent<WeaponScript>().damageOnce = false;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.GetRigidbody().velocity = new Vector3(0, character.GetRigidbody().velocity.y, 0);

            //적의 앞으로 이동하며 공격
            if(character.attackChargeDes != new Vector3(0, 10000, 0))
            {
                character.transform.position = Vector3.Slerp(character.transform.position, character.attackChargeDes, 1f * Time.fixedDeltaTime);
            }

            character.attackTimer += Time.deltaTime;
            if (character.isAttacking)
            {
                animator.SetBool("Attack", true);

                if(character.attackTimer > attackEnableTime) //공격 포인트에 돌입시 무기 공격 활성화
                {
                    character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().enabled = true;
                }
                if(character.attackTimer >= character.attackEndTime) //공격 포인트를 지나면 무기 공격 비활성화
                {
                    character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().enabled = false;
                    character.isAttacking = false;
                }
            }
            
            //애니메이션 끝
            if(character.attackTimer > character.curAimTime - Time.deltaTime)
            {
                animator.SetBool("Attack", false);
            }           
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.PickNextAttack(); //미리 다음 공격 타입을 정한다
            character.attackTimer = 0;
            character.isAttacking = false;
        }
    }
}