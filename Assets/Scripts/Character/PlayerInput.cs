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

        private void Awake()
        {
            character = GetComponent<CharacterControl>();
            character.facingStandardTransfom = Camera.main.transform;  //플레이어의 이동 방향 기준을 카매라로 설정  
        } 

        private void Update()
        {
            MoveVerticalInput();
            StartBattleInput();
        }

        private void MoveVerticalInput()
        {
            character.runVelocity.z = Input.GetAxis("Vertical");
            character.runVelocity.x = Input.GetAxis("Horizontal");
        }

        private void StartBattleInput()
        {
            if(Input.GetMouseButtonDown(0))
                character.isBattleModeOne = !character.isBattleModeOne;
        }
    }

}