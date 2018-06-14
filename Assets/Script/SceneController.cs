using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Elevator;

namespace OperationTrident.Elevator {
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        public GameObject EnemyPosPre;
        //出生点的位置
        public List<Vector3> ePos = new List<Vector3>();
        //电梯开关的预设
        public GameObject buttonPre;
        //button的位置
        public Vector3 ButtonPosition;


        private enum ElevatorState { Initing, FindingButton, Fighting, End };
        private GameObject button;
        private ElevatorState state;
        //敌人出生点
        private List<GameObject> EnemyPos;

        //private void Awake()
        //{
        //    Messenger<int>.AddListener(GameEvent.BUTTON_GOT, OnButtonGot);
        //}

        //private void Destroy()
        //{
        //    Messenger<int>.RemoveListener(GameEvent.BUTTON_GOT, OnButtonGot);
        //}

        // Use this for initialization
        void Start()
        {
            state = ElevatorState.Initing;
            EnemyPos = new List<GameObject>();
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

                    for (int i = 0; i < ePos.Count; i++)
                    {
                        GameObject temp = GameObject.Instantiate(EnemyPosPre);
                        temp.transform.localPosition = ePos[i];
                        EnemyPos.Add(temp);
                    }

                    state = ElevatorState.FindingButton;
                    break;

                case ElevatorState.FindingButton:
                    break;

                case ElevatorState.Fighting:
                    foreach (GameObject enemy in EnemyPos)
                    {
                        enemy.SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                    }
                    state = ElevatorState.End;
                    break;
                case ElevatorState.End:
                    break;
            }
        }


        private void Operate()
        {

            if (state == ElevatorState.FindingButton)
            {
                button.SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                state = ElevatorState.Fighting;
            }
        }


    }
}
