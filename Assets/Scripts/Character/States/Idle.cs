/*
 * Class: Idle
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
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Idle")]
    public class Idle : StateData
    {
        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            float vz = character.velocity.z;
            if (vz > 0.1f || vz < -0.1f)
            {
                animator.SetFloat("RunningVeritical", vz);
            }
        } 
    }

}