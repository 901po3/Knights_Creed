/*
 * Class: CameraMove
 * Date: 2020.8.11
 * Last Modified : 2020.8.11
 * Author: Hyukin Kwon 
 * Description: 카매라 행동
*/
using UnityEngine;

namespace HyukinKwon
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] private GameObject campPivotObj;
        [SerializeField] private float distance;
        [SerializeField] private float minDistance;
        [SerializeField] private float maxDistance;
        [SerializeField] private float zoomSpeed;
        private float distanceSpeed = 15;

        private void Awake()
        {
            transform.position = campPivotObj.transform.position;
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            //Distance를 구한다
            if(scroll > 0 && distance > minDistance) //줌인
            {
                distance -= zoomSpeed * Time.deltaTime;
            }
            else if(scroll < 0 && distance < maxDistance) //줌아웃
            {
                distance += zoomSpeed * Time.deltaTime;
            }
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            RaycastHit hit;
            Vector3 dir = (campPivotObj.transform.position - Vector3.forward * distance) - campPivotObj.transform.position;
            if (Physics.Raycast(campPivotObj.transform.position, dir, out hit, distance)) //만약 카매라와 플레이어 사이에 물체가 있으면 줌인
            {
                if (hit.transform.tag != "Player")
                {
                    transform.localPosition = Vector3.Lerp(transform.position, campPivotObj.transform.position - Vector3.forward * hit.distance, distanceSpeed * Time.deltaTime);
                }
                Debug.Log(hit.transform.gameObject);
            }
            else //사이에 물체가 없으면 기존에 구한 Distance값 적용
            {
                transform.localPosition = Vector3.Lerp(transform.position, campPivotObj.transform.position - Vector3.forward * distance, distanceSpeed * Time.deltaTime);
                Debug.Log("Did not hit");
            }
        }
    }
}
