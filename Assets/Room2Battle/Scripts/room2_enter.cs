using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

using OperationTrident.FPS.Common;
using OperationTrident.Common.AI;

namespace room2Battle
{
    //==================================================
    //================   走出电梯   ====================
    //=================================================

    public class room2_enter : Subscene, NetSyncInterface
    {
        //网络同步
        NetSyncController m_controller;
        //相机节点
        protected GameObject playerCamera = null;
        //倍镜
        protected List<GameObject> playerCameraMirror = new List<GameObject>();
        //
        protected GetCamera getCamera;

        //[SerializeField]
        //protected GameObject player;

        //挂载脚本的shader，包括dark和depth sensor
        [SerializeField]
        protected Shader shader_dark = null;

        [SerializeField]
        protected Shader shader_depthSensor = null;

        //depthSensor的纹理
        [SerializeField]
        protected Texture waveTexture = null;

        [SerializeField]
        protected Texture waveMaskTexture = null;
        //关灯否
        protected bool isNear = false;
        //进入房间否
        protected bool isEnter = false;
        //当前相机
        Camera mCamera;
        //任务详细
        public string[] missionDetails =
            {
            "行动代号：三叉戟",
            "2048年8月1日，中华人民共和国 建军节",
            "外太空****空域"
        };
        //台词
        public string[] line =
        {
        };

        public float wordTransparentInterval = 0.005f; // 字变得更加透明的周期
        public float wordAppearanceInterval = 0.1f; // 每行字一个个出现的速度
        public float lineSubsequentlyInterval = 1.236f; // 每行字一行行出现的速度
        public int fontSize = 16; // 字体大小
        //初始化网络玩家
        protected bool isInit = false;

        //房间开始位置
        [SerializeField]
        protected Transform roomPos;
        //显示目标
        protected bool isShowTarget = false;
        //距离
        protected float distance = float.MaxValue;
        //BGM
        [SerializeField]
        protected AudioSource source;
        //台词
        [SerializeField]
        protected AudioSource TimelineSource;
        //台词，bgm
        [SerializeField]
        protected AudioClip[] clips;

        protected bool playOnce = false;

        protected bool playOnce_ = false;
        //AI的参数
        [SerializeField]
        WanderAIAgentInitParams[] wanderAIAgentInitParams;

       // protected bool playOnce = false;


        ///=======================================================
        ///================ method  ==============================
        ///=======================================================

        public override void onSubsceneInit()
        {
            if (GameMgr.instance)
            {
                Debug.Log("3 ENEMY");
                AIController.instance.CreateAI(3, 0, "EnemyInitPos1", wanderAIAgentInitParams[0]);
            }

        }
        //进入房间转power
        public override bool isTransitionTriggered()
        {
            return isEnter;
        }

        public override string GetNextSubscene()
        {
            return "room2_powerroom";
        }

        public override void onSubsceneDestory()
        {

        }
        /// <summary>
        /// @brief 两次碰撞体检测
        /// @param i 碰撞体提供的magic number，相当于注册的事件
        /// </summary>
        /// <param name="i"></param>
        public override void notify(int i)
        {
            if (this.enabled)
            {
                if (i == 1)
                {
                    near_();
                    m_controller.RPC(this, "near_");
                    //初始化
                    becomeDark_();
                    m_controller.RPC(this, "becomeDark_");
                }
                else if (i == 2)
                {
                    enter_();
                    m_controller.RPC(this, "enter_");
                }
            }
        }
        /// <summary>
        /// 显示GUI
        /// </summary>
        void OnGUI()
        {
            //仅当初始化完成
            if (isInit)
            {
                //联网
                if (GameMgr.instance)
                {
                    mCamera = getCamera.GetCurrentUsedCamera();
                }

                //使命召唤风格
                GUIUtil.DisplayMissionDetailDefault(
                                missionDetails,
                                mCamera,
                                GUIUtil.yellowColor,
                                wordTransparentInterval: wordTransparentInterval,
                                wordAppearanceInterval: wordAppearanceInterval,
                                lineSubsequentlyInterval: lineSubsequentlyInterval,
                                fontSize: fontSize);
                //未遭遇
                if (!isNear)
                {
                    GUIUtil.DisplayMissionTargetInMessSequently("突入电源室！",
                        mCamera,
                        GUIUtil.whiteColor,
                        0.5f, 0.1f, 16);
                    GUIUtil.DisplaySubtitleInGivenGrammar("^g地球指挥部^w：你们已经进入了电源室，你们需要开启电源，电源室才能正常运作。", mCamera, 16, 0.9f, 0.5f, 3.0f);
                }
                else//遭遇
                {
                    //深度摄像头是否开启
                    bool open = playerCamera.GetComponent<depthSensor>().enabled;
                    GUIUtil.DisplayMissionTargetInMessSequently("任务变化：开启照明开关！",
                        mCamera,
                        GUIUtil.whiteColor,
                        0.5f, 0.1f, 16);
                    if (!open)
                        GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yH^w开启/关闭探测器", mCamera, 12, 0.7f);

                        GUIUtil.DisplaySubtitlesInGivenGrammar(line, mCamera, 16, 0.9f, 0.2f, 1.5f);
                        playOnce_ = true;
                }
                GUIUtil.DisplayMissionPoint(roomPos.position, mCamera, Color.white);
            }
        }
        //设置状态
        public void near_()
        {
            isNear = true;
        }

        public void enter_()
        {
            isEnter = true;
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
            m_controller = controller;
        }

        /// <summary>
        /// @brief 根据输入决定后处理特效
        /// </summary>
        void LateUpdate()
        {
            //按H打开夜视仪
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (isInit)
                {
                    //通过只有一个后处理，减少post processing的pass
                    if (playerCamera.GetComponent<becomeDark>().enabled)
                    {
                        playerCamera.GetComponent<depthSensor>().enabled = true;
                        playerCamera.GetComponent<becomeDark>().enabled = false;

                        foreach (GameObject mirror in playerCameraMirror)
                        {
                            mirror.GetComponent<depthSensor>().enabled = true;
                            mirror.GetComponent<becomeDark>().enabled = false;
                        }
                    }
                    else
                    {
                        playerCamera.GetComponent<depthSensor>().enabled = false;
                        playerCamera.GetComponent<becomeDark>().enabled = true;

                        foreach (GameObject mirror in playerCameraMirror)
                        {
                            mirror.GetComponent<depthSensor>().enabled = false;
                            mirror.GetComponent<becomeDark>().enabled = true;
                        }

                    }
                }
            }

        }
        /// <summary>
        /// @brief 一直检测是否网络初始化，然后初始化玩家
        /// </summary>
        void Update()
        {
            //到时间播放
            if (isInit)
            {
                mCamera = getCamera.GetCurrentUsedCamera();

                if (isNear)
                {
                    //bool值保证只调用一次
                    if (!playOnce)
                    {
                        //播放台词
                        source.Stop();
                        source.clip = clips[1];
                        source.Play();
                        playOnce = true;
                        //产生AI
                        AIController.instance.CreateAI(4, 0, "EnemyInitPos2", wanderAIAgentInitParams[1]);
                    }
                }
            }
            else//初始化
            {
                //确保是当前场景被enable才初始化
                if (this.enabled)
                {
                    if (GameMgr.instance)//联网状态
                    {              
                        //设置相机
                        if (SceneNetManager.instance.list.Count != 0)
                        {
                            GameObject PLAYER = (SceneNetManager.instance.list[GameMgr.instance.id]);
                            //设置合适大小
                            foreach (var a in (SceneNetManager.instance.list))
                            {
                                GameObject temp = a.Value;
                                temp.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
                            }
                            //加入后处理脚本
                            getCamera = PLAYER.GetComponent<GetCamera>();

                            playerCamera = getCamera.MainCamera;

                            Destroy(PLAYER.GetComponent<Rigidbody>());
                            foreach (GameObject cam in getCamera.MirrorCameras)
                            {
                                playerCameraMirror.Add(cam);
                            }

                            //给腰射相机加特效
                            playerCamera.AddComponent<becomeDark>();
                            playerCamera.AddComponent<depthSensor>();

                            //初始化脚本参数
                            (playerCamera.GetComponent<becomeDark>() as becomeDark).m_Shader = shader_dark;
                            playerCamera.GetComponent<becomeDark>().enabled = false;

                            (playerCamera.GetComponent<depthSensor>() as depthSensor).m_Shader = shader_depthSensor;
                            (playerCamera.GetComponent<depthSensor>() as depthSensor).m_WaveColorTexture = waveTexture;
                            (playerCamera.GetComponent<depthSensor>() as depthSensor).m_WaveMaskTexture = waveMaskTexture;
                            playerCamera.GetComponent<depthSensor>().enabled = false;

                            //给倍镜加特效
                            foreach (GameObject mirror in playerCameraMirror)
                            {
                                mirror.AddComponent<becomeDark>();
                                mirror.AddComponent<depthSensor>();

                                //初始化脚本参数
                                (mirror.GetComponent<becomeDark>() as becomeDark).m_Shader = shader_dark;
                                mirror.GetComponent<becomeDark>().enabled = false;

                                (mirror.GetComponent<depthSensor>() as depthSensor).m_Shader = shader_depthSensor;
                                (mirror.GetComponent<depthSensor>() as depthSensor).m_WaveColorTexture = waveTexture;
                                (mirror.GetComponent<depthSensor>() as depthSensor).m_WaveMaskTexture = waveMaskTexture;
                                mirror.GetComponent<depthSensor>().enabled = false;
                            }

                            isInit = true;
                            //bgm，台词
                            TimelineSource.clip = clips[2];
                            TimelineSource.Play();
                            //台词
                            source.clip = clips[0];
                            source.Play();
                            source.priority = TimelineSource.priority + 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// @brief 关灯
        /// </summary>
        public void becomeDark_()
        {
            if (isInit)
            {
                playerCamera.GetComponent<becomeDark>().enabled = true;

                foreach (GameObject mirror in playerCameraMirror)
                {
                    mirror.GetComponent<becomeDark>().enabled = true;
                }
            }
        }
    }
}
