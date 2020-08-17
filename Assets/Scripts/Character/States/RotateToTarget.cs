/*
 * Class: RotateToTarget
 * Date: 2020.8.16
 * Last Modified : 2020.8.16
 * Author: Hyukin Kwon 
 * Description:  타켓 처다보기
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/RotateToTarget")]
    public class RotateToTarget : StateData
    {
        public float turnSpeed;
        public float enableDistance;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            //천천히 목표 방향값으로 회전
            CharacterControl character = characterState.GetCharacterControl(animator);

            if (character.targetEnemy != null && Vector3.Distance(character.transform.position, character.targetEnemy.transform.position) <= enableDistance)
            {
                if (character.isBattleModeOn)
                {
                    // //회전
                    Vector3 targetDirection = (character.targetEnemy.transform.position - character.transform.position).normalized;
                    targetDirection.y = 0;
                    character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards
                        (character.transform.forward, targetDirection, turnSpeed * Time.fixedDeltaTime, 0f)));
                }
            }
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
        }
    }

}