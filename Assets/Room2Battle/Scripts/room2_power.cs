﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

using OperationTrident.FPS.Common;

namespace room2Battle
{
    //能源房大战
    public class room2_power : Subscene, NetSyncInterface
    {
        //敌人预设
        [SerializeField]
        protected GameObject enemyPrefabs;
        //敌人列表方便管理
        protected ArrayList enemyList = new ArrayList();
        //当前敌人数目，用于补充敌人数目
        protected int currentEnemyNum = 0;
        //最多敌人数目
        protected int maxEnemyNum = 20;
        //敌人出生点
        [SerializeField]
        protected Transform[] enemyInitPositions;

        //钥匙
        [SerializeField]
        protected GameObject key;

        //主相机，需要判断是否拿到钥匙
        protected Camera mCamera;

        //是否对准开关
        protected bool isFocus = false;

        //是否开始计时
        //protected bool startTiming = false;
        //是否打开开关
        protected bool isSwitchOpen = false;
        //是否进入2楼
        protected bool isIntoSecondFloor = false;
        //是否打开夜视仪
        protected bool isOpenDepthSensor = false;
        //挂载相机的对象，联网可用
        protected GameObject playerCamera = null;
        //倍镜
        protected List<GameObject> playerCameraMirror = new List<GameObject>();
        //获取camera
        protected GetCamera getCamera;

        //挂载相机的对象，单机可用
        //[SerializeField]
        //protected GameObject player;

        [SerializeField]
        protected Transform switchPos;

        [SerializeField]
        protected Transform secondFloor;

        protected bool isShowTarget = false;

        public string[] line =
        {
        };

        //获取相机句柄
        void Start()
        {
            mCamera = Camera.main;
        }

        protected float distance = float.MaxValue;

        //@override
        public override bool isTransitionTriggered()
        {
            return isIntoSecondFloor && isSwitchOpen;
        }

        //@override
        public override string GetNextSubscene()
        {
            return "room2_battle";
        }

        public override void onSubsceneDestory()
        {
            playerCamera.GetComponent<depthSensor>().enabled = false;
            playerCamera.GetComponent<becomeDark>().enabled = false;


            Destroy(playerCamera.GetComponent<depthSensor>());
            Destroy(playerCamera.GetComponent<becomeDark>());

            foreach (GameObject cam in getCamera.MirrorCameras)
            {
                Destroy(cam.GetComponent<depthSensor>());
                Destroy(cam.GetComponent<becomeDark>());
            }

            //@TODO: 替换成老Y的AI
            foreach (GameObject obj in enemyList)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }

        public override void onSubsceneInit()
        {
            //RenderSettings.ambientIntensity = 0.1f;
            //添加脚本
            if (GameMgr.instance)//联网状态
            {
                getCamera = (SceneNetManager.instance.list[GameMgr.instance.id]).GetComponent<GetCamera>();
                playerCamera = getCamera.MainCamera;
                foreach (GameObject cam in getCamera.MirrorCameras)
                {
                    playerCameraMirror.Add(cam);
                }
            }

            //@TODO: 替换成老Y的AI
            for (int i = 0; i < maxEnemyNum; ++i)
            {
                GameObject obj = Instantiate(enemyPrefabs, enemyInitPositions[Random.Range(0, enemyInitPositions.Length)].position, Quaternion.identity);
                enemyList.Add(obj);
            }

            distance = Vector3.Distance(switchPos.position, playerCamera.GetComponent<Transform>().position);
        }

        public override void notify(int i)
        {
            //碰撞体消息
            if (i == 1)
            {
                if (isSwitchOpen)
                {
                    isIntoSecondFloor = true;
                    Debug.Log("player into floor2");
                    gameObject.GetComponent<NetSyncController>().SyncVariables();
                }
            }
        }

        void Update()
        {
            mCamera = getCamera.GetCurrentUsedCamera();

            Vector3 point = new Vector3(mCamera.pixelWidth / 2, mCamera.pixelHeight / 2, 0);

            Ray ray = mCamera.ScreenPointToRay(point);

            //通过摄像机，射向对应物体，判断标签
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //获取物体
                GameObject obj = hit.transform.gameObject;
                //判断标签
                if (obj.tag == "switch")
                {
                    {
                        isFocus = true;
                    }
                }
                else
                {
                    isFocus = false;

                }
            }
        }

        void LateUpdate()
        {
            //当看到物品时
            if (isFocus)
            {

                if (Input.GetKey(KeyCode.F))
                {
                    if (distance <= 5.0f)
                    {
                        isSwitchOpen = true;
                        gameObject.GetComponent<NetSyncController>().SyncVariables();
                    }
                }
            }
            //按H打开夜视仪
            if (Input.GetKeyDown(KeyCode.H))
            {
                //通过只有一个后处理，减少post processing的pass
                if (!isOpenDepthSensor)
                {
                    playerCamera.GetComponent<depthSensor>().enabled = true;
                    playerCamera.GetComponent<becomeDark>().enabled = false;

                    foreach (GameObject mirror in playerCameraMirror)
                    {
                        mirror.GetComponent<depthSensor>().enabled = true;
                        mirror.GetComponent<becomeDark>().enabled = false;
                    }

                    isOpenDepthSensor = true;
                }
                else
                {
                    if (!isSwitchOpen)
                    {
                        playerCamera.GetComponent<becomeDark>().enabled = true;

                        foreach (GameObject mirror in playerCameraMirror)
                        {
                            mirror.GetComponent<becomeDark>().enabled = true;
                        }
                    }
                    playerCamera.GetComponent<depthSensor>().enabled = false;
                    foreach (GameObject mirror in playerCameraMirror)
                    {
                        mirror.GetComponent<depthSensor>().enabled = false;
                    }
                    isOpenDepthSensor = false;
                }
            }

            if (isSwitchOpen)
            {
                playerCamera.GetComponent<becomeDark>().enabled = false;
                foreach (GameObject mirror in playerCameraMirror)
                {
                    mirror.GetComponent<becomeDark>().enabled = false;
                }
            }

        }

        //TODO： 更新到佩炜的GUI
        void OnGUI()
        {
            if (isFocus)
            {
                float posX = mCamera.pixelWidth / 2 - 50;
                float posY = mCamera.pixelHeight / 2 - 50;
                //交互提示
                if (!isSwitchOpen)
                {
                    GUIUtil.DisplaySubtitleInGivenGrammar("^w打开这该死的照明电源", mCamera, 16, 0.9f);
                    GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yF^w与物品交互", mCamera, 12, 0.7f);
                }
            }
            //深度摄像头是否开启，是否黑
            bool open = playerCamera.GetComponent<becomeDark>().enabled;
            bool open2 = playerCamera.GetComponent<depthSensor>().enabled;
            //没开灯
            if (!isSwitchOpen)
            {
                //任务目标
                GUIUtil.DisplayMissionTargetInMessSequently("清除附近敌人，打通到电源室的道路！",
                   mCamera,
                   GUIUtil.yellowColor,
                   0.5f, 0.1f, 16);
                if (!open2 && open)
                {
                    GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yH^w开启/关闭探测器", mCamera, 12, 0.7f);
                }
                //目标位置
                GUIUtil.DisplayMissionPoint(switchPos.position, mCamera,Color.white);
                
            }
            else
            {
                //杀入2楼
                if (!isIntoSecondFloor)
                {
                    //台词
                    //GUIUtil.DisplaySubtitlesInGivenGrammar(line, mCamera, 16,0.9f,0.2f,1.2f);
                    //任务目标
                    GUIUtil.DisplayMissionTargetInMessSequently("挺进2楼！",
                       mCamera,
                       GUIUtil.yellowColor,
                       0.5f, 0.1f, 16);
                    //任务目标位置
                    GUIUtil.DisplayMissionPoint(secondFloor.position, mCamera, Color.white);
                }
                if (open2 && !isFocus)
                {
                    GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yH^w关闭探测器", mCamera, 12, 0.7f);
                }
            }


        }

        public void RecvData(SyncData data)
        {
            isSwitchOpen = (bool)data.Get(typeof(bool));
            isIntoSecondFloor = (bool)data.Get(typeof(bool));
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            data.Add(isSwitchOpen);
            data.Add(isIntoSecondFloor);
            return data;
        }

        public void Init(NetSyncController controller)
        {

        }
    }
}
