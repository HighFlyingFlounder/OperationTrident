using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Elevator {
    public class SceneController : MonoBehaviour, NetSyncInterface
    {
        [SerializeField]

        //持续时间
        public int d_time = 60;

        //准备时间
        public int r_time = 10;

        //状态
        public enum ElevatorState { Initing, Ready, Start_Fighting ,Fighting, End, Escape };
        public static ElevatorState state;

        //开始战斗的时间
        private float s_time;
        //现在时间
        private float c_time;
        //结束时间
        private float e_time;

        //碰撞次数（为偶数）
        private int count = 0;

        private bool change;

        //碰撞体
        private BoxCollider bcollider;

        NetSyncController m_controller = null;

        // Use this for initialization
        void Start()
        {
            state = ElevatorState.Initing;
            bcollider = this.GetComponent<BoxCollider>();
            change = true;
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case ElevatorState.Initing:
                    break;

                case ElevatorState.Ready:
                    if (change)
                    {
                        //开始计时
                        s_time = Time.time;
                        c_time = s_time;
                        e_time = s_time + r_time;

                        bcollider.size = new Vector3(40, bcollider.size.y, bcollider.size.z);

                        change = false;
                    }
                    else
                    {
                        c_time += Time.deltaTime;

                        //准备时间结束，切换到下一个场景
                        if (c_time >= e_time)
                            changeState();
                    }
                    break;

                case ElevatorState.Start_Fighting:
                    Messenger.Broadcast(GameEvent.Enemy_Start);

                    //开始计时
                    s_time = Time.time;
                    c_time = s_time;
                    e_time = s_time + d_time;

                    changeState();
                    break;

                case ElevatorState.Fighting:
                    c_time += Time.deltaTime;

                    if(c_time >= e_time)
                    {
                        change = true;
                        changeState();
                    }

                    break;

                case ElevatorState.End:
                    Messenger.Broadcast(GameEvent.End);

                    //开门
                    GameObject.Find("DoorTrigger").SendMessage("openDoor", SendMessageOptions.DontRequireReceiver);
                    change = false;

                    changeState();
                    break;

                case ElevatorState.Escape:
                    break;
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

        void OnTriggerEnter(Collider other)
        {
            if (state == ElevatorState.Initing || state == ElevatorState.End)
            {
                //if (other.GetComponent<NetSyncTransform>().ctrlType != NetSyncTransform.CtrlType.player)
                    //return;
                int number = 1;//SceneNetManager.instance.list.Count;

                count++;

                //进入关门
                if (count >= number && Door.state && state == ElevatorState.Initing)
                {
                    changeState();
                    GameObject.Find("DoorTrigger").SendMessage("closeDoor", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (state == ElevatorState.Initing || state == ElevatorState.Escape)
            {
                //离开关门
                count--;

                if (count <= 0 && Door.state && state == ElevatorState.Escape)
                {
                    changeState();
                    GameObject.Find("DoorTrigger").SendMessage("closeDoor", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        public void changeState()
        {
            switch (state)
            {
                case ElevatorState.Initing:
                    state = ElevatorState.Ready;
                    break;

                case ElevatorState.Ready:
                    state = ElevatorState.Start_Fighting;
                    break;

                case ElevatorState.Start_Fighting:
                    state = ElevatorState.Fighting;
                    break;

                case ElevatorState.Fighting:
                    state = ElevatorState.End;
                    break;

                case ElevatorState.End:
                    state = ElevatorState.Escape;
                    break;
            }
        }
    }
}
