using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    //==================================================
    //================   走出电梯   ====================
    //=================================================

    public class room2_enter : Subscene {
        [SerializeField]
        protected GameObject[] players;

        [SerializeField]
        protected Transform playerInitPos;

        [SerializeField]
        protected Transform[] enemyInitPositions;

        [SerializeField]
        protected Elevator elevator;

        protected bool isEnter = false;

        protected bool enterElevator = false;

        //摆放玩家位置，初始化敌人
        public override void onSubsceneInit()
        {
            for (int i = 0; i < players.Length; ++i)
            {
                players[i].transform.position = playerInitPos.position;
            }
            //RenderSettings.ambientLight = Color.black;
            
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
                elevator.disableAutoOpenDoor();
                elevator.shutDown = true;
            }
        }
    }
}
