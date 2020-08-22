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
            if(character.attacker != null)
            {
                switch (character.attacker.medAttackType)
                {
                    case MED_ATTACK_TYPE.HIGH:
                        animator.SetFloat("RandomHit", 0);
                        character.curAimTime = 1.45f;
                        break;
                    case MED_ATTACK_TYPE.MIDDLE:
                        animator.SetFloat("RandomHit", 1);
                        character.curAimTime = 1f;
                        break;
                    case MED_ATTACK_TYPE.LOW:
                        animator.SetFloat("RandomHit", 2);
                        character.curAimTime = 1.1f;
                        break;
                    case MED_ATTACK_TYPE.COMBO:
                        animator.SetFloat("RandomHit", 3);
                        character.curAimTime = 3.1f;
                        break;
                }

                Transform attackerTrans = character.attacker.transform;
                attackerTrans.position = new Vector3(character.attacker.transform.position.x,
                    character.transform.position.y, character.attacker.transform.position.z);
                character.transform.LookAt(attackerTrans);
            }
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            character.hurtTimer += Time.deltaTime;
            if (character.hurtTimer >= character.curAimTime && character.currentState != CURRENT_STATE.DEAD) //애니메이션 시간이 끝나면 나간다
            {
                character.currentState = CURRENT_STATE.NONE;
                animator.SetBool("Hurt", false);
                character.isBattleModeOn = true;

                //이미 맞았으므로 피하기 취소
                animator.SetBool("Dodge", false);
            }

            //회전 //나중에 다른 State로 분리
            if(character.attacker != null)
            {
                Vector3 targetDirection = (character.attacker.transform.position - character.transform.position).normalized;
                targetDirection.y = 0f;
                character.transform.rotation = Quaternion.RotateTowards(character.transform.rotation, Quaternion.LookRotation(targetDirection), Time.fixedDeltaTime);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }
    }

}