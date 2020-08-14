﻿/*
 * Class: CharacterControl
 * Date: 2020.8.10
 * Last Modified : 2020.8.11
 * Author: Hyukin Kwon 
 * Description: 캐릭터(플레이어, AI)의 상태 클래스 
 *              플레이어면 인풋 클래스와 함께 사용
 *              적이면 AI 클래스와 함께 사용
*/
using System.Collections.Generic;
using UnityEngine;

namespace HyukinKwon
{
    public enum TEAM
    {
        BLUE, YELLOW, BLACK
    }

    public class CharacterControl : MonoBehaviour
    {
        public TEAM team;
        public int health;
        public int damage;

        public List<GameObject> undrawedWeapon;
        public List<GameObject> drawedWeapon;
        public Equipment.WEAPON weapon;

        private Animator mAnimator;
        private Rigidbody mRigidbody;

        //레그돌
        private Collider[] colliders;
        private Rigidbody[] rigidbodies;
        private Collider mCollider;

        //이동 방향의 기준
        //플레이어면 카매라 - 카매라 정면이 기준
        public Transform facingStandardTransfom;

        //상태 변수들 (애니메이션 전환에 쓰인다)
        public Vector3 runVelocity = Vector3.zero;
        public bool isTurning = false;
        public bool isBattleModeOne = false;  //전투모드 
        public bool isDrawingWeapon = false;  //무기 들고 있는지 아닌지

        public bool isDetected = false; //어그로 체크
        public float undetectedTime; //undetectedTime초 이후에 전투모드 Off
        public float curUndetectedTime = 0; //undetectedTime 타이머

        public bool isAttacking = false;
        public float attackTimer; //공격 타이머
        public bool isDodging = false;
        public float dodgeTimer = 0f; //파하기 타이머
        public bool isHurt = false;
        public float hurtTimer = 0;

        public bool isChangingMode = false;

        private void Awake()
        {
            //맞는 무기를 장비한다.
            Equipment.Clear(this);
            Equipment.ToogleWeapon(this);
            drawedWeapon[(int)weapon].GetComponent<BoxCollider>().enabled = false;
            colliders = GetComponentsInChildren<Collider>();
            rigidbodies = GetComponentsInChildren<Rigidbody>();

            mCollider = GetComponent<Collider>();
            mRigidbody = GetComponent<Rigidbody>();
            mAnimator = GetComponent<Animator>();

            ToggleRagdoll(false);
        }

        public Animator GetAnimator()
        {
            return mAnimator;
        } 

        public Rigidbody GetRigidbody()
        {
            return mRigidbody;
        }

        private void OnCollisionEnter(Collision collision)
        {
            CheckHurt(collision);
        }

        private void CheckHurt(Collision collision)
        {
            //다치는 상태가 아니고
            //적이 공격중이고
            //다른팀이면 피격

            /*레그돌
             * 애니에미터 끄고, m콜라이더 끄고, navmesh있으면 끈다.
             */
            if (collision.transform.tag == "Weapon" && !isHurt)
            {
                CharacterControl character = collision.gameObject.GetComponentInParent<CharacterControl>();
                if (character.isAttacking && character.team != team)
                {
                    isHurt = true;                  
                    //GetComponent<Animator>().SetBool("HurtRight", true);
                    GetDamaged(character.damage);

                    //공격 들어온 방향에 맞게 힘 적용
                    Vector3 dir = collision.contacts[0].point - collision.gameObject.transform.position;
                    mRigidbody.AddForce(dir * 100);

                    ToggleRagdoll(true);
                }
            }
        }

        private void ToggleRagdoll(bool b) //if b == true 레그돌 활성
        {
            foreach (Collider c in colliders)
            {
                c.enabled = b;
            }
            foreach (Rigidbody r in rigidbodies)
            {
                r.isKinematic = !b;
            }

            mAnimator.enabled = !b;
            mCollider.enabled = !b;
            mRigidbody.isKinematic = b;
        }

        public void GetDamaged(int damage)
        {
            health -= damage;
            Debug.Log(health);
        }
    }
}
