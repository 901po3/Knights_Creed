/*
 * Class: CharacterControl
 * Date: 2020.8.10
 * Last Modified : 2020.8.11
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
        private Animator animator;
        private Rigidbody rigidbody;

        //상태 변수들 (애니메이션 전환에 쓰인다)
        public Vector3 runVelocity = Vector3.zero;

        //이동 방향의 기준
        //플레이어면 카매라가 바라보는 방향
        public Vector3 facingStandardDir = Vector3.zero;

        public Animator GetAnimator()
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
            return animator;
        }

        public Rigidbody GetRigidbody()
        {
            if(rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody>();
            }
            return rigidbody;
        }

    }
}
