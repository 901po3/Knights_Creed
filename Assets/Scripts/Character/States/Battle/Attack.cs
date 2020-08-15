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
            
            switch(Random.Range(0, 3))
            {
                case 0:
                    character.medAttackType = MED_ATTACK_TYPE.HIGH;
                    character.curAttackTime = 1.783f;
                    break;
                case 1:
                    character.medAttackType = MED_ATTACK_TYPE.MIDDLE;
                    character.curAttackTime = 1.5f;
                    break;
                case 2:
                    character.medAttackType = MED_ATTACK_TYPE.LOW;
                    character.curAttackTime = 1.9f;
                    break;
            }
            if(character.prevMedAttackType == character.medAttackType)
            {
                character.medAttackType = (character.medAttackType + 1);
                if ((int)character.medAttackType == 3)
                    character.medAttackType = 0;
            }
            animator.SetFloat("RandomAttack", (float)character.medAttackType);
            character.prevMedAttackType = character.medAttackType;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            character.attackTimer += Time.deltaTime;
            if (character.isAttacking)
            {
                animator.SetBool("SwingSword", true);

                if(character.attackTimer > attackEnableTime) //무기 공격 활성화
                {
                    character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().enabled = true;
                }            
            }
            
            if(character.attackTimer > character.curAttackTime - Time.deltaTime)
            {
                animator.SetBool("SwingSword", false);
            }           
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
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