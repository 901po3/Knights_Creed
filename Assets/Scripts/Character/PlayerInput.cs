/*
 * Class: CharacterState
 * Date: 2020.8.10
 * Last Modified : 2020.8.11
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
            character.facingStandardTransfom = Camera.main.transform;  //플레이어의 이동 방향 기준을 카매라로 설정  
        }

        private void Update()
        {
            MoveVerticalInput();
            StartBattleInput();
            AttackInput();
        }

        private void MoveVerticalInput()
        {
            character.runVelocity.z = Input.GetAxis("Vertical");
            character.runVelocity.x = Input.GetAxis("Horizontal");
        }

        private void StartBattleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                character.isBattleModeOne = true;
            }
        }

        private void AttackInput()
        {
            if (character.isDrawingWeapon) //무기를 들고있을때
            {
                if (Input.GetMouseButtonDown(0))
                {
                    character.curUndetectedTime = 0; //공격 -> 전투 해제 타이머 리셋
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
    }

}