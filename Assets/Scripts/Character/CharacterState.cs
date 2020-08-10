/*
 * Class: CharacterState
 * Date: 2020.8.10
 * Last Modified : 2020.8.10
 * Author: Hyukin Kwon 
 * Description:  
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyukinKwon
{
    public class CharacterState : StateMachineBehaviour
    {
        public List<StateData> abilityDataList = new List<StateData>();

        public void UpdateAll(CharacterState characterState, Animator animator)
        {
            foreach(StateData data in abilityDataList)
            {
                data.UpdateAbility(characterState, animator);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            UpdateAll(this, animator);
        }

        private CharacterControl characterControl;
        public CharacterControl GetCharacterControl(Animator animator)
        {
            if(characterControl == null)
            {
                characterControl = animator.GetComponentInParent<CharacterControl>();
            }
            return characterControl;
        }
    }
}