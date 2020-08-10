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
    public class PlayerInput : MonoBehaviour
    {
        private CharacterControl character;

        private void Awake()
        {
            character = GetComponent<CharacterControl>();
        }

        private void Update()
        {
            MoveVerticalInput();
        }

        private void MoveVerticalInput()
        {
            character.velocity.z = Input.GetAxis("Vertical");

        }
    }

}