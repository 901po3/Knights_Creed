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
        public float range;
        public float turnSpeed;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.damage = damage;
            character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().enabled = false;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            if (character.isAttacking)
            {
                animator.SetBool("SwingSword", true);
                if(character.attackTimer < duration)
                {
                    character.attackTimer += Time.deltaTime;

                    if (character.attackTimer >= 0.2f)
                    {
                        character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().enabled = true;
                    }

                    //회전
                    Vector3 targetDirection = character.runVelocity.normalized;
                    targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
                    targetDirection.y = 0f;
                    character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(character.transform.forward,
                        targetDirection, turnSpeed * Time.fixedDeltaTime, 0f)));
                }
            }
            else
            {
                animator.SetBool("SwingSword", false);
            }          
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.attackTimer = 0;
        }
    }

}