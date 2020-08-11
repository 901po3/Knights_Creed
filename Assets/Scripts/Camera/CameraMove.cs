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

        //이동 관련 변수
        [SerializeField] private float distance;
        [SerializeField] private float minDistance;
        [SerializeField] private float maxDistance;
        [SerializeField] private float zoomSpeed; // 마우스 스크롤에 의한 줌 속도
        [SerializeField] private float distanceSpeed; //부드러운 카매라 이동

        //회전 관련 변수
        [SerializeField] private float minHeight;
        [SerializeField] private float maxHeight;
        [SerializeField] private float rotSpeed;

        private void Awake()
        {
            transform.position = campPivotObj.transform.position;
        }

        private void Update()
        {
            //Rotate();
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
            }
            else //사이에 물체가 없으면 기존에 구한 Distance값 적용
            {
                transform.localPosition = Vector3.Lerp(transform.position, campPivotObj.transform.position - Vector3.forward * distance, distanceSpeed * Time.deltaTime);
            }
        }

        private void Rotate()
        {
            Vector3 angle = campPivotObj.transform.eulerAngles;
            angle.y += Input.GetAxis("Mouse X") * Time.deltaTime * rotSpeed;
            angle.x += Input.GetAxis("Mouse Y") * Time.deltaTime * rotSpeed;
            angle.x = Mathf.Clamp(angle.x, minHeight, maxHeight);

            Quaternion rot = Quaternion.Euler(angle);
            campPivotObj.transform.rotation = Quaternion.Slerp(campPivotObj.transform.rotation, rot, rotSpeed * Time.deltaTime);
            transform.rotation = campPivotObj.transform.rotation;
        }
    }
}
