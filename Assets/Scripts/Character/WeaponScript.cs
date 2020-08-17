﻿/*
 * Class: WeaponScript
 * Date: 2020.8.16
 * Last Modified : 2020.8.16
 * Author: Hyukin Kwon 
 * Description:  무기에 들어가는 스크립트
 *             
*/
using UnityEngine;

namespace HyukinKwon
{
    public class WeaponScript : MonoBehaviour
    {
        CharacterControl owner;

        private void Start()
        {
            owner = GetComponentInParent<CharacterControl>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.GetComponent<CharacterControl>() != null)
            {
                if(collision.gameObject.GetComponent<CharacterControl>().team != owner.team)
                {
                    if(owner.targetEnemy == null || owner.targetEnemy != collision.gameObject)
                    {
                        owner.targetEnemy = collision.gameObject;
                        owner.isTargetChanged = true;
                    }

                    //공격한 적
                    bool isAlreadyAdded = false;
                    foreach(CharacterControl c in owner.gameObject.GetComponent<CharacterControl>().attackList)
                    {
                        if(c == collision.gameObject.GetComponent<CharacterControl>())
                        {
                            isAlreadyAdded = true;
                            break;
                        }
                    }
                    if(!isAlreadyAdded)
                    {
                        owner.gameObject.GetComponent<CharacterControl>().attackList.Add(collision.gameObject.GetComponent<CharacterControl>());
                    }

                    GetComponent<Collider>().enabled = false;
                    collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero; //충돌 반동 초기화
                }
            }
        }
    }
}

