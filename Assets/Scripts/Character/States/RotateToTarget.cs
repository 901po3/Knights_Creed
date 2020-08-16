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
        public float rotSpeed;

        public override void StartAbility(CharacterState characterState, Animator animator)
        {

        }

        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            //천천히 목표 방향값으로 회전
            CharacterControl character = characterState.GetCharacterControl(animator);

            if(character.tag == "AI") //임시 사용
            {
                if (character.targetEnemy != null)
                {
                    if (character.isBattleModeOn)
                    {
                        // //회전
                        // Vector3 targetDirection = (character.targetEnemy.transform.position - character.transform.position).normalized;
                        //// targetDirection = character.facingStandardTransfom.TransformDirection(targetDirection);
                        // targetDirection.y = 0f;

                        // character.transform.rotation = Quaternion.Lerp(character.transform.rotation, Quaternion.Euler(targetDirection), rotSpeed * Time.deltaTime);
                        // //character.GetRigidbody().MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards
                        // //    (character.transform.forward, targetDirection, rotSpeed * Time.fixedDeltaTime, 0f)));
                        Vector3 target = character.targetEnemy.transform.position;
                        target.y = 0;
                        character.transform.LookAt(target);
                    }
                }
            }           
        }

        public override void ExitAbility(CharacterState characterState, Animator animator)
        {
        }
    }

}