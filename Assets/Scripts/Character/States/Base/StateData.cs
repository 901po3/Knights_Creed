/*
 * Class: StateData
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
    public abstract class StateData : ScriptableObject
    {
        public float duration;

        public abstract void UpdateAbility(CharacterState characterState, Animator animator);
    }

}