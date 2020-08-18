/*
 * Class: CharacterControl
 * Date: 2020.8.10
 * Last Modified : 2020.8.17
 * Author: Hyukin Kwon 
 * Description: 캐릭터(플레이어, AI)의 상태 클래스 
 *              플레이어면 인풋 클래스와 함께 사용
 *              적이면 AI 클래스와 함께 사용
*/
using System.Collections;
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
        HIGH, MIDDLE, LOW, COMBO
    }

    public class CharacterControl : MonoBehaviour
    {
        public TEAM team;
        public int health;

        public List<GameObject> undrawedWeapon;
        public List<GameObject> drawedWeapon;
        public Equipment.WEAPON weapon;

        private Animator mAnimator;
        private Rigidbody mRigidbody;

        public float curAimTime = 0; //현재 사용하는 애니메이션의 길이
        public float curAnimSpeed = 0; //애니메이션 이동 관련에 사용되는 속도
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
        public bool isBattleModeOn = false;  //전투모드 
        public bool isDrawingWeapon = false;  //무기 들고 있는지 아닌지
        public bool isChangingMode = false;

        //어그로 관련
        public bool isDetected = false; //어그로 체크
        public float undetectedTime; //undetectedTime초 이후에 전투모드 Off

        //공격 관련
        public List<CharacterControl> attackList; //자신이 때린 적
        public bool isAttacking = false;
        public MED_ATTACK_TYPE medAttackType;
        public MED_ATTACK_TYPE prevMedAttackType;
        public float attackEnableTime = 0;
        public float attackEndTime = 0;
        public float attackRange = 0;
        public Vector3 attackChargeDes = new Vector3(0, 10000, 0); //공격시 위치 보정 용도
        public float chargeDis = 2.2f;

        //피하기 관련
        public bool isDodging = false;
        public float parryDodgeEndTime = 0; //Dodge와 Parry 애니메이션의 피하기 모션 파트가 끝나는 시간. 용도: 자연스러운 피하기 연출
        public Vector3 moveParryDodgeVec = Vector3.zero;

        //막기 관련
        public bool isParrying = false;
        public bool isBlocked = false;

        //공격 받음 관련
        public bool hurtAnimOnce = false;
        public CharacterControl attacker; //자신을 때린 적
        public bool isDead = false;
        //피 이펙트 관련 변수
        public GameObject bloodEffect;
        private List<GameObject> blooodEffectList;
        private int maxBloodNum = 3;
        private int curBloodIndex = 0;
        //칼 충돌 위치 관련 변수
        private Vector3 contactPoint = new Vector3(0, 10000, 0);
        private Vector3 contactDir = Vector3.zero;


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
        public float parryDodgeTimer = 0f; //파하기 타이머
        public float blockedTimer = 0f; //공격막힘 타이머

        private void Awake()
        {
            //맞는 무기를 장비한다.
            Equipment.Clear(this);
            Equipment.ToogleWeapon(this);
            drawedWeapon[(int)weapon].GetComponent<BoxCollider>().enabled = false;
            drawedWeapon[(int)weapon].GetComponent<CapsuleCollider>().enabled = false;
            colliders = GetComponentsInChildren<Collider>();
            rigidbodies = GetComponentsInChildren<Rigidbody>();

            mCollider = GetComponent<Collider>();
            mRigidbody = GetComponent<Rigidbody>();
            mAnimator = GetComponent<Animator>();           

            attackList = new List<CharacterControl>();

            //처음 공격 예약
            PickFirstNextAttack();

            //파티클 이펙트
            SetupParticleEffects();
        }

        private void Update()
        {
            if(isDrawingWeapon)
            {
                drawedWeapon[(int)weapon].GetComponent<WeaponScript>().FixTransform();
            }
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

        //파티클 이펙트 초기 설정
        private void SetupParticleEffects()
        {
            blooodEffectList = new List<GameObject>();
            for (int i = 0; i < maxBloodNum; i++) //피 이펙트 3개
            {
                GameObject blood = Instantiate(bloodEffect);
                blood.transform.parent = transform;
                blooodEffectList.Add(blood);
                blood.SetActive(false);
            }
            bloodEffect.SetActive(false);
        }

        //피해 적용
        private void CheckHurt(Collision collision)
        {
            if(collision.transform.tag == "Weapon")
            {
                //공격한 대상 등록
                attacker = collision.gameObject.GetComponentInParent<CharacterControl>();
                targetEnemy = attacker.gameObject;

                if (!isDodging && !isDead && !isParrying)
                {                               //Parrying떄 콜라이더가 쳐지기 떄문에 예외처리 필요
                    if (attacker.team != team && (!attacker.isParrying || attacker.medAttackType == MED_ATTACK_TYPE.COMBO)) //적이 막는중이 아니면 다친다
                    {
                        //충돌 위치 저장
                        //용도: 옳바른 위치에 파티클 이팩트 생성
                        contactDir = (gameObject.transform.position - collision.contacts[0].point).normalized;
                        contactPoint = collision.contacts[0].point;

                        //피 이팩트 생성
                        if (contactPoint != new Vector3(0, 10000, 0))
                        {
                            GameObject blood = blooodEffectList[curBloodIndex];
                            if (!blood.activeSelf)
                            {
                                curBloodIndex = (curBloodIndex + 1) % maxBloodNum;
                                blood.transform.rotation = Quaternion.Euler(contactDir);
                                blood.transform.position = contactPoint;
                                blood.SetActive(true);
                                StartCoroutine(TurnOffBloodEffect(blood)); //일정 시간후 비활성
                            }
                            contactPoint = new Vector3(0, 10000, 0);
                            contactDir = Vector3.zero;
                        }
                        collision.gameObject.GetComponent<Collider>().isTrigger = true;

                        if (!hurtAnimOnce)
                        {
                            hurtAnimOnce = true;

                            //일정 시간마다 Hurt 애니메이션 재생
                            StartCoroutine(HurtPlayFrequency());

                            mAnimator.SetBool("Hurt", true);
                            mAnimator.SetBool("Dead", false);
                        }
                    }
                }
            }
            
        }

        IEnumerator TurnOffBloodEffect(GameObject blood)
        {
            yield return new WaitForSeconds(0.8f);
            blood.SetActive(false);
        }

        IEnumerator HurtPlayFrequency()
        {
            yield return new WaitForSeconds(2f);
            hurtAnimOnce = false;
        }

        //미리 다음 공격을 정해둔다
        //미라 정하는 이유: 상대방이 옳바른 Dodge와 Parry 애니메이션을 재생
        public void PickFirstNextAttack()
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    prevMedAttackType = MED_ATTACK_TYPE.HIGH;
                    break;
                case 1:
                    prevMedAttackType = MED_ATTACK_TYPE.MIDDLE;
                    break;
                case 2:
                    prevMedAttackType = MED_ATTACK_TYPE.LOW;
                    break;
            }
            PickNextAttack(false);
        }

        public void PickNextAttack(bool Combo)
        {
            if(!Combo)
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        medAttackType = MED_ATTACK_TYPE.HIGH;
                        curAimTime = 1.783f;
                        attackEndTime = 1f;
                        attackEnableTime = 0.3f;
                        attackRange = 1.2f;
                        break;
                    case 1:
                        medAttackType = MED_ATTACK_TYPE.MIDDLE;
                        curAimTime = 1.5f;
                        attackEndTime = 0.75f;
                        attackEnableTime = 0.3f;
                        attackRange = 0.7f;
                        break;
                    case 2:
                        medAttackType = MED_ATTACK_TYPE.LOW;
                        curAimTime = 1.9f;
                        attackEndTime = 1f;
                        attackEnableTime = 0.3f;
                        attackRange = 1.2f;
                        break;
                }
                if (prevMedAttackType == medAttackType)
                {
                    medAttackType = (medAttackType + 1);
                    if ((int)medAttackType >= 3)
                        medAttackType = 0;
                }
                prevMedAttackType = medAttackType;
            }
            else
            {
                medAttackType = MED_ATTACK_TYPE.COMBO;
                curAimTime = 1.4f;
                attackEndTime = 0.55f;
                attackEnableTime = 0.45f;
                attackRange = 0.75f;
            }
        }
    }
}
