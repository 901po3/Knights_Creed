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

        private void OnTriggerEnter(Collider other)
        {
            CheckHurt(other);
        }

        private void CheckHurt(Collider other)
        {
            //다치는 상태가 아니고
            //적이 공격중이고
            //다른팀이면 피격
            if (other.transform.tag == "Weapon" && !isHurt)
            {
                CharacterControl character = other.GetComponentInParent<CharacterControl>();
                if (character.isAttacking && character.team != team)
                {
                    character.drawedWeapon[(int)character.weapon].GetComponent<BoxCollider>().enabled = false;
                    isHurt = true;
                    GetComponent<Animator>().SetBool("HurtRight", true);
                    GetDamaged(character.damage);
                }
            }
        }

        public void GetDamaged(int damage)
        {
            health -= damage;
            Debug.Log(health);
        }
    }
}
