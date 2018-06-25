using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Elevator {
    public class SceneController : MonoBehaviour, NetSyncInterface
    {
        [SerializeField]
        //电梯开关的预设
        public GameObject buttonPre;
        //持续时间
        public int d_time = 60;

        //状态
        public enum ElevatorState { Initing, FindingButton, Start_Fighting ,Fighting, End, Escape };
        public static ElevatorState state;

        //开始战斗的时间
        private float s_time;
        //现在时间
        private float c_time;
        //结束时间
        private float e_time;

        //按钮
        private GameObject button;
        //button的位置
        public static Vector3 ButtonPosition;

        //碰撞次数（为偶数）
        private int count = 0;

        //是否更改碰撞体
        private bool change = false;

        //碰撞体
        private BoxCollider bcollider;

        NetSyncController m_controller = null;

        // Use this for initialization
        void Start()
        {
            //初始化BUTTON
            button = GameObject.Instantiate(buttonPre);
            button.transform.localPosition = new Vector3(0, 2, 0);
            ButtonPosition = button.transform.localPosition;
            state = ElevatorState.Initing;
            bcollider = this.GetComponent<BoxCollider>();
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case ElevatorState.Initing:
                    break;

                case ElevatorState.FindingButton:
                    break;

                case ElevatorState.Start_Fighting:
                    Messenger<int>.Broadcast(GameEvent.Enemy_Start, 0);
                    state = ElevatorState.Fighting;
                    s_time = Time.time;
                    c_time = s_time;
                    e_time = s_time + d_time;
                    bcollider.size = new Vector3(10f, bcollider.size.y, bcollider.size.z);
                    break;

                case ElevatorState.Fighting:
                    c_time += Time.deltaTime;
                    if(c_time >= e_time)
                    {
                        change = true;
                        state = ElevatorState.End;
                    }

                    break;

                case ElevatorState.End:
                    Messenger<int>.Broadcast(GameEvent.End, 0);

                    if (change)
                    {
                        //更改碰撞体
                        bcollider.size = new Vector3(12f, bcollider.size.y, bcollider.size.z);
                        //开门
                        GameObject.Find("DoorTrigger").SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                        change = false;
                    }
                    break;

                case ElevatorState.Escape:
                    break;
            }
        }

        //监听器
        private void Awake()
        {
            Messenger<int>.AddListener(GameEvent.Push_Button, fight);
        }

        private void Destroy()
        {
            Messenger<int>.RemoveListener(GameEvent.Push_Button, fight);
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
                int number = SceneNetManager.instance.list.Count;
                //进入关门
                count++;
                Debug.Log("Enter" + count);
                if (count >= number && Door.state && state == ElevatorState.Initing)
                {
                    changeState();
                    m_controller.RPC(this, "changeState");
                    GameObject.Find("DoorTrigger").SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (state == ElevatorState.Initing || state == ElevatorState.End)
            {
                //离开关门
                count--;
                if (count <= 0 && Door.state && state == ElevatorState.End)
                {
                    changeState();
                    m_controller.RPC(this, "changeState");
                    GameObject.Find("DoorTrigger").SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        private void fight(int id)
        {
            //点击按钮
            if (state == ElevatorState.FindingButton && !Door.state)
            {
                changeState();
                m_controller.RPC(this, "changeState");
            }
        }

        public void changeState()
        {
            switch (state)
            {
                case ElevatorState.Initing:
                    state = ElevatorState.FindingButton;
                    break;

                case ElevatorState.FindingButton:
                    state = ElevatorState.Start_Fighting;
                    break;

                case ElevatorState.End:
                    state = ElevatorState.Escape;
                    break;
            }
        }
    }
}
