using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;

namespace OperationTrident.Elevator {
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        //敌人出生点
        public List<GameObject> EnemyPos = new List<GameObject>();
        //电梯开关的预设
        public GameObject buttonPre;
        //button的位置
        public Vector3 ButtonPosition;
        //持续时间
        public int time;

        //状态
        private enum ElevatorState { Initing, FindingButton, Start_Fighting ,Fighting, End };
        private ElevatorState state;

        //开始战斗的时间
        private float s_time;

        //按钮
        private GameObject button;

        // Use this for initialization
        void Start()
        {
            state = ElevatorState.Initing;
            time = 15;
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case ElevatorState.Initing:
                    //初始化BUTTON
                    button = GameObject.Instantiate(buttonPre);
                    button.transform.localPosition = ButtonPosition;

                    state = ElevatorState.FindingButton;
                    break;

                case ElevatorState.FindingButton:
                    break;

                case ElevatorState.Start_Fighting:
                    Messenger<int>.Broadcast(GameEvent.Enemy_Start, 0);
                    state = ElevatorState.Fighting;
                    s_time = System.DateTime.Now.Second;
                    break;
                case ElevatorState.Fighting:
                    if (System.DateTime.Now.Second - s_time >= time)
                    {
                        state = ElevatorState.End;
                    }
                    break;
                case ElevatorState.End:
                    Messenger<int>.Broadcast(GameEvent.End, 0);
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


        private void Operate(int id)
        {

            if (state == ElevatorState.FindingButton)
            {
                button.SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                state = ElevatorState.Start_Fighting;
            }
        }


    }
}
