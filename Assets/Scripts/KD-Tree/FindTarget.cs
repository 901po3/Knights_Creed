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
            teamOneMembers.UpdatePositions();
            teamTwoMembers.UpdatePositions();


            for (int i = 0; i < teamOneMembers.Count; i++)
            {
                if (teamOneMembers[i].health <= 0)
                {
                    teamOneMembers.RemoveAt(i);
                }
                CharacterControl nearestTarget = teamTwoMembers.FindClosest(teamOneMembers[i].transform.position);
                if (teamOneMembers[i].GetComponent<AI_Input>() != null)
                {
                    teamOneMembers[i].GetComponent<AI_Input>().nearestEnemy = nearestTarget.gameObject;
                }
            }

            for (int i = 0; i < teamTwoMembers.Count; i++)
            {
                if(teamTwoMembers[i].health <= 0)
                {
                    teamTwoMembers.RemoveAt(i);
                }
                CharacterControl nearestTarget = teamOneMembers.FindClosest(teamTwoMembers[i].transform.position);
                if (teamTwoMembers[i].GetComponent<AI_Input>() != null)
                {
                    teamTwoMembers[i].GetComponent<AI_Input>().nearestEnemy = nearestTarget.gameObject;
                }
            }
        }
    }

}