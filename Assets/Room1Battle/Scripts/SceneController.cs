using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.EventSystem;
using System;

namespace OperationTrident.Room1
{

    public class SceneController : MonoBehaviour
    {
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

        private GameObject key1;
        private GameObject key2;
        // 这里的尸体指的是尸体上的C4，尸体是一直都在的,这里有个问题！@Question: 可以不用把这个C4显示出来的
        public GameObject cropse;
        private GameObject IDCard;

        // 三个门
        private GameObject doorStart;
        private GameObject door1;
        private GameObject door2;

        // 场景中一些物品出现的位置
        [SerializeField]
        private Vector3 Key1Position; //= new Vector3(-15.842f, 1.075f, 29.765f);
        [SerializeField]
        private Vector3 Key2Position; //= new Vector3(-52.58f, 1.075f, -2.91f);
                                      // public Vector3 CropsePosition;  暂时不需要这个了
        [SerializeField]
        private Vector3 IDCardPosition; // = new Vector3(-34.961f, 1.32f, 76.99f);

        [SerializeField]
        private Vector3 DoorStartPosition; // = new Vector3(-7.04771f, 1.345f, 25.848f);
        [SerializeField]
        private Vector3 Door1Position; // = new Vector3(-58.706f, 1.394f, 9.44f); 开门的话向X轴负半轴移动
        [SerializeField]
        private Vector3 Door2Position; // = new Vectir3(-58.12f, 2.05f, 74.87f); 初始化时Scale的x值要变成3. 开门也是向x轴负半轴移动

        public static Vector3 Key1WorldPosition;
        public static Vector3 Key2WorldPosition;
        public static Vector3 CropseWorldPosition;
        public static Vector3 IDCardWorldPosition;

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

        private void Awake()
        {
            // 增加第一个key的侦听器
            Messenger<int>.AddListener(GameEvent.KEY_GOT, OnKeyGot);
            // 增加第一个door的侦听器
            Messenger<int>.AddListener(GameEvent.DOOR_OPEN, OnDoorOpen);
            // 增加尸体的侦听器
            Messenger.AddListener(GameEvent.CROPSE_TRY, OnCropseTry);
        }

        private void Destroy()
        {
            // 删除第一个key的侦听器
            Messenger<int>.RemoveListener(GameEvent.KEY_GOT, OnKeyGot);
            // 删除第一个door的侦听器
            Messenger<int>.RemoveListener(GameEvent.DOOR_OPEN, OnDoorOpen);
            // 删除尸体的侦听器
            Messenger.RemoveListener(GameEvent.CROPSE_TRY, OnCropseTry);
        }

        // Use this for initialization
        void Start()
        {
            // 场景状态初始
            state = Room1State.Initing;
            gameObjects = new GameObject[gameObjectCount];

            //enemysList = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                // 场景的初始状态
                case Room1State.Initing:
                    InitAllGameObject();
                    Key1WorldPosition = key1.transform.position;
                    Key2WorldPosition = key2.transform.position;
                    CropseWorldPosition = cropse.transform.position;
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

                    break;
                // 玩家正在找ID卡
                case Room1State.FindingIDCard:

                    break;
                // 玩家正在逃离房间
                case Room1State.EscapingRoom:

                    break;
            }
        }

        // 一次性生成了全部的GameObject，但是这里并没有生成C4？
        private void InitAllGameObject()
        {
            key1 = Instantiate(keyPrefab) as GameObject;
            key1.transform.localPosition = Key1Position;

            key2 = Instantiate(keyPrefab) as GameObject;
            key2.transform.localPosition = Key2Position;

            IDCard = Instantiate(IDCardPrefab) as GameObject;
            IDCard.transform.localPosition = IDCardPosition;

            doorStart = Instantiate(DoorPrefab) as GameObject;
            doorStart.transform.localPosition = DoorStartPosition;
            doorStart.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
            doorStart.transform.localScale = new Vector3(1.6f, 2.5f, 0.2f);

            door1 = Instantiate(DoorPrefab) as GameObject;
            door1.transform.localPosition = Door1Position;

            door2 = Instantiate(DoorPrefab) as GameObject;
            door2.transform.localPosition = Door2Position;
            door2.transform.localScale = new Vector3(3.0f, door2.transform.localScale.y, door2.transform.localScale.z);
        }

        // 改进后的函数，所有钥匙的事件分ID处理
        private void OnKeyGot(int id)
        {
            switch (id)
            {
                // 第一个钥匙
                case 0:
                    if (state == Room1State.FindingKey1)
                    {
                        Destroy(key1);
                        state = Room1State.FindingKey2;
                    }
                    break;
                // 第二个钥匙
                case 1:
                    if (state == Room1State.FindingKey2)
                    {
                        Destroy(key2);
                        state = Room1State.TryingToOpenRoom;
                    }
                    break;
                // ID卡（也当成钥匙了）
                case 2:
                    if (state == Room1State.FindingIDCard)
                    {
                        Destroy(IDCard);
                        state = Room1State.EscapingRoom;
                    }
                    break;
            }
        }

        // 改进后的函数，所有门的事件分ID处理
        private void OnDoorOpen(int id)
        {
            switch (id)
            {
                // 第一扇门
                case 0:
                    if (state == Room1State.FindingKey1)
                    {
                        doorStart.GetComponent<DoorScript>().OpenAndDestroy(2.5f,DoorScript.DoorOpenDirection.ZPositive);
                    }
                    break;
                // 第二扇门
                case 1:
                    if (state == Room1State.FindingKey2)
                    {
                        door1.GetComponent<DoorScript>().OpenAndDestroy(3.0f,DoorScript.DoorOpenDirection.XNegative);
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
                        door2.GetComponent<DoorScript>().OpenAndDestroy(5.0f,DoorScript.DoorOpenDirection.XNegative);
                    }
                    break;
            }
        }

        // 搜刮尸体
        private void OnCropseTry()
        {
            if (state == Room1State.FindingNeeded)
            {
                // TODO: 拿到了尸体C4
                state = Room1State.FindingIDCard;
                cropse.GetComponent<InteractiveThing>().enabled = false;
            }
        }



        //// 第一个key拿到了！
        //private void OnKey1Got()
        //{
        //    if (state == Room1State.FindingKey1)
        //    {
        //        Destroy(key1);
        //        state = Room1State.FindingKey2;
        //    }
        //}

        //// 第二个key拿到了
        //private void OnKey2Got()
        //{
        //    if (state == Room1State.FindingKey2)
        //    {
        //        Destroy(key2);
        //        state = Room1State.TryingToOpenRoom;
        //    }
        //}

        //// 尝试把门1打开
        //private void OnDoor1Open()
        //{
        //    if (state == Room1State.FindingKey2)
        //    {
        //        door1.GetComponent<DoorScript>().OpenAndDestroy();
        //    }
        //}
    }
}