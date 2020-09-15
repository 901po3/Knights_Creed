/*
 * Class: Death
 * Date: 2020.8.16
 * Last Modified : 2020.8.17
 * Author: Hyukin Kwon 
 * Description:  피해 애니메이션
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Death")]
    public class Death : StateData
    {
        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            switch (character.attacker.medAttackType)
            {
                case MED_ATTACK_TYPE.HIGH:
                    animator.SetFloat("RandomHit", 0);
                    character.curAimTime = 2.66f;
                    break;
                case MED_ATTACK_TYPE.MIDDLE:
                    animator.SetFloat("RandomHit", 1);
                    character.curAimTime = 2.56f;
                    break;
                case MED_ATTACK_TYPE.LOW:
                    animator.SetFloat("RandomHit", 2);
                    character.curAimTime = 3.6f;
                    break;
            }
            character.gameObject.layer = 15;
            character.invincible = true;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            //애니메이션이 끝나면 캐릭터 비활성화
            character.deathTimer += Time.deltaTime;
            if(character.deathTimer > character.curAimTime - Time.deltaTime)
            {
                if(character.attacker != null && character.attacker.currentState != CURRENT_STATE.DEAD)
                {
                    character.attacker.targetEnemy = null;
                }
                animator.enabled = false;
                character.GetRigidbody().isKinematic = true;
                character.GetComponent<Collider>().isTrigger = true;
                //character.gameObject.SetActive(false);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            animator.enabled = false;
            character.GetRigidbody().isKinematic = true;
            character.GetComponent<Collider>().isTrigger = true;
        }
    }

}