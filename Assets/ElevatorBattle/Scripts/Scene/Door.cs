using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Elevator
{
    public class Door : MonoBehaviour, NetSyncInterface
    {
        private GameObject child1;
        private GameObject child2;

        private bool open;
        private bool close;

        NetSyncController m_controller;

        //true: open false: close
        public static bool state;

        [SerializeField]
        public float speed;

        // Use this for initialization
        void Start()
        {
            child1 = GameObject.Find("left");
            child2 = GameObject.Find("right");
            open = false;
            close = false;
            state = false;
        }

        // Update is called once per frame
        void Update()
        {
            //关门过程结束
            if (child1.transform.position.z <= 2.5 || child2.transform.position.z >= -2.5)
            {
                state = false;
                close = false;
            }

            //开门过程结束
            if (child1.transform.position.z >= 6 || child2.transform.position.z <= -6)
            {
                state = true;
                open = false;
            }

            //打开门
            if (open)
            {
                child1.transform.position += new Vector3(0, 0, speed);
                child2.transform.position -= new Vector3(0, 0, speed);
            }

            //关上门
            if (close)
            {
                child1.transform.position -= new Vector3(0, 0, speed);
                child2.transform.position += new Vector3(0, 0, speed);
            }

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

        //发信息
        private void Operate()
        {
            Operate_Imp();
            m_controller.RPC(this, "Operate_Imp");
        }

        private void Operate_Imp()
        {
            switch (SceneController.state)
            {
                case SceneController.ElevatorState.Initing:
                    if (!state)
                    {
                        //开门
                        open = true;
                    }
                    break;

                case SceneController.ElevatorState.FindingButton:
                    if (state)
                    {
                        //关门
                        close = true;
                    }
                    break;

                case SceneController.ElevatorState.Start_Fighting:
                    if (state)
                    {
                        //关门
                        close = true;
                    }
                    break;

                case SceneController.ElevatorState.Fighting:
                    if (state)
                    {
                        //关门
                        close = true;
                    }
                    break;

                case SceneController.ElevatorState.End:
                    if (!state)
                    {
                        //开门
                        open = true;
                    }
                    break;
                case SceneController.ElevatorState.Escape:
                    if (state)
                    {
                        //关门
                        close = true;
                    }
                    break;
            }

        }
    }
}
