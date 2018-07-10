using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;
using OperationTrident.Common.AI;

namespace OperationTrident.Elevator {
    public class SceneController : MonoBehaviour, NetSyncInterface
    {
        [SerializeField]
        WanderAIAgentInitParams[] wanderAIAgentInitParams;

        [SerializeField]
        TurretAIAgentInitParams[] turretAIAgentInitParams;

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

        private float t_time;

        //碰撞次数
        private int count = 0;

        private bool change;

        private bool flag;

        private bool flag1;

        private IEnumerator coroutine1;

        private IEnumerator coroutine2;

        //碰撞体
        private BoxCollider bcollider;

        NetSyncController m_controller = null;

        // Use this for initialization
        void Start()
        {
            state = ElevatorState.Initing;
            bcollider = this.GetComponent<BoxCollider>();
            change = true;

            flag = true;
            flag1 = true;

            coroutine1 = WaitAndPrint1(10);
            coroutine2 = WaitAndPrint2(10);
        }

        // Update is called once per frame
        void Update()
        {

            switch (state)
            {
                case ElevatorState.Initing:
                    if (GameMgr.instance && flag)
                    {
                        foreach (var player in SceneNetManager.instance.list)
                        {
                            GameObject temp = player.Value;
                            temp.transform.localScale = new Vector3(3f, 3f, 3f);
                        }
                        flag = false;
                    }
                    break;

                case ElevatorState.Ready:
                    if (change)
                    {
                        //开始计时
                        s_time = Time.time;
                        c_time = s_time;
                        e_time = s_time + r_time;

                        bcollider.size = new Vector3(bcollider.size.x * 2f, bcollider.size.y, bcollider.size.z);

                        change = false;

                        GameObject.Find("BGM").GetComponent<AudioSource>().Play();
                    }
                    else
                    {
                        c_time += Time.deltaTime;
                        

                        //准备时间结束，切换到下一个场景
                        if (c_time >= e_time && GameMgr.instance.isMasterClient)
                        {
                            changeState();
                            m_controller.RPC(this, "changeState");
                        }
                    }
                    break;

                case ElevatorState.Start_Fighting:
                    //开始计时
                    s_time = Time.time;
                    c_time = s_time;
                    e_time = s_time + d_time;

                    t_time = s_time + 5;

                    if (GameMgr.instance.isMasterClient)
                    {
                        changeState();
                        m_controller.RPC(this, "changeState");
                    }
                    break;

                case ElevatorState.Fighting:
                    c_time += Time.deltaTime;

                    if (!flag)
                    {
                        StartCoroutine(coroutine1);
                        flag = true;
                    }

                    if (flag1&&c_time >= t_time)
                    {
                        StartCoroutine(coroutine2);
                        flag1 = false;
                    }

                    if (c_time >= e_time && GameMgr.instance.isMasterClient)
                    {
                        changeState();
                        m_controller.RPC(this, "changeState");
                    }

                    break;

                case ElevatorState.End:
                    //Messenger.Broadcast(GameEvent.End);

                    //开门
                    GameObject.Find("DoorTrigger").SendMessage("openDoor", SendMessageOptions.DontRequireReceiver);

                    if (OperationTrident.Elevator.Wall.state && GameMgr.instance.isMasterClient)
                    {
                        changeState();
                        m_controller.RPC(this, "changeState");
                    }

                    break;

                case ElevatorState.Escape:
                    StopCoroutine(coroutine1);
                    StopCoroutine(coroutine2);
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
            if ((state == ElevatorState.Initing || state == ElevatorState.Escape) && other.tag == "Player")
            {
                //if (other.GetComponent<NetSyncTransform>().ctrlType != NetSyncTransform.CtrlType.player)
                //return;
                int number = SceneNetManager.instance.list.Count;

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
            if ((state == ElevatorState.Initing || state == ElevatorState.Escape) && other.tag == "Player")
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

        private IEnumerator WaitAndPrint1(float waitTime)
        {
            while (true)
            {
                //生成物体
                AIController.instance.CreateAI(1, 0, "AIborn0", wanderAIAgentInitParams[0]);
                AIController.instance.CreateAI(1, 0, "AIborn2", wanderAIAgentInitParams[1]);
                AIController.instance.CreateAI(1, 0, "AIborn4", wanderAIAgentInitParams[2]);
                AIController.instance.CreateAI(1, 0, "AIborn6", wanderAIAgentInitParams[3]);
                yield return new WaitForSeconds(waitTime);
            }
        }

        private IEnumerator WaitAndPrint2(float waitTime)
        {
            while (true)
            {
                //生成物体
                AIController.instance.CreateAI(1, 0, "AIborn1", wanderAIAgentInitParams[0]);
                AIController.instance.CreateAI(1, 0, "AIborn3", wanderAIAgentInitParams[1]);
                AIController.instance.CreateAI(1, 0, "AIborn5", wanderAIAgentInitParams[2]);
                AIController.instance.CreateAI(1, 0, "AIborn7", wanderAIAgentInitParams[3]);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
