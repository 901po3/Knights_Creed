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
        [SerializeField] private GameObject CamPivotObj;
        private void Update()
        {
            transform.position = CamPivotObj.transform.position;
        }
    }
}
