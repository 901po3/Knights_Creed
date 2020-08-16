/*
 * Class: AI_Input
 * Date: 2020.8.13
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: AI 인풋을 받는 클래스
 *              AI 인풋으로 CharacterControl 상태 변수를 제어한다.
*/
using UnityEngine;

namespace HyukinKwon
{
    public class AI_Input : MonoBehaviour
    {
        //캐릭터 Gameobject의 CharacterControl 스크립트
        private CharacterControl character;

        private void Awake()
        {
            character = GetComponent<CharacterControl>();
            //자기 자신으로 기준
            character.facingStandardTransfom = character.transform;
        }

        private void Update()
        {
            if(character.targetEnemy.GetComponent<CharacterControl>().isAttacking)
            {
                if (character.isBattleModeOne) //전투중인지 먼저 체크 후 
                {
                    if (character.isDrawingWeapon) //무기를 들고 있는지 체크
                    {
                        character.isDodging = true;
                        character.GetAnimator().SetBool("Dodge", true);
                    }
                }
            }
        }
    }

}
