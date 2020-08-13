/*
 * Class: Dodge
 * Date: 2020.8.13
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: Dodge 상태 조정
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/Dodge")]
    public class Dodge : StateData
    {
        public float dodgeTime = 0f; //파하기 타이머
        public float turnSpeed;
        public float speed;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);

            //캐릭터가 서있으면 반대방향으로 회전
            if (character.runVelocity == Vector3.zero)
            {
                character.transform.rotation = Quaternion.LookRotation(-character.transform.forward);
            }
        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            if (character.runVelocity == Vector3.zero)
            {
                character.GetRigidbody().MovePosition(character.transform.position + character.transform.forward * speed * Time.fixedDeltaTime);
            }
            else
            {
                character.GetRigidbody().MovePosition(character.transform.position + character.transform.forward * speed * 1.3f * Time.fixedDeltaTime);
            }


            //dodgeDuration 이후에 피하기 상태 해제
            if (character.isDodging)
            {
                if (dodgeTime < duration)
                {
                    dodgeTime += Time.deltaTime;
                    if (dodgeTime >= duration)
                    {
                        dodgeTime = 0f;
                        character.isDodging = false;
                        character.GetAnimator().SetBool("Dodging", false);
                    }
                }
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {

        }
    }
}
