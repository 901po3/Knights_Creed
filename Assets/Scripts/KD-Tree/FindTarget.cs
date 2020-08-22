/*
 * Class: FindTarget
 * Date: 2020.8.21
 * Last Modified : 2020.8.21
 * Author: Hyukin Kwon 
 * Description: 가장 가까운 타겟을 찾는다
*/
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
            //foreach(CharacterControl character in teamOne.GetComponentsInChildren<CharacterControl>())
            //{
            //    teamOneMembers.Add(character);
            //}
            //foreach (CharacterControl character in teamTwo.GetComponentsInChildren<CharacterControl>())
            //{
            //    teamTwoMembers.Add(character);
            //}
        }

        private void Update()
        {
            RemoveDeadCharacter(teamOneMembers, teamOne);
            RemoveDeadCharacter(teamTwoMembers, teamTwo);

            teamOneMembers.UpdatePositions();
            teamTwoMembers.UpdatePositions();

            UpdateNearestObject(teamOneMembers, teamTwoMembers);
            UpdateNearestObject(teamTwoMembers, teamOneMembers);
        }

        //죽은 적 리스트에서 제거
        private void RemoveDeadCharacter(KdTree<CharacterControl> cTree, GameObject team)
        {
            cTree.Clear();
            foreach (CharacterControl character in team.GetComponentsInChildren<CharacterControl>())
            {
                if(character.health > 0)
                {
                    cTree.Add(character);
                }
            }

            //for (int i = 0; i < cTree.Count; i++)
            //{
            //    if (cTree[i].health <= 0)
            //    {
            //        foreach (CharacterControl cTarget in cTree[i].targetOnMe)
            //        {
            //            if (cTarget.attacker == cTree[i])
            //            {
            //                cTarget.attacker = null;
            //            }
            //            cTarget.targetEnemy = null;
            //        }
            //        cTree[i].targetOnMe.Clear();
            //        cTree.RemoveAt(i);
            //    }
            //}
        }

        //가장 가까운 타겟 찾기
        private void UpdateNearestObject(KdTree<CharacterControl> cTree, KdTree<CharacterControl> targetCTree)
        {
            if(targetCTree.Count > 0)
            {
                foreach (CharacterControl c in cTree)
                {
                    CharacterControl nearestTarget = targetCTree.FindClosest(c.transform.position);
                    if (c.GetComponent<AI_Input>() != null)
                    {
                        c.GetComponent<AI_Input>().nearestEnemy = nearestTarget.gameObject;
                    }
                }
            }
            else
            {
                foreach (CharacterControl c in cTree)
                {
                    if (c.GetComponent<AI_Input>() != null)
                    {
                        c.GetComponent<AI_Input>().nearestEnemy = null;
                    }
                }
            }
        }
    }
}