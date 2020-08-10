/*
 * Class: CharacterControl
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
    public class CharacterControl : MonoBehaviour
    {
        public float speed;
        private Animator animator;

        //스테이트들
        public Vector3 velocity = Vector3.zero;

        public Animator GetAnimator()
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
            return animator;
        }

    }
}
