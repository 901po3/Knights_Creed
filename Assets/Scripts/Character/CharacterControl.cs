/*
 * Class: CharacterControl
 * Date: 2020.8.10
 * Last Modified : 2020.8.10
 * Author: Hyukin Kwon 
 * Description: 캐릭터(플레이어, AI)의 상태 클래스 
 *              플레이어면 인풋 클래스와 함께 사용
 *              적이면 AI 클래스와 함께 사용
*/
using UnityEngine;

namespace HyukinKwon
{
    public class CharacterControl : MonoBehaviour
    {
        public float speed;
        private Animator animator;

        //상태 변수들
        public Vector3 velocity = Vector3.zero;

        public Animator GetAnimator()
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
            return animator;
        }

    }
}
