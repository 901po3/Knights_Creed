/*
 * Class: MoveVertical
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
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/MoveVertical")]
    public class MoveVertical : StateData
    {
        public float forwardSpeed;
        public float backwardSpeed;
        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            float vz = character.velocity.z;
            animator.SetFloat("RunningVeritical", vz);
            if(vz <= 0.1f && vz >= -0.1f)
            {
                return;
            }
            else if(vz > 0.1f)
            {
                character.transform.Translate(character.velocity * forwardSpeed * character.speed * Time.deltaTime);
            }
            else if(vz < -0.1f)
            {
                character.transform.Translate(character.velocity * backwardSpeed * character.speed * Time.deltaTime);
            }
        }
    }
}
