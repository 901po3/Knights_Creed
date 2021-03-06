﻿/*
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
        [SerializeField] private float rotSpeed;
        [SerializeField] float minAngle;
        [SerializeField] float maxAngle;
        float prevX = 0;

        public bool lockCursor = false;

        private void Awake()
        {
            transform.position = cameraManObj.transform.position;          
        }

        private void Update()
        {
            Rotate();
            if(lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked; //회전을 위해 마우스커서 잠금
            }
        }

        private void FixedUpdate()
        {
            //transform.rotation = cameraManObj.transform.rotation;
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
            Vector3 dir = -cameraManObj.transform.forward;
            if (Physics.Raycast(cameraManObj.transform.position, dir, out hit, distance)) //만약 카매라와 플레이어 사이에 물체가 있으면 줌인
            {
                if(hit.transform.tag == "Weapon" || hit.transform.tag == "Player" || hit.transform.tag == "MainCamera" || hit.transform.tag == "AI")
                {
                    Debug.DrawRay(cameraManObj.transform.position, dir * distance, Color.red);
                    transform.localPosition = Vector3.Lerp(transform.position, cameraManObj.transform.position + dir * distance, distanceSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    transform.localPosition = Vector3.Lerp(transform.position, cameraManObj.transform.position + dir * hit.distance, distanceSpeed * Time.fixedDeltaTime);
                    Debug.DrawRay(cameraManObj.transform.position, dir * hit.distance, Color.green);
                }
            }
            else //사이에 물체가 없으면 기존에 구한 Distance값 적용
            {
                Debug.DrawRay(cameraManObj.transform.position, dir * distance, Color.red);
                transform.localPosition = Vector3.Lerp(transform.position, cameraManObj.transform.position + dir * distance, distanceSpeed * Time.fixedDeltaTime);
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
            float sensitivity = 10f;
            angle.y += Input.GetAxis("Mouse X") * sensitivity;
            angle.x -= Input.GetAxis("Mouse Y") * sensitivity;
            //각도값 사용이 편하게 보정 (minAngle(작은수) ~maxAngle(큰수))
            float a = angle.x + 90;
            if (a > 360)
                a -= 360;
            if(a > maxAngle) //보정한 각도값 적용
            {
                angle.x -= (a - maxAngle);
            }
            else if( a < minAngle)
            {
                angle.x += (minAngle - a);
            }
                        
            Quaternion rot = Quaternion.Euler(angle.x, angle.y, 0);
            cameraManObj.transform.rotation = Quaternion.Slerp(cameraManObj.transform.rotation, rot, rotSpeed * Time.deltaTime);


            //3. 회전 후 저장했던 distance만큼 이동
            transform.position = transform.position - cameraManObj.transform.forward * curDistance;

            prevX = Input.GetAxis("Mouse X");
            transform.rotation = cameraManObj.transform.rotation;
        }
    }
}
