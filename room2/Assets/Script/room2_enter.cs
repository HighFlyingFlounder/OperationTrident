using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    //==================================================
    //================   走出电梯   ====================
    //=================================================

    public class room2_enter : Subscene {

        //敌人容器，用于初始化，以及之后转移AI的目标位置到2楼
        [SerializeField]
        protected GameObject enemyPrefabs;

        protected ArrayList enemyList = new ArrayList();

        protected int currentEnemyNum = 0;

        protected int maxEnemyNum = 20;

        [SerializeField]
        protected GameObject[] players;

        [SerializeField]
        protected Transform playerInitPos;

        [SerializeField]
        protected Transform[] enemyInitPositions;

        [SerializeField]
        protected Elevator elevator;

        protected bool isEnter = false;

        //摆放玩家位置，初始化敌人
        public override void onSubsceneInit()
        {
            for (int i = 0; i < maxEnemyNum; ++i)
            {
                GameObject obj = Instantiate(enemyPrefabs, enemyInitPositions[Random.Range(0, enemyInitPositions.Length)].position, Quaternion.identity);
                enemyList.Add(obj);
            }

            for (int i = 0; i < players.Length; ++i)
            {
                players[i].transform.position = playerInitPos.position;
            }
        }

        public override bool isTransitionTriggered()
        {
            return isEnter;
        }

        public override string GetNextSubscene()
        {
            return "room2_powerroom";
        }

        public override void onSubsceneDestory()
        {
            
        }

        public override void notify(int i)
        {
            if (i == 1)
            {
                isEnter = true;
                elevator.shutDown = true;
            }
        }
    }
}
