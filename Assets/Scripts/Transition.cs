/*
 * Class: Transition
 * Date: 2020.8.13
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: 애니메이션 노드 이동 
*/
using System.Collections.Generic;
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Transition")]
    public class Transition : StateData
    {
        public enum TRANSITION_TYPE
        {
            TO_BATTLE,
            TO_NORMAL,
            ATTACK,
            DODGE
        }

        public int index;
        public List<TRANSITION_TYPE> transition = new List<TRANSITION_TYPE>();

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl charControl = characterState.GetCharacterControl(animator);
            if (MakeTransition(charControl))
            {
                animator.SetInteger("TransitionIndex", index);
            }
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl charControl = characterState.GetCharacterControl(animator);
            if (MakeTransition(charControl))
            {
                animator.SetInteger("TransitionIndex", index);
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
            animator.SetInteger("TransitionIndex", 0);
        }

        private bool MakeTransition(CharacterControl character)
        {
            foreach (TRANSITION_TYPE t in transition)
            {
                switch (t)
                {
                    case TRANSITION_TYPE.TO_BATTLE:
                        if (character.isBattleModeOne)
                        {
                            return false;
                        }
                        break;
                    case TRANSITION_TYPE.TO_NORMAL:
                        if (!character.isBattleModeOne)
                        {
                            return false;
                        }
                        break;
                    case TRANSITION_TYPE.ATTACK:
                        if (!character.isAttacking)
                        {
                            return false;
                        }
                        break;
                    case TRANSITION_TYPE.DODGE:
                        if (!character.isDodging)
                        {
                            return false;
                        }
                        break;
                }
            }
            return true;
        }
    }
}

