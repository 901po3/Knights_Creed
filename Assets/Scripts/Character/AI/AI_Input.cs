/*
 * Class: AI_Input
 * Date: 2020.8.13
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: AI 인풋을 받는 클래스
 *              AI 인풋으로 CharacterControl 상태 변수를 제어한다.
*/
using System.Collections;
using UnityEngine;

namespace HyukinKwon
{
    public class AI_Input : MonoBehaviour
    {
        //캐릭터 Gameobject의 CharacterControl 스크립트
        private CharacterControl character;

        //행동을 한번만 선택한다.
        private bool statePicked = false;

        private void Awake()
        {
            character = GetComponent<CharacterControl>();
            //자기 자신으로 기준
            character.facingStandardTransfom = character.transform;
        }

        private void Update()
        {
            DodgeInput();

            if(statePicked)
            {
                StartCoroutine(ResetPickedState(1.5f));
            }
        }

        private void DodgeInput()
        {
            if (character.targetEnemy.GetComponent<CharacterControl>().isAttacking && !statePicked)
            {
                statePicked = true;

                if(Random.Range(0, 3) == 0)
                {
                    if (character.isBattleModeOne) //전투중인지 먼저 체크 후 
                    {
                        if (character.isDrawingWeapon) //무기를 들고 있는지 체크
                        {
                            StartCoroutine(PlayDodgeDelay());
                        }
                    }
                }
            }
        }
        
        IEnumerator PlayDodgeDelay()
        {
            yield return new WaitForSeconds(0.2f);
            if(character.targetEnemy.GetComponent<CharacterControl>().isAttacking) //아직까지 공격중인지 체크
            {
                yield return new WaitForSeconds(0.3f);
                character.isDodging = true;
                character.GetAnimator().SetBool("Dodge", true);
            }
        }

        IEnumerator ResetPickedState(float frequency)
        {
            yield return new WaitForSeconds(frequency);
            statePicked = false;
        }
    }

}
