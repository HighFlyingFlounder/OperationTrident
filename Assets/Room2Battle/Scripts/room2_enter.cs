using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    //==================================================
    //================   走出电梯   ====================
    //=================================================

    public class room2_enter : Subscene, NetSyncInterface
    {
        NetSyncController m_controller;
        [SerializeField]
        protected GameObject[] players;

        [SerializeField]
        protected Transform playerInitPos;

        [SerializeField]
        protected GameObject player;

        protected bool isNear = false;

        protected bool isEnter = false;

        //摆放玩家位置，初始化敌人
        public override void onSubsceneInit()
        {
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
                isNear = true;
                player.GetComponent<becomeDark>().enabled = true;
                m_controller.RPC("method_need_to_sync",this.GetType(),1,"string for param2");
            }
            else if (i == 2)
            {
                isEnter = true;
            }
        }

        public void method_need_to_sync(int param1,string param2){
            Debug.Log(param1);
            Debug.Log(param2);
        }

        void OnGUI()
        {
            if (!isNear)
            {
                OperationTrident.Util.GUIUtil.DisplayMissionTargetDefault("突入电源室！", Camera.main, true);
            }
            else
            {
                OperationTrident.Util.GUIUtil.DisplayMissionTargetDefault("任务变化：开启照明开关！", Camera.main, true);
            }
        }

        public void RecvData(SyncData data)
        {
            isNear = (bool)data.Get(typeof(bool));
            isEnter = (bool)data.Get(typeof(bool));
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            data.Add(isNear);
            data.Add(isEnter);
            return data;
        }

        public void Init(NetSyncController controller)
        {
            m_controller = controller;
        }
    }
}
