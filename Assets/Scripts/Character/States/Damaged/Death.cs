﻿/*
 * Class: Death
 * Date: 2020.8.16
 * Last Modified : 2020.8.16
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
            switch (character.attackerList[0].medAttackType)
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

            foreach(CharacterControl c in character.attackerList)
            {
                if(c.targetEnemy == character.gameObject)
                {
                    c.targetEnemy = null; //자신을 타겟으로 갖고 있는 모든 적을 null로 해야함 
                }
            }
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.deathTimer += Time.deltaTime;
            if(character.deathTimer >= character.curAimTime - Time.deltaTime)
            {
                animator.enabled = false;
                character.GetRigidbody().isKinematic = true;
                character.GetComponent<Collider>().isTrigger = true;
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            character.attackerList[0].targetEnemy = null;
        }
    }

}