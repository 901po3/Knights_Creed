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

            character.ApplyCurrentState();
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
            if (Input.GetMouseButton(0))
            {
                character.curUndetectedTimer = 0; //공격 -> 전투 해제 타이머 리셋

                if (character.attacker != null && character.attacker.currentState == CURRENT_STATE.BLOCKED && character.canComboAttacking &&
                    (character.isDrawingWeapon && character.currentState == CURRENT_STATE.NONE || character.isDrawingWeapon && character.currentState == CURRENT_STATE.PARRY))
                {
                    character.currentState = CURRENT_STATE.COMBO_ATTACK;
                    character.PickNextAttack(true); //true면 다음 공격으로 콤보어택 석택
                }
                else if(character.isDrawingWeapon && character.currentState == CURRENT_STATE.NONE)
                {
                    character.currentState = CURRENT_STATE.ATTACK;
                }
            }
        }

        private void ParryDodgeInput()
        {
            if (character.isDrawingWeapon) //무기를 들고 있는지 체크
            {
                if (character.currentState != CURRENT_STATE.HURT && character.currentState != CURRENT_STATE.ATTACK && character.currentState != CURRENT_STATE.COMBO_ATTACK)
                {
                    if (Input.GetKeyDown(KeyCode.Space)) //피하기 시도
                    {
                        if (character.currentState != CURRENT_STATE.PARRY) //막기 시도중이면 피하기 불가능
                        {
                            if (character.runVelocity.magnitude < 0.1f && character.targetEnemy != null)                            
                            {
                                character.currentState = CURRENT_STATE.DODGE;
                            }
                            else if (character.runVelocity.magnitude > 0.1f)                            
                            {
                                character.currentState = CURRENT_STATE.MOVE_DODGE;
                            }
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Q)) //막기 시도
                    {
                        if (character.currentState != CURRENT_STATE.DODGE && character.currentState != CURRENT_STATE.MOVE_DODGE)
                        {
                            if (character.targetEnemy != null && character.targetEnemy.GetComponent<CharacterControl>().currentState == CURRENT_STATE.ATTACK)
                            {
                                character.currentState = CURRENT_STATE.PARRY;
                            }
                        }
                    }
                }
            }
        }
    }
}