/*
 * Class: Attack
 * Date: 2020.8.12
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: 공격 
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Attack")]
    public class Attack : StateData
    {
        public int damage;
        public float attackEnableTime;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.damage = damage;
            
            //공격 타입에 맞게 시간 설정 
            switch(character.medAttackType)
            {
                case MED_ATTACK_TYPE.HIGH:
                    character.curAimTime = 1.783f;
                    character.attackEndTime = 1f;
                    break;
                case MED_ATTACK_TYPE.MIDDLE:
                    character.curAimTime = 1.5f;
                    character.attackEndTime = 0.5f;
                    break;
                case MED_ATTACK_TYPE.LOW:
                    character.curAimTime = 1.9f;
                    character.attackEndTime = 1f;
                    break;
            }
            animator.SetFloat("RandomAttack", (float)character.medAttackType);
            character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().isTrigger = false;

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
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.GetRigidbody().velocity = new Vector3(0, character.GetRigidbody().velocity.y, 0);

            if(character.attackChargeDes != new Vector3(0, 10000, 0))
            {
                character.transform.position = Vector3.Slerp(character.transform.position, character.attackChargeDes, 1f * Time.fixedDeltaTime);
            }

            character.attackTimer += Time.deltaTime;
            if (character.isAttacking)
            {
                animator.SetBool("Attack", true);

                if(character.attackTimer > attackEnableTime) //무기 공격 활성화
                {
                    character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().enabled = true;
                }
                if(character.attackTimer >= character.attackEndTime)
                {
                    character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().enabled = false;
                    character.isAttacking = false;
                }
            }
            
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
            character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().enabled = false;
        }



        //private void OldAtttackFunc(CharacterState characterState, Animator animator)
        //{
        //    CharacterControl character = characterState.GetCharacterControl(animator);
        //    if (character.isAttacking)
        //    {
        //        animator.SetBool("SwingSword", true);
        //        if (character.attackTimer < duration)
        //        {
        //            character.attackTimer += Time.deltaTime;
        //            //회전
        //            Vector3 targetDirection = character.runVelocity.normalized;
        //            targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
        //            targetDirection.y = 0f;
        //            character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(character.transform.forward,
        //                targetDirection, turnSpeed * Time.fixedDeltaTime, 0f)));
        //        }
        //    }
        //    else
        //    {
        //        animator.SetBool("SwingSword", false);
        //    }
        //}

    }

}