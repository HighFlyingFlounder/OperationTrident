using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    //能源房大战
    public class room2_power : Subscene
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
        //玩家
        //public GameObject player;

        //钥匙
        [SerializeField]
        protected GameObject key;

        //主相机，需要判断是否拿到钥匙
        protected Camera mCamera;

        //计时器
        protected System.DateTime sw = new System.DateTime();

        protected System.TimeSpan span;

        //是否对准开关
        protected bool isFocus = false;

        //是否开始计时
        protected bool startTiming = false;
        //是否打开开关
        protected bool isSwitchOpen = false;
        //是否进入2楼
        protected bool isIntoSecondFloor = false;
        //是否打开夜视仪
        protected bool isOpenDepthSensor = false;

        protected GameObject playerCamera = null;

        //挂载脚本的shader，包括dark和depth sensor
        [SerializeField]
        protected Shader shader_dark = null;

        //获取相机句柄
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

        public override void onSubsceneDestory()
        {
            //player.GetComponent<depthSensor>().enabled = false;
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
            playerCamera = (NetWorkManager.instance.list[GameMgr.instance.id]).transform.Find("Camera").gameObject;
            playerCamera.AddComponent<becomeDark>();
            //初始化脚本参数
            (playerCamera.GetComponent<becomeDark>() as becomeDark).m_Shader = shader_dark;
            playerCamera.GetComponent<becomeDark>().enabled = true;
            
            //player.GetComponent<becomeDark>().enabled = true;
            //player.GetComponent<depthSensor>().enabled = true;

            //生成敌人
            for (int i = 0; i < maxEnemyNum; ++i)
            {
                GameObject obj = Instantiate(enemyPrefabs, enemyInitPositions[Random.Range(0, enemyInitPositions.Length)].position, Quaternion.identity);
                enemyList.Add(obj);
            }
        }

        public override void notify(int i)
        {
            if (i == 1)
            {
                isIntoSecondFloor = true;
                Debug.Log("player into floor2");
            }
        }

        void Update()
        {
            Vector3 point = new Vector3(mCamera.pixelWidth / 2, mCamera.pixelHeight / 2, 0);

            Ray ray = mCamera.ScreenPointToRay(point);

            //通过摄像机的蓝色轴（即Z轴），射向对应物体，判断标签
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //获取物体
                GameObject obj = hit.transform.gameObject;
                //判断标签
                if (obj.tag == "switch")
                {
                    if ((hit.transform.position - mCamera.transform.position).sqrMagnitude < 5.0f)
                    {
                        //开始计时
                        isFocus = true;
                    }
                }
                else
                {
                    //重置
                    isFocus = false;
                    if (startTiming)
                    {
                        span = System.TimeSpan.Zero;
                        startTiming = false;
                    }
                }
            }

            //补充敌人
            for (int i = 0; i < enemyList.Count; ++i)
            {
                if (enemyList[i] == null)
                {
                    enemyList[i] = Instantiate(enemyPrefabs, enemyInitPositions[Random.Range(0, enemyInitPositions.Length)].position, Quaternion.identity);
                    break;
                }
            }
        }

        void LateUpdate()
        {
            //当看到物品时
            if (isFocus)
            {
                //一直按键
                if (Input.GetKey(KeyCode.F))
                {
                    if (!startTiming)
                    {
                        sw = System.DateTime.Now;
                        span = System.TimeSpan.Zero;
                        startTiming = true;
                    }
                    System.DateTime after = System.DateTime.Now;
                    span = after.Subtract(sw);
                    if (span.TotalSeconds >= 5.0f)
                    {
                        //player.GetComponent<becomeDark>().enabled = false;
                        RenderSettings.ambientIntensity = 1.0f;
                        isSwitchOpen = true;
                    }
                }
                else
                {
                    //重置
                    startTiming = false;
                    sw = System.DateTime.Now;
                    span = System.TimeSpan.Zero;
                }
            }
            //按H打开夜视仪
            if (Input.GetKeyDown(KeyCode.H))
            {
                //通过只有一个后处理，减少post processing的pass
                if (!isOpenDepthSensor)
                {
                    // player.GetComponent<depthSensor>().enabled = true;
                    //player.GetComponent<becomeDark>().enabled = false;
                    isOpenDepthSensor = true;
                }
                else
                {
                    if (!isSwitchOpen)
                    {
                        //player.GetComponent<becomeDark>().enabled = true;
                    }
                    //player.GetComponent<depthSensor>().enabled = false; 
                    isOpenDepthSensor = false;
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
                if (startTiming)
                {
                    if (!isSwitchOpen)
                        GUI.Label(new Rect(posX, posY, 100, 100), "还剩" + (5.0f - span.TotalSeconds) + "s");
                    else
                        GUI.Label(new Rect(posX, posY, 100, 100), "干的漂亮");
                }
                else if (!isSwitchOpen)
                {
                    GUI.Label(new Rect(posX, posY, 100, 100), "按住F");
                }
                else
                {
                    GUI.Label(new Rect(posX, posY, 100, 100), "");
                }
            }
            if (!isSwitchOpen)
            {
                OperationTrident.Util.GUIUtil.DisplayMissionTargetDefault("清除附近敌人，打通到电源室的道路！", Camera.main, true);
            }
            else
            {
                if (!isIntoSecondFloor)
                {
                    OperationTrident.Util.GUIUtil.DisplayMissionTargetDefault("挺进2楼！", Camera.main, true);
                }
            }
        }
    }
}
