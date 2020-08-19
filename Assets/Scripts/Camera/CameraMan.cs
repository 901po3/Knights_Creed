/*
 * Class: CameraMan
 * Date: 2020.8.11
 * Last Modified : 2020.8.13
 * Author: Hyukin Kwon 
 * Description: 카매라 피봇의 위치를 업데이트 받는다.
*/
using UnityEngine;

namespace HyukinKwon
{
    public class CameraMan : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private float heightOffset = 2f; 
        [SerializeField] private float dampSpeed; //카매라 따라가는 속도
        private Rigidbody rigidbody;
        
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public Rigidbody GetRigidbody()
        {
            return rigidbody;
        }

        private void FixedUpdate()
        {
            //따라가는 속도 조정, 적용
            Rigidbody playerRigid = player.GetComponent<CharacterControl>().GetRigidbody();
            Vector3 playerPos = playerRigid.position;

            float speed = dampSpeed * Time.fixedDeltaTime;
            if (playerRigid.velocity.magnitude > dampSpeed)
            {
                speed = playerRigid.velocity.magnitude * Time.fixedDeltaTime;
            }
            transform.position = Vector3.Lerp(transform.position, new Vector3(playerPos.x, playerPos.y + heightOffset, playerPos.z), speed);
        }
    }
}
