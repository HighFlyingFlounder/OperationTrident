using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Elevator {
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        //电梯开关的预设
        public GameObject buttonPre;
        //持续时间
        public int d_time = 60;

        //状态
        public enum ElevatorState { Initing, FindingButton, Start_Fighting ,Fighting, End };
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

        //碰撞次数（进入为偶数）
        private int count = 0;

        // Use this for initialization
        void Start()
        {
            //初始化BUTTON
            button = GameObject.Instantiate(buttonPre);
            button.transform.localPosition = new Vector3(0, 2, 0);
            ButtonPosition = button.transform.localPosition;
            state = ElevatorState.Initing;
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
                    break;

                case ElevatorState.Fighting:
                    c_time += Time.deltaTime;
                    if(c_time >= e_time)
                    {
                        state = ElevatorState.End;
                    }

                    break;
                case ElevatorState.End:
                    Messenger<int>.Broadcast(GameEvent.End, 0);
                    GameObject.Find("DoorTrigger").SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                    break;
            }
        }

        private void Awake()
        {
            Messenger<int>.AddListener(GameEvent.Push_Button, Operate);
        }

        private void Destroy()
        {
            Messenger<int>.RemoveListener(GameEvent.Push_Button, Operate);
        }

        void OnTriggerEnter(Collider other)
        {
            //进入关门
            count++;
            if(count % 2 == 0 && state == ElevatorState.Initing)
            {
                state = ElevatorState.FindingButton;
                GameObject.Find("DoorTrigger").SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
            }
        }

        private void Operate(int id)
        {
            //点击按钮
            if (state == ElevatorState.FindingButton && !Door.state)
            {
                state = ElevatorState.Start_Fighting;
            }
        }


    }
}
