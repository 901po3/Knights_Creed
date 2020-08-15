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
        public float attackInputOffTime = 0.5f;
        private float curAttackTime = 0f;

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
            DodgeInput();
        }

        private void MoveVerticalInput()
        {
            character.runVelocity.z = Input.GetAxis("Vertical");
            character.runVelocity.x = Input.GetAxis("Horizontal");
            character.runVelocity.y = 0;
        }

        private void StartBattleInput()
        {
            if (Input.GetMouseButtonDown(0) && !character.isBattleModeOne)
            {
                character.isBattleModeOne = true;
                character.isChangingMode = true;
            }
        }

        private void AttackInput()
        {
            if (character.isDrawingWeapon) //무기를 들고있을때
            {
                if (Input.GetMouseButton(0))
                {
                    character.curUndetectedTimer = 0; //공격 -> 전투 해제 타이머 리셋
                    character.isAttacking = true;
                    curAttackTime = 0f;
                }
            }
            if (character.isAttacking) //일정 시간 마우스를 누르지 않으면 콤보공격 X
            {
                curAttackTime += Time.deltaTime;
                if (curAttackTime >= attackInputOffTime)
                {
                    curAttackTime = 0f;
                    character.isAttacking = false;
                }
            }
        }

        private void DodgeInput()
        {
            if(character.isBattleModeOne) //전투중인지 먼저 체크 후 
            {
                if(character.isDrawingWeapon) //무기를 들고 있는지 체크
                {
                    if(Input.GetKeyDown(KeyCode.Space))
                    {
                        character.isDodging = true;
                        character.GetAnimator().SetBool("Dodging", true);
                    }
                }
            }
        }
    }

}