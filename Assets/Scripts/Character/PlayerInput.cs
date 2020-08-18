/*
 * Class: CharacterState
 * Date: 2020.8.10
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: 플레이어의 인풋을 받는 클래스
 *              플레이어의 인풋으로 CharacterControl 상태 변수를 제어한다.
*/
using UnityEngine;

namespace HyukinKwon
{
    public class PlayerInput : MonoBehaviour
    {
        //캐릭터 Gameobject의 CharacterControl 스크립트
        private CharacterControl character;

        private void Awake()
        {
            character = GetComponent<CharacterControl>();
            //플레이어의 이동 방향 기준을 카매라로 설정  
            character.facingStandardTransfom = Camera.main.transform;  
        }

        private void Update()
        {
            if (character.isChangingMode) return;
            MoveVerticalInput();
            StartBattleInput();
            AttackInput();
            ParryDodgeInput();

            //if(character.targetEnemy != null)
            //    Debug.Log(Vector3.Distance(transform.position, character.targetEnemy.transform.position));
        }

        private void MoveVerticalInput()
        {
            character.runVelocity.z = Input.GetAxis("Vertical");
            character.runVelocity.x = Input.GetAxis("Horizontal");
            character.runVelocity.y = 0;
        }

        private void StartBattleInput()
        {
            if (Input.GetMouseButtonDown(0) && !character.isBattleModeOn)
            {
                character.isBattleModeOn = true;
                character.isChangingMode = true;
            }
        }

        private void AttackInput()
        {
            if(!character.isDodging)
            {
                if (character.isDrawingWeapon) //무기를 들고있을때
                {
                    if (Input.GetMouseButton(0))
                    {
                        if(character.attacker != null && character.attacker.isBlocked && character.medAttackType != MED_ATTACK_TYPE.COMBO)
                        {
                            character.isAttacking = true;
                            character.PickNextAttack(true);
                            character.GetAnimator().SetTrigger("ComboAttack");
                        }
                        else
                        {
                            character.curUndetectedTimer = 0; //공격 -> 전투 해제 타이머 리셋
                            character.isAttacking = true;
                        }
                        character.isDodging = false;
                        character.GetAnimator().SetBool("Dodge", false);
                    }
                }
            }
        }

        private void ParryDodgeInput()
        {
            if (character.isBattleModeOn) //전투중인지 먼저 체크 후 
            {
                if(character.isDrawingWeapon) //무기를 들고 있는지 체크
                {
                    if(Input.GetKeyDown(KeyCode.Space)) //피하기 시도
                    { 
                        if(character.runVelocity.magnitude < 0.1f && character.targetEnemy != null)
                        {
                            character.isDodging = true;
                            character.GetAnimator().SetBool("Dodge", true);
                           
                        }
                        else if(character.runVelocity.magnitude > 0.1f)
                        {
                            character.GetAnimator().SetBool("MoveDodge", true);
                        }
                        character.isAttacking = false;
                    }
                    else if(Input.GetKeyDown(KeyCode.Q)) //막기 시도
                    {
                        if (character.runVelocity.magnitude < 0.1f && character.targetEnemy != null)
                        {
                            character.isParrying = true;
                            character.GetAnimator().SetBool("Parry", true);

                        }
                    }
                }
            }
        }

    }
}