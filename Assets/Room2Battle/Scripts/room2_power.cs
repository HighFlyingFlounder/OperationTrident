using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

using OperationTrident.FPS.Common;
using OperationTrident.Common.AI;
using OperationTrident.Common;

namespace room2Battle
{
    //能源房大战
    public class room2_power : Subscene, NetSyncInterface
    {
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
        
        //开关位置
        [SerializeField]
        protected Transform switchPos;

        //二楼
        [SerializeField]
        protected Transform secondFloor;
        
        //玩家距离开关距离
        protected float distance = float.MaxValue;

        //台词
        public string[] line =
        {
        };

        [SerializeField]
        protected AudioSource anotherSource;

        [SerializeField]
        protected AudioClip[] clips;

        //AI的参数
        [SerializeField]
        WanderAIAgentInitParams[] wanderAIAgentInitParams;

        [SerializeField]
        TurretAIAgentInitParams[] turrentAIAgentInitParams;

        //bool值保证只生成一次AI
        protected bool initEnemyAgain = false;

        protected float lastTimeInitAI = 0.0f;

        protected bool autoOpenDepthSensor = false;

        /// <summary>
        ///  method 
        /// </summary>

        ///==============================
        //@brief 获取相机句柄,不至于为空
        //===============================
        void Start()
        {
            mCamera = Camera.main;
        }

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

        /// <summary>
        /// @override 
        /// @brief 删除加入的后处理脚本
        /// </summary>
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
        }

        /// <summary>
        /// @brief 联网状态下，获取相机句柄，方便GUI显示
        /// </summary>
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

                //@TODO: 替换成老Y的AI
                Debug.Log("7 enemy");
                AIController.instance.CreateAI(4, 0, "EnemyInitPos4", wanderAIAgentInitParams[1]);
                AIController.instance.CreateAI(3, 0, "EnemyInitPos3", wanderAIAgentInitParams[1]);
            }
            distance = Vector3.Distance(switchPos.position, playerCamera.GetComponent<Transform>().position);
        }

        /// <summary>
        /// @brief 类似消息处理函数
        /// @param i 消息
        /// </summary>
        /// <param name="i"></param>
        public override void notify(int i)
        {
            if (this.enabled)
            {
                //碰撞体消息
                if (i == 1)
                {
                    if (isSwitchOpen)
                    {
                        enterSecondFloor_Room2();
                        gameObject.GetComponent<NetSyncController>().RPC(this, "enterSecondFloor_Room2");
                    }
                }
            }
        }

        /// <summary>
        /// @brief 判断是否看向开关，生成AI
        /// </summary>
        void Update()
        {
            
            if (getCamera != null)
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

                distance = Vector3.Distance(switchPos.position, playerCamera.GetComponent<Transform>().position);
            }
            if (isSwitchOpen)
            {
                if (!initEnemyAgain)
                {
                    Debug.Log("7 enemy");
                    AIController.instance.CreateAI(2, 0, "EnemyInitPos3", wanderAIAgentInitParams[1]);
                    AIController.instance.CreateAI(2, 0, "EnemyInitPos4", wanderAIAgentInitParams[1]);
                    AIController.instance.CreateAI(2, 0, "EnemyInitPos1", wanderAIAgentInitParams[1]);

                    initEnemyAgain = true;
                }
            }

            if (lastTimeInitAI > 13.0f)
            {
                AIController.instance.CreateAI(1, 0, "EnemyInitPos1", wanderAIAgentInitParams[1]);
                lastTimeInitAI = 0.0f;
            }
            else
            {
                lastTimeInitAI += Time.deltaTime;
            }

        }


        /// <summary>
        /// @brief 根据输入控制相机后处理，开开关
        /// </summary>
        void LateUpdate()
        {
            if (getCamera != null)
            {
                //当看到物品时
                if (isFocus)
                {

                    if (Input.GetKey(KeyCode.F))
                    {
                        if (distance <= 5.0f)
                        {
                            switchOn_Room2();
                            gameObject.GetComponent<NetSyncController>().RPC(this, "switchOn_Room2");
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
                //开关被开了
                if (isSwitchOpen)
                {
                    playerCamera.GetComponent<becomeDark>().enabled = false;
                    
                    foreach (GameObject mirror in playerCameraMirror)
                    {
                        mirror.GetComponent<becomeDark>().enabled = false;
                        mirror.GetComponent<depthSensor>().enabled = false;
                    }

                    if (!autoOpenDepthSensor)
                    {
                        playerCamera.GetComponent<depthSensor>().enabled = false;

                        foreach (GameObject mirror in playerCameraMirror)
                        {
                            mirror.GetComponent<depthSensor>().enabled = false;
                        }
                        anotherSource.clip = clips[0];
                        anotherSource.Play();
                        autoOpenDepthSensor = true;
                    }
                }
            }
        }

        //TODO： 更新到佩炜的GUI
        void OnGUI()
        {
            if (mCamera != null)
            {
                if (isFocus)
                {
                    float posX = mCamera.pixelWidth / 2 - 50;
                    float posY = mCamera.pixelHeight / 2 - 50;
                    //交互提示
                    if (!isSwitchOpen)
                    {
                        if (distance <= 5.0f)
                            GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yF^w与物品交互", mCamera, 12, 0.8f);
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
                       GUIUtil.whiteColor,
                       0.5f, 0.1f, 16);
                    if (!open2 && open)
                    {
                        GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yH^w开启/关闭探测器", mCamera, 12, 0.7f);
                    }
                    //目标位置
                    GUIUtil.DisplayMissionPoint(switchPos.position, mCamera, Color.white);

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
                           GUIUtil.whiteColor,
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

        }

        public void RecvData(SyncData data)
        {
        }

        public SyncData SendData()
        {
            return null;
        }

        public void Init(NetSyncController controller)
        {

        }

        public void switchOn_Room2()
        {
            isSwitchOpen = true;
        }

        public void enterSecondFloor_Room2()
        {
            isIntoSecondFloor = true;
        }
    }
}
