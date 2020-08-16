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
using UnityEngine.AI;

namespace HyukinKwon
{
    public class AI_Input : MonoBehaviour
    {
        private CharacterControl character; //캐릭터 Gameobject의 CharacterControl 스크립트
        private NavMeshAgent nav;
        private bool statePicked = false; //행동을 한번만 선택한다.

        private void Awake()
        {
            character = GetComponent<CharacterControl>();
            nav = GetComponent<NavMeshAgent>();
            //자기 자신으로 기준
            character.facingStandardTransfom = character.transform;
        }

        private void Update()
        {
            if (character.targetEnemy != null)
            {
                SetDestination();

                if (character.isBattleModeOn && character.isDrawingWeapon) //전투중인지 먼저 체크 후 
                {
                    DodgeInput();
                    AttackInput();
                    if (statePicked)
                    {
                        StartCoroutine(ResetPickedState(1.5f));
                    }
                }
            }
        }

        private void SetDestination()
        {
            if(character.isTargetChanged)
            {
                character.isTargetChanged = false;
                nav.SetDestination(character.targetEnemy.transform.position);
            }
        }

        private void DodgeInput()
        {
            CharacterControl targetScript = character.targetEnemy.GetComponent<CharacterControl>();
            if (Vector3.Distance(transform.position, character.targetEnemy.transform.position) < targetScript.chargeDis)
            {
                if (targetScript.isAttacking && !statePicked)
                {
                    statePicked = true;

                    if (Random.Range(0, 3) == 0) // 1/3 확률도 피하기
                    {
                        StartCoroutine(PlayDodgeDelay());
                    }
                }
            }
        }

        private void AttackInput()
        {
            CharacterControl targetScript = character.targetEnemy.GetComponent<CharacterControl>();
            if (Vector3.Distance(transform.position, character.targetEnemy.transform.position) < character.chargeDis)
            {
                if (!statePicked)
                {
                    if (Random.Range(0, 5) == 0) // 1/5 확률도 공격
                    {
                        character.curUndetectedTimer = 0; //공격 -> 전투 해제 타이머 리셋
                        character.isAttacking = true;
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
