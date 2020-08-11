/*
 * Class: CameraMan
 * Date: 2020.8.11
 * Last Modified : 2020.8.11
 * Author: Hyukin Kwon 
 * Description: 카매라 피봇의 위치를 업데이트 받는다.
*/
using UnityEngine;

namespace HyukinKwon
{
    public class CameraMan : MonoBehaviour
    {
        [SerializeField] private GameObject player;

        private void FixedUpdate()
        {
            transform.position = player.GetComponent<CharacterControl>().GetRigidbody().position;
            transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
        }
    }
}
