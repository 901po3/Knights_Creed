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
        [SerializeField] private GameObject cameraManObj;

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
            Cursor.visible = false;
            transform.position = cameraManObj.transform.position;
        }

        private void Update()
        {
            Rotate();
            Move();
        }

        //private void FixedUpdate()
        //{
        //    if (Vector3.Distance(cameraManObj.transform.position, transform.position) > maxDistance)
        //    {
        //        transform.position = transform.position - cameraManObj.transform.forward * maxDistance;
        //    }
        //}

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
            Vector3 dir = -cameraManObj.transform.forward;
            if (Physics.Raycast(cameraManObj.transform.position, dir, out hit, distance)) //만약 카매라와 플레이어 사이에 물체가 있으면 줌인
            {
                if (hit.transform.tag != "MainCamera")
                {
                    transform.localPosition = Vector3.Lerp(transform.position, cameraManObj.transform.position + dir * hit.distance, distanceSpeed * Time.deltaTime);
                    Debug.DrawRay(cameraManObj.transform.position, dir * hit.distance, Color.green);
                }
            }
            else //사이에 물체가 없으면 기존에 구한 Distance값 적용
            {
                Debug.DrawRay(cameraManObj.transform.position, dir * distance, Color.red);
                transform.localPosition = Vector3.Lerp(transform.position, cameraManObj.transform.position + dir * distance, distanceSpeed * Time.deltaTime);
            }
            transform.localPosition = new Vector3(transform.localPosition.x, cameraManObj.transform.position.y, transform.localPosition.z);
        }

        private void Rotate()
        {
            //1. 현재 distance를 저장 후 Camera의 위치를 cameraManObj로 이동
            float curDistance = Vector3.Distance(cameraManObj.transform.position, transform.position);
            transform.position = cameraManObj.transform.position;

            //2. cameraManObj를 회전
            Vector3 angle = cameraManObj.transform.eulerAngles;
            float mouseY = Input.GetAxis("Mouse Y");
            if (mouseY > 0)  //마우스 오른쪽으로 이동
            {
                angle.y += Input.GetAxis("Mouse Y");
            }
            else if (mouseY < 0) //마우스 왼쪽 이동
            {
                angle.y -= Input.GetAxis("Mouse Y");
            }
            Quaternion rot = Quaternion.Euler(angle);
            cameraManObj.transform.rotation = Quaternion.Slerp(cameraManObj.transform.rotation, rot, rotSpeed * Time.deltaTime);

            //3. 회전 후 저장했던 distance만큼 이동
            transform.position = transform.position - cameraManObj.transform.forward * curDistance;
            transform.rotation = cameraManObj.transform.rotation;
            //transform.LookAt(cameraManObj.transform);

        }
    }
}
