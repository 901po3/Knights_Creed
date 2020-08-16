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

    public enum NORMAL_IDLE_TYPE
    {
        IDLE = 0, RELAXED_IDLE, GUARD_IDLE_1, GUARD_IDLE_2
    }

    public enum BATTLE_IDLE_TYPE
    {
        SWORD_IDLE_1, SWORD_IDLE_2, SWORD_IDLE_3
    }

    public enum MED_ATTACK_TYPE
    {
        HIGH, MIDDLE, LOW
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

        public float curAimTime = 0; //현재 사용하는 애니메이션의 길이
        public GameObject targetEnemy; //타겟팅 된 적

        //레그돌
        private Collider[] colliders;
        private Rigidbody[] rigidbodies;
        private Collider mCollider;

        //이동 방향의 기준
        //플레이어면 카매라 - 카매라 정면이 기준
        public Transform facingStandardTransfom;

        //상태 변수들 (애니메이션 전환에 쓰인다)
        public Vector3 prevRunVelocity = Vector3.zero;// velocity의 변화값으로 어느 방향으로 회전 중인지 체크
        public Vector3 runVelocity = Vector3.zero;
        public float horizontalV = 0;

        //서있기 관련
        public NORMAL_IDLE_TYPE NormalIdleType;
        public BATTLE_IDLE_TYPE BattleIdleType;

        //모드 바꾸기 관련
        public bool isBattleModeOne = false;  //전투모드 
        public bool isDrawingWeapon = false;  //무기 들고 있는지 아닌지
        public bool isChangingMode = false;

        //어그로 관련
        public bool isDetected = false; //어그로 체크
        public float undetectedTime; //undetectedTime초 이후에 전투모드 Off

        //공격 관련
        public bool isAttacking = false;
        public MED_ATTACK_TYPE medAttackType;
        public MED_ATTACK_TYPE prevMedAttackType;

        //피하기 관련
        public bool isDodging = false;

        //공격 받음 관련
        public bool isHurt = false;
        public CharacterControl attacker; //자신을 때린 적
        private Vector3 contactPoint = new Vector3(0, 10000, 0);
        private Vector3 contactDir = Vector3.zero;
        public GameObject bloodEffect;
        private List<GameObject> blooodEffectList;
        private int maxBloodNum = 3;
        private int curBloodIndex = 0;

        //회전 관련
        public bool isTurning = false; //회전중
        public bool turning = false; //180도 급회전 변수

        //타이머 변수들
        public float turnTimer = 0; //180도 급회전 변수 타이머
        public float startTurnTimer = 0;
        public float hurtTimer = 0;
        public float deathTimer = 0;
        public float curUndetectedTimer = 0; //undetectedTime 타이머
        public float attackTimer; //공격 타이머
        public float dodgeTimer = 0f; //파하기 타이머

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

            //파티클 이펙트
            blooodEffectList = new List<GameObject>();
            for (int i = 0; i < maxBloodNum; i++) //피 이펙트 3개
            {
                GameObject blood = Instantiate(bloodEffect);
                blood.transform.parent = transform;
                blooodEffectList.Add(blood);
                blood.SetActive(false);
            }
            bloodEffect.SetActive(false);

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

        private void OnTriggerStay(Collider other)
        {

        }

        //피해 적용
        private void CheckHurt(Collision collision)
        {
            if (collision.transform.tag == "Weapon")
            {
                CharacterControl atk = collision.gameObject.GetComponentInParent<CharacterControl>();
                if (atk.team != team)
                {
                    //공격한 무기의 트리거를 끄고 충돌시 발생하는 힘을 제거한다
                    collision.gameObject.GetComponent<Collider>().isTrigger = true; 
                    mRigidbody.velocity = new Vector3(0, mRigidbody.velocity.y, 0);
                    atk.GetRigidbody().velocity = new Vector3(0, atk.GetRigidbody().velocity.y, 0);

                    attacker = atk;
                    isHurt = true;
                    contactDir = collision.contacts[0].point - collision.gameObject.transform.position;
                    contactPoint = collision.contacts[0].point;

                    mAnimator.SetBool("Hurt", true);
                    mAnimator.SetBool("Dead", false);
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

        public void PickTargetEnemy()
        { 

        }

        public void GetDamaged(int damage)
        {
            health -= damage;
            targetEnemy = attacker.gameObject;

            //피 생성
            foreach (GameObject b in blooodEffectList)
            {
                if (b.GetComponent<ParticleSystem>().time >= 0.3f - Time.deltaTime)
                {
                    b.SetActive(false);
                }
            }
            if(contactPoint != new Vector3(0, 10000, 0))
            {
                GameObject blood = blooodEffectList[curBloodIndex];
                if (!blood.activeSelf)
                {
                    curBloodIndex = (curBloodIndex + 1) % maxBloodNum;
                    blood.transform.rotation = Quaternion.Euler(contactDir);
                    blood.transform.position = contactPoint;
                    blood.SetActive(true);
                }

                contactPoint = new Vector3(0, 10000, 0);
                contactDir = Vector3.zero;
            }


            Debug.Log(gameObject + "'s health: " + health);
        }
    }
}
