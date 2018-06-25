using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    //==================================================
    //================   走出电梯   ====================
    //=================================================

    public class room2_enter : Subscene, NetSyncInterface
    {
        NetSyncController m_controller;

        protected GameObject playerCamera = null;

        [SerializeField]
        protected GameObject player;

        //挂载脚本的shader，包括dark和depth sensor
        [SerializeField]
        protected Shader shader_dark = null;

        [SerializeField]
        protected Shader shader_depthSensor = null;

        //depthSensor的纹理
        [SerializeField]
        protected Texture texture = null;

        protected bool isNear = false;

        protected bool isEnter = false;

        public string[] missionDetails =
            {
            "2018.6.22  星期五",
            "外太空",
            "三叉戟行动"
        };

        public float wordTransparentInterval = 0.005f; // 字变得更加透明的周期
        public float wordAppearanceInterval = 0.1f; // 每行字一个个出现的速度
        public float lineSubsequentlyInterval = 1.236f; // 每行字一行行出现的速度
        public int fontSize = 16; // 字体大小

        protected bool isInit = false;

        public override void onSubsceneInit()
        {
        }

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

        public override void notify(int i)
        {
            if (i == 1)
            {
                isNear = true;
                // player.GetComponent<becomeDark>().enabled = true;
                m_controller.SyncVariables();
                //m_controller.RPC(this,"method_need_to_sync",1,"string for param2");
                //初始化
                if (GameMgr.instance)//联网状态
                    playerCamera = (NetWorkManager.instance.list[GameMgr.instance.id]).transform.Find("Camera").gameObject;
                else
                    playerCamera = player.transform.Find("Camera").gameObject;

                if (!isInit && this.enabled)
                {
                    playerCamera.AddComponent<becomeDark>();
                    playerCamera.AddComponent<depthSensor>();
                    //初始化脚本参数
                    (playerCamera.GetComponent<becomeDark>() as becomeDark).m_Shader = shader_dark;
                    playerCamera.GetComponent<becomeDark>().enabled = true;

                    (playerCamera.GetComponent<depthSensor>() as depthSensor).m_Shader = shader_depthSensor;
                    (playerCamera.GetComponent<depthSensor>() as depthSensor).m_Texture = texture;
                    playerCamera.GetComponent<depthSensor>().enabled = false;

                    isInit = true;
                }
            }
            else if (i == 2)
            {
                isEnter = true;
                m_controller.SyncVariables();
            }
        }

        void OnGUI()
        {
            OperationTrident.Util.GUIUtil.DisplayMissionDetailDefault(
                            missionDetails,
                            Camera.main,
                            OperationTrident.Util.GUIUtil.yellowColor,
                            wordTransparentInterval: wordTransparentInterval,
                            wordAppearanceInterval: wordAppearanceInterval,
                            lineSubsequentlyInterval: lineSubsequentlyInterval,
                            fontSize: fontSize);

            if (!isNear)
            {
                OperationTrident.Util.GUIUtil.DisplayMissionTargetInMessSequently("突入电源室！",
                    Camera.main,
                    OperationTrident.Util.GUIUtil.yellowColor,
                    0.5f, 0.1f, 16);
            }
            else
            {
                //深度摄像头是否开启
                bool open = playerCamera.GetComponent<depthSensor>().enabled;
                OperationTrident.Util.GUIUtil.DisplayMissionTargetInMessSequently("任务变化：开启照明开关！",
                    Camera.main,
                    OperationTrident.Util.GUIUtil.yellowColor,
                    0.5f,0.1f,16);
                if(!open)
                    OperationTrident.Util.GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yG^w开启/关闭探测器", Camera.main, 12, 0.5f);
            }
        }

        public void RecvData(SyncData data)
        {
            isNear = (bool)data.Get(typeof(bool));
            isEnter = (bool)data.Get(typeof(bool));
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            data.Add(isNear);
            data.Add(isEnter);
            return data;
        }

        public void Init(NetSyncController controller)
        {
            m_controller = controller;
        }


        void LateUpdate()
        {
            //按G打开夜视仪
            if (Input.GetKeyDown(KeyCode.G))
            {
                //通过只有一个后处理，减少post processing的pass
                if (playerCamera.GetComponent<becomeDark>().enabled)
                {
                    playerCamera.GetComponent<depthSensor>().enabled = true;
                    playerCamera.GetComponent<becomeDark>().enabled = false;
                }
                else
                {
                    playerCamera.GetComponent<depthSensor>().enabled = false;
                    playerCamera.GetComponent<becomeDark>().enabled = true;
                }
            }

        }
    }
}
