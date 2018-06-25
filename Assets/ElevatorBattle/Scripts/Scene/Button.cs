using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Elevator
{
    public class Button : MonoBehaviour, NetSyncInterface
    {
        NetSyncController m_controller;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //RPC调用
        private void Operate()
        {
            Operate_Imp();
            m_controller.RPC(this, "Operate_Imp");
        }

        public void Operate_Imp()
        {
            Messenger<int>.Broadcast(GameEvent.Push_Button, 0);
        }

        //网络同步
        public void RecvData(SyncData data)
        {
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            return data;
        }

        public void Init(NetSyncController controller)
        {
            m_controller = controller;
        }
    }
}
