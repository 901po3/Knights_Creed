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

            //자신이 공격했던 적중에 자신을 타겟으로 하는 적의 타겟을 초기화
            foreach(CharacterControl c in character.attackList)
            {
                if(c.targetEnemy == character.gameObject)
                {
                    if (c.attacker == character)
                        c.attacker = null;
                    c.targetEnemy = null;
                }
            }

            character.gameObject.layer = 15;
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            //애니메이션이 끝나면 캐릭터 비활성화
            character.deathTimer += Time.deltaTime;
            if(character.deathTimer >= character.curAimTime - Time.deltaTime)
            {
                character.isDodging = false;
                character.isAttacking = false;
                character.isParrying = false;
                character.attacker.targetEnemy = null;
                animator.enabled = false;
                character.GetRigidbody().isKinematic = true;
                character.GetComponent<Collider>().isTrigger = true;
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }
    }

}