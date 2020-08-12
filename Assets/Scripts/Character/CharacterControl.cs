/*
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
    public class CharacterControl : MonoBehaviour
    {
        public List<GameObject> undrawedWeapon;
        public List<GameObject> drawedWeapon;
        public Equipment.WEAPON weapon;

        private Animator animator;
        private Rigidbody rigidbody;

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
        public float curUndetectedTime; //undetectedTime 타이머
        public bool isAttacking = false;

        private void Awake()
        {
            //맞는 무기를 장비한다.
            Equipment.Clear(this);
            Equipment.ToogleWeapon(this);
        }

        public Animator GetAnimator()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            return animator;
        }

        public Rigidbody GetRigidbody()
        {
            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody>();
            }
            return rigidbody;
        }

    }
}
