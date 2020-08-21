/*
 * Class: FindTarget
 * Date: 2020.8.21
 * Last Modified : 2020.8.21
 * Author: Hyukin Kwon 
 * Description: 가장 가까운 타겟을 찾는다
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyukinKwon
{
    public class FindTarget : MonoBehaviour
    {
        public GameObject teamOne;
        public GameObject teamTwo;
        public KdTree<CharacterControl> teamOneMembers = new KdTree<CharacterControl>();
        public KdTree<CharacterControl> teamTwoMembers = new KdTree<CharacterControl>();

        private void Start()
        {            
            foreach(CharacterControl character in teamOne.GetComponentsInChildren<CharacterControl>())
            {
                teamOneMembers.Add(character);
            }
            foreach (CharacterControl character in teamTwo.GetComponentsInChildren<CharacterControl>())
            {
                teamTwoMembers.Add(character);
            }
        }

        private void Update()
        {
            teamTwoMembers.UpdatePositions();

            foreach(CharacterControl c in teamTwoMembers)
            {
                CharacterControl nearestTarget = teamOneMembers.FindClosest(c.transform.position);
                Debug.DrawLine(c.transform.position, nearestTarget.transform.position, Color.red);
            }

        }
    }

}