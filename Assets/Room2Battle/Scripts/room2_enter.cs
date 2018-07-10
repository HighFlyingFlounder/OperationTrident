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
            "2018.6.22  星期五",
            "外太空",
            "三叉戟行动"
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

        [SerializeField]
        WanderAIAgentInitParams[] wanderAIAgentInitParams;


        ///=======================================================
        ///================ method  ==============================
        ///=======================================================

        public override void onSubsceneInit()
        {
            if (GameMgr.instance)
            {
                //AIController.instance.CreateAI(3, 0, "EnemyInitPos1", wanderAIAgentInitParams[0]);
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
        //两次碰撞体检测
        public override void notify(int i)
        {
            if (this.enabled)
            {
                if (i == 1)
                {
                    near();
                    m_controller.RPC(this, "near");
                    //初始化
                    becomeDark();
                    m_controller.RPC(this, "becomeDark");
                }
                else if (i == 2)
                {
                    enter();
                    m_controller.RPC(this, "enter");
                }
            }
        }

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
                        GUIUtil.yellowColor,
                        0.5f, 0.1f, 16);
                    GUIUtil.DisplaySubtitleInGivenGrammar("^y地球指挥部^w：你们已经进入了电源室，你们需要开启电源，电源室才能正常运作。", mCamera, 16, 0.9f, 0.5f, 3.0f);
                }
                else//遭遇
                {
                    //深度摄像头是否开启
                    bool open = playerCamera.GetComponent<depthSensor>().enabled;
                    GUIUtil.DisplayMissionTargetInMessSequently("任务变化：开启照明开关！",
                        mCamera,
                        GUIUtil.yellowColor,
                        0.5f, 0.1f, 16);
                    if (!open)
                        GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yH^w开启/关闭探测器", mCamera, 12, 0.7f);
                    GUIUtil.DisplaySubtitlesInGivenGrammar(line, mCamera, 16, 0.9f, 0.2f, 2.2f);
                }
                GUIUtil.DisplayMissionPoint(roomPos.position, mCamera, Color.white);
            }
        }

        public void near()
        {
            isNear = true;
        }

        public void enter()
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

        void Update()
        {
            //到时间播放
            if (isInit)
            {
                mCamera = getCamera.GetCurrentUsedCamera();

                if (isNear)
                {
                    if (!playOnce)
                    {
                        TimelineSource.clip = clips[1];
                        TimelineSource.Play();
                        playOnce = true;

                        AIController.instance.CreateAI(4, 0, "EnemyInitPos2", wanderAIAgentInitParams[1]);
                    }
                }
            }
            else//初始化
            {
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
                            TimelineSource.clip = clips[0];
                            TimelineSource.Play();

                            source.clip = clips[2];
                            source.Play();
                            source.priority = TimelineSource.priority + 1;
                        }
                    }
                }
            }
        }
        //关灯
        public void becomeDark()
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
