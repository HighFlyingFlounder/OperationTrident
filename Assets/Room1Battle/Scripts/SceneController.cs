using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;
using OperationTrident.Util;
using System;
using UnityEngine.Playables;
using OperationTrident.Common.AI;

namespace OperationTrident.Room1
{

    public class SceneController : MonoBehaviour, NetSyncInterface
    {
        //private new Camera camera;

        NetSyncController m_controller;

        // 钥匙预设
        [SerializeField]
        private GameObject keyPrefab;
        // SubRoom3必需品的预设(现在仔细想想，好像并不需要预设！！！！)
        //[SerializeField]
        //private GameObject CropsePrefab;
        // ID卡的预设
        [SerializeField]
        private GameObject IDCardPrefab;
        [SerializeField]
        private GameObject DoorPrefab;

        // 场景中可交互物品的数组
        private GameObject[] gameObjects;
        // 场景中当前出现的可交互物品
        private GameObject gameObject;

        public GameObject key1;
        private GameObject key2;
        // 这里的尸体指的是尸体上的C4，尸体是一直都在的,这里有个问题！@Question: 可以不用把这个C4显示出来的
        public GameObject corpse;
        private GameObject IDCard;

        // 三个门
        public GameObject doorStart;
        private GameObject door1;
        private GameObject door2;

        // 场景中一些物品出现的位置
        public Transform Key2Transform;

        public Transform IDCardTransform;

        //[SerializeField]
        //private Vector3 DoorStartPosition; // = new Vector3(-7.04771f, 1.345f, 25.848f);

        public Transform door1Transform;
        public Transform door2Transform;

        [NonSerialized]
        public static Vector3 escapePosition;
        [SerializeField]
        private GameObject escapeGameObject;

        public static Vector3 Key1WorldPosition;
        public static Vector3 Key2WorldPosition;
        public static Vector3 CropseWorldPosition;
        public static Vector3 IDCardWorldPosition;

        [SerializeField]
        private PlayableDirector elevator;

        // 场景中的物体数（指的是可交互的物品）
        private const int gameObjectCount = 7;
        /*
            房间内会有防守机器人，进入后会启动预设机器人，玩家需要尽可能击毁机器人。
            房间共有3个ID卡地点，玩家需要每个地点都走一遍，找出可用的ID卡。在第三个ID位置前会有一个必需要拿的物体，玩家需先过去get到才能开门。
            任务目标显示为SubROOM1，玩家需前往寻找钥匙。
            过了SubROOM1后，任务目标更新为SubROOM2，玩家需前往寻找钥匙。
            两个SubROOM都过了之后，任务目标更新为SubROOM3前的必需品。
            取得必需品，任务目标更新为开门。进入SubROOM3后取得OK的ID卡。
        */
        // 场景的状态。
        public enum Room1State { Initing, FindingKey1, FindingKey2, TryingToOpenRoom, FindingNeeded, FindingIDCard, EscapingRoom};

        public static Room1State state;

        [SerializeField]
        WanderAIAgentInitParams[] _wanderAIAgentInitParams;

        private void Awake()
        {
            // 增加第一个key的侦听器
            Messenger<int>.AddListener(GameEvent.KEY_GOT, OnKeyGot);
            // 增加第一个door的侦听器
            Messenger<int>.AddListener(GameEvent.DOOR_OPEN, OnDoorOpen);
            // 增加尸体的侦听器
            Messenger.AddListener(GameEvent.CROPSE_TRY, OnCropseTry);

            Messenger.AddListener(GameEvent.ELEVATOR_OPEN, OnElevatorOpen);
        }

        private void OnElevatorOpen()
        {
            if(state==Room1State.EscapingRoom) elevator.Play();
        }

        private void Destroy()
        {
            // 删除第一个key的侦听器
            Messenger<int>.RemoveListener(GameEvent.KEY_GOT, OnKeyGot);
            // 删除第一个door的侦听器
            Messenger<int>.RemoveListener(GameEvent.DOOR_OPEN, OnDoorOpen);
            // 删除尸体的侦听器
            Messenger.RemoveListener(GameEvent.CROPSE_TRY, OnCropseTry);

            Messenger.RemoveListener(GameEvent.ELEVATOR_OPEN, OnElevatorOpen);
        }

        // Use this for initialization
        void Start()
        {
            // 场景状态初始
            state = Room1State.Initing;
            gameObjects = new GameObject[gameObjectCount];
            elevator.playOnAwake = false;
            //camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            //enemysList = new List<GameObject>();
            AIController.instance.CreateAI(2, 0, "AIborn1", _wanderAIAgentInitParams[0]);
            AIController.instance.CreateAI(2, 0, "AIborn1", _wanderAIAgentInitParams[1]);
            AIController.instance.CreateAI(2, 0, "AIborn1", _wanderAIAgentInitParams[2]);
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                // 场景的初始状态
                case Room1State.Initing:
                    InitAllGameObject();
                    subtitlesToDisplay = subtitleInitToDisplay;
                    Key1WorldPosition = key1.transform.position;
                    Key2WorldPosition = key2.transform.position;
                    CropseWorldPosition = corpse.transform.position;
                    IDCardWorldPosition = IDCard.transform.position;
                    //StartCoroutine(EnemyCreateRountine());
                    state = Room1State.FindingKey1;
                    
                    break;
                // 玩家正在找第一个钥匙
                case Room1State.FindingKey1:

                    break;
                // 玩家正在找第二个钥匙
                case Room1State.FindingKey2:

                    break;
                // 玩家正准备尝试开最后一个门
                case Room1State.TryingToOpenRoom:
                    
                    break;
                // 玩家正在找必需品
                case Room1State.FindingNeeded:
                    subtitlesToDisplay = subtitleOpenDoorFalseToDisplay;
                    break;
                // 玩家正在找ID卡
                case Room1State.FindingIDCard:
                    
                    break;
                // 玩家正在逃离房间
                case Room1State.EscapingRoom:
                    subtitlesToDisplay = subtitleEscapingToDisplay;
                    break;
            }
        }

        IEnumerator AddSubtitleIndex(float delay)
        {
            yield return new WaitForSeconds(delay);
            ++subtitleIndex;
        }

        IEnumerator SetSubtitleIndex(float delay,int index)
        {
            yield return new WaitForSeconds(delay);
            subtitleIndex = index;
        }

        // 一次性生成了全部的GameObject，但是这里并没有生成C4？
        private void InitAllGameObject()
        {
            //key1 = Instantiate(keyPrefab) as GameObject;
            //key1.transform.localPosition = Key1Position;

            key2 = Instantiate(IDCardPrefab) as GameObject;
            Util.SetParent(key2, Key2Transform);

            IDCard = Instantiate(IDCardPrefab) as GameObject;
            Util.SetParent(IDCard, IDCardTransform);

            //doorStart = Instantiate(DoorPrefab) as GameObject;
            //doorStart.transform.localPosition = DoorStartPosition;
            //doorStart.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
            //doorStart.transform.localScale = new Vector3(1.6f, 2.5f, 0.2f);

            door1 = Instantiate(DoorPrefab) as GameObject;
            Util.SetParent(door1, door1Transform);

            door2 = Instantiate(DoorPrefab) as GameObject;
            Util.SetParent(door2, door2Transform);

            escapeGameObject.SetActive(false);

        }

        private void OnKeyGot(int id)
        {
            OnKeyGot_Imp(id);
            m_controller.RPC(this, "OnKeyGot_Imp", id);
        }

        // 改进后的函数，所有钥匙的事件分ID处理
        public void OnKeyGot_Imp(int id)
        {
            Debug.Log("id: "+id+" state: "+state);
            switch (id)
            {
                // 第一个钥匙
                case 0:
                    if (state == Room1State.FindingKey1)
                    {
                        Destroy(key1);
                        state = Room1State.FindingKey2;
                        AIController.instance.CreateAI(2, 0, "AIborn2", _wanderAIAgentInitParams[3]);
                        AIController.instance.CreateAI(2, 0, "AIborn2", _wanderAIAgentInitParams[4]);
                        AIController.instance.CreateAI(2, 0, "AIborn2", _wanderAIAgentInitParams[5]);
                    }
                    break;
                // 第二个钥匙
                case 1:
                    if (state == Room1State.FindingKey2)
                    {
                        Destroy(key2);
                        state = Room1State.TryingToOpenRoom;
                        AIController.instance.CreateAI(2, 0, "AIborn3", _wanderAIAgentInitParams[6]);
                        AIController.instance.CreateAI(2, 0, "AIborn3", _wanderAIAgentInitParams[7]);
                        AIController.instance.CreateAI(2, 0, "AIborn3", _wanderAIAgentInitParams[8]);

                    }
                    break;
                // ID卡（也当成钥匙了）
                case 2:
                    if (state == Room1State.FindingIDCard)
                    {
                        Destroy(IDCard);
                        state = Room1State.EscapingRoom;
                        escapeGameObject.SetActive(true);
                        escapePosition = escapeGameObject.transform.position;
                        AIController.instance.CreateAI(2, 0, "AIborn1", _wanderAIAgentInitParams[0]);
                        AIController.instance.CreateAI(2, 0, "AIborn1", _wanderAIAgentInitParams[1]);
                        AIController.instance.CreateAI(2, 0, "AIborn1", _wanderAIAgentInitParams[2]);
                        AIController.instance.CreateAI(2, 0, "AIborn2", _wanderAIAgentInitParams[3]);
                        AIController.instance.CreateAI(2, 0, "AIborn2", _wanderAIAgentInitParams[4]);
                        AIController.instance.CreateAI(2, 0, "AIborn2", _wanderAIAgentInitParams[5]);
                        AIController.instance.CreateAI(2, 0, "AIborn3", _wanderAIAgentInitParams[6]);
                        AIController.instance.CreateAI(2, 0, "AIborn3", _wanderAIAgentInitParams[7]);
                        AIController.instance.CreateAI(2, 0, "AIborn3", _wanderAIAgentInitParams[8]);
                        AIController.instance.CreateAI(2, 0, "AIborn4", _wanderAIAgentInitParams[9]);
                        AIController.instance.CreateAI(2, 0, "AIborn4", _wanderAIAgentInitParams[10]);
                        AIController.instance.CreateAI(2, 0, "AIborn4", _wanderAIAgentInitParams[11]);
                    }
                    break;
            }
        }

        private void OnDoorOpen(int id)
        {
            OnDoorOpen_Imp(id);
            m_controller.RPC(this, "OnDoorOpen_Imp", id);
        }

        public GameObject C4Door;

        // 改进后的函数，所有门的事件分ID处理
        public void OnDoorOpen_Imp(int id)
        {
            
            switch (id)
            {
                // 第一扇门
                case 0:
                    if (state == Room1State.FindingKey1)
                    {
                        Debug.Log("WindyIce");
                        doorStart.GetComponent<DoorScript>().OpenAndDestroy(10.0f,DoorScript.DoorOpenDirection.ZNegative);
                        doorStart.GetComponent<HintableObject>().DestroyThis();
                    }
                    break;
                // 第二扇门
                case 1:
                    if (state == Room1State.FindingKey2)
                    {
                        door1.GetComponent<DoorScript>().OpenAndDestroy(10.0f,DoorScript.DoorOpenDirection.XPositive);
                        door1.GetComponent<HintableObject>().DestroyThis();
                    }
                    break;
                // 第三扇门
                case 2:
                    if (state == Room1State.TryingToOpenRoom)
                    {
                        // TODO： 玩家打开门失败
                        state = Room1State.FindingNeeded;
                    }
                    else if (state == Room1State.FindingIDCard)
                    {
                        C4Door.SetActive(true);
                        StartCoroutine(WaitForExplosion(9.0f));
                    }
                    break;
            }
        }



        IEnumerator WaitForExplosion(float t)
        {

            yield return new WaitForSeconds(t);
            door2.GetComponent<DoorScript>().CreateFragmentsInFloor(270, true);
            door2.GetComponent<HintableObject>().DestroyThis();
            C4Door.SetActive(false);
            Destroy(door2);
        }

        private void OnCropseTry()
        {
            OnCorpseTry_Imp();
            m_controller.RPC(this, "OnCorpseTry_Imp");
        }



        // 搜刮尸体
        public void OnCorpseTry_Imp()
        {
            if (state == Room1State.FindingNeeded)
            {
                // TODO: 拿到了尸体C4
                state = Room1State.FindingIDCard;
                Destroy(corpse);
                
            }
        }

        public string[] subtitleInitToDisplay = {
            "^b地球指挥官:^w根据情报显示，开启电源室入口的^y智能感应芯片^w在仓库里的几个可能位置",
            "^b地球指挥官:^w你们要拿到它，小心里面的^r巡逻机器人"
        };

        public string[] subtitleOpenDoorFalseToDisplay={
            "^rAI:^w对不起，身份识别错误",
            "^b队友:^w门居然打不开，来点硬一点的方法吧"
        };

        public string[] subtitleEscapingToDisplay = {
            "^b地球指挥官:^w注意，仓库的警报启动",
            "^b地球指挥官:^w大量的^r防御机器人^w正在进入你们的房间"
        };
        private string[] subtitlesToDisplay;
        private int subtitleIndex = 0;
        void OnGUI()
        {
            //GUIUtil.DisplaySubtitlesInGivenGrammar(
            //    subtitlesToDisplay,
            //    Util.GetCamera(),
            //    fontSize: 16,
            //    subtitleRatioHeight: 0.9f,
            //    secondOfEachWord: 0.2f,
            //    secondBetweenLine: 4.0f);

            //string[] toD = {
            //    "^w你好，^r一王^w，我是^b WindyIce",
            //    "^w你好，^r鸡王^w，我是^b WindyIce" ,
            //    "^w我们都要取回^b托卡马克之心" };
            //float[] a1 = { 10.0f, 5.0f, 10.0f };
            //float[] a2 = { 5.0f, 10.0f, 5.0f };
            //GUIUtil.DisplaySubtitlesInGivenGrammarWithTimeStamp(
            //    toD,
            //    Util.GetCamera(),
            //    fontSize: 16,
            //    subtitleRatioHeight: 0.9f,
            //    secondsOfEachLine: a1,
            //    secondBetweenLine: a2);
        }

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