using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

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

        protected GameObject playerMirror;

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

        [SerializeField]
        protected Transform roomPos;

        protected bool isShowTarget = false;

        protected float distance = float.MaxValue;

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

                if (playerCamera)
                {
                    GameObject gun = playerCamera.transform.Find("Gun").gameObject;
                    playerMirror = gun.transform.Find("Mirror").gameObject;
                }

                if (!isInit && this.enabled)
                {
                    playerCamera.AddComponent<becomeDark>();
                    playerCamera.AddComponent<depthSensor>();

                    playerMirror.AddComponent<becomeDark>();
                    playerMirror.AddComponent<depthSensor>();
                    //初始化脚本参数
                    (playerCamera.GetComponent<becomeDark>() as becomeDark).m_Shader = shader_dark;
                    playerCamera.GetComponent<becomeDark>().enabled = true;

                    (playerCamera.GetComponent<depthSensor>() as depthSensor).m_Shader = shader_depthSensor;
                    (playerCamera.GetComponent<depthSensor>() as depthSensor).m_Texture = texture;
                    playerCamera.GetComponent<depthSensor>().enabled = false;

                    (playerMirror.GetComponent<becomeDark>() as becomeDark).m_Shader = shader_dark;
                    playerMirror.GetComponent<becomeDark>().enabled = true;

                    (playerMirror.GetComponent<depthSensor>() as depthSensor).m_Shader = shader_depthSensor;
                    (playerMirror.GetComponent<depthSensor>() as depthSensor).m_Texture = texture;
                    playerMirror.GetComponent<depthSensor>().enabled = false;

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
            GUIUtil.DisplayMissionDetailDefault(
                            missionDetails,
                            Camera.main,
                            GUIUtil.yellowColor,
                            wordTransparentInterval: wordTransparentInterval,
                            wordAppearanceInterval: wordAppearanceInterval,
                            lineSubsequentlyInterval: lineSubsequentlyInterval,
                            fontSize: fontSize);

            if (!isNear)
            {
                GUIUtil.DisplayMissionTargetInMessSequently("突入电源室！",
                    Camera.main,
                    GUIUtil.yellowColor,
                    0.5f, 0.1f, 16);
            }
            else
            {
                //深度摄像头是否开启
                bool open = playerCamera.GetComponent<depthSensor>().enabled;
                GUIUtil.DisplayMissionTargetInMessSequently("任务变化：开启照明开关！",
                    Camera.main,
                    GUIUtil.yellowColor,
                    0.5f,0.1f,16);
                if(!open)
                    GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yG^w开启/关闭探测器", Camera.main, 12, 0.5f);
            }

            GUIStyle style = GUIUtil.GetDefaultTextStyle(GUIUtil.FadeAColor(GUIUtil.greyColor, 60.0f));
            Rect rect = GUIUtil.GetFixedRectDirectlyFromWorldPosition(roomPos.position, Camera.main);
            // 指定颜色
            if (isShowTarget)
            {
                GUI.Label(rect, (int)distance + "m", style);
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

                    playerMirror.GetComponent<depthSensor>().enabled = true;
                    playerMirror.GetComponent<becomeDark>().enabled = false;
                }
                else
                {
                    playerMirror.GetComponent<depthSensor>().enabled = false;
                    playerMirror.GetComponent<becomeDark>().enabled = true;

                    playerMirror.GetComponent<depthSensor>().enabled = true;
                    playerMirror.GetComponent<becomeDark>().enabled = false;
                }
            }

        }

        void Update()
        {
            if (isInit)
            {
                Vector3 point = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0);

                Ray ray = Camera.main.ScreenPointToRay(point);

                distance = Vector3.Distance(roomPos.position, playerCamera.GetComponent<Transform>().position);

                Vector3 direction1 = ray.direction; // 摄像头的方向
                Vector3 direction2 = roomPos.position - playerCamera.GetComponentInParent<Transform>().position; // 到物体的方向

                if (Vector3.Dot(direction1, direction2) <= 0)
                    isShowTarget = false;
                else
                    isShowTarget = true;
            }
        }
    }
}
