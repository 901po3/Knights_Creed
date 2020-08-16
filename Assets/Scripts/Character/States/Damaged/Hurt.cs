/*
 * Class: Hurt
 * Date: 2020.8.13
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description:  피해 애니메이션
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Hurt")]
    public class Hurt : StateData
    {
        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            switch(character.attacker.medAttackType)
            {
                case MED_ATTACK_TYPE.HIGH:
                    animator.SetFloat("RandomHit", 0);
                    break;
                case MED_ATTACK_TYPE.MIDDLE:
                    animator.SetFloat("RandomHit", 1);
                    break;
                case MED_ATTACK_TYPE.LOW:
                    animator.SetFloat("RandomHit", 2);
                    break;
            }
            Transform attackerTrans = character.attacker.transform;
            attackerTrans.position = new Vector3(character.attacker.transform.position.x, 
                character.transform.position.y, character.attacker.transform.position.z);
            character.transform.LookAt(attackerTrans);

            if(!character.isDodging)
            {
                character.GetDamaged(character.attacker.damage);
            }
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            if(character.health > 0)
            {
                if (character.hurtTimer < duration)
                {
                    character.hurtTimer += Time.deltaTime;
                    if (character.hurtTimer >= duration)
                    {
                        character.hurtTimer = 0f;
                        character.isHurt = false;
                        animator.SetBool("Hurt", false);
                        character.isBattleModeOne = true;
                    }
                }
            }
            else
            {
                character.hurtTimer = 0f;
                character.isHurt = false;
                animator.SetBool("Dead", true);
            }

            //회전 //나중에 다른 State로 분리
            Vector3 targetDirection = (character.attacker.transform.position - character.transform.position).normalized;
            targetDirection.y = 0f;

            character.transform.rotation = Quaternion.RotateTowards(character.transform.rotation, Quaternion.LookRotation(targetDirection), Time.fixedDeltaTime);
            //character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards
            //   (character.transform.forward, targetDirection,  0.1f  * Time.fixedDeltaTime, 0f)));
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }
    }

}