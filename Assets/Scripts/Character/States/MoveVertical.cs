/*
 * Class: MoveVertical
 * Date: 2020.8.10
 * Last Modified : 2020.8.10
 * Author: Hyukin Kwon 
 * Description:  앞뒤 이동 에니메이션 스크립터블오브젝트
*/
using UnityEngine;

namespace HyukinKwon
{
    [CreateAssetMenu(fileName = "New State", menuName = "HyukinKwon/AbilityData/MoveVertical")]
    public class MoveVertical : StateData
    {
        public float forwardSpeed;
        public float backwardSpeed;

        //속도에 따라 앞뒤 이동과 맞는 애니메이션 재생
        public override void UpdateAbility(CharacterState characterState, Animator animator)
        {
            CharacterControl character = characterState.GetCharacterControl(animator);
            float vz = character.velocity.z;
            animator.SetFloat("RunningVeritical", vz);
            if(vz <= 0.1f && vz >= -0.1f)
            {
                return;
            }
            else if(vz > 0.1f)
            {
                character.transform.Translate(character.velocity * forwardSpeed * character.speed * Time.deltaTime);
            }
            else if(vz < -0.1f)
            {
                character.transform.Translate(character.velocity * backwardSpeed * character.speed * Time.deltaTime);
            }
        }
    }
}
