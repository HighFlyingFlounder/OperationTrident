using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

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

        protected GameObject playerMirror = null;

        //挂载脚本的shader，包括dark和depth sensor
        //[SerializeField]
        //protected Shader shader_dark = null;

        //挂载相机的对象，单机可用
        [SerializeField]
        protected GameObject player;

        [SerializeField]
        protected Transform switchPos;

        [SerializeField]
        protected Transform secondFloor;

        protected bool isShowTarget = false;

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

            playerMirror.GetComponent<depthSensor>().enabled = false;
            playerMirror.GetComponent<becomeDark>().enabled = false;

            Destroy(playerCamera.GetComponent<depthSensor>());
            Destroy(playerCamera.GetComponent<becomeDark>());
            Destroy(playerMirror.GetComponent<depthSensor>());
            Destroy(playerMirror.GetComponent<becomeDark>());

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
                playerCamera = (SceneNetManager.instance.list[GameMgr.instance.id]).transform.Find("Camera").gameObject;
            else
                playerCamera = player.transform.Find("Camera").gameObject;

            if (playerCamera)
            {
                GameObject gun = playerCamera.transform.Find("Gun").gameObject;
                playerMirror = gun.transform.Find("Mirror").gameObject;
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
                isIntoSecondFloor = true;
                Debug.Log("player into floor2");
                gameObject.GetComponent<NetSyncController>().SyncVariables();
            }
        }

        void Update()
        {
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
                    if ((hit.transform.position - mCamera.transform.position).sqrMagnitude < 5.0f)
                    {
                        isFocus = true;
                    }
                }
                else
                {
                    isFocus = false;

                }
            }

            //@TODO: 替换成老Y的AI
            for (int i = 0; i < enemyList.Count; ++i)
            {
                if (enemyList[i] == null)
                {
                    enemyList[i] = Instantiate(enemyPrefabs, enemyInitPositions[Random.Range(0, enemyInitPositions.Length)].position, Quaternion.identity);
                    break;
                }
            }
            Vector3 direction1 = ray.direction; // 摄像头的方向
            Vector3 direction2;

            if (!isSwitchOpen)
            {
                distance = Vector3.Distance(switchPos.position, playerCamera.GetComponent<Transform>().position);
                direction2 = switchPos.position - playerCamera.GetComponentInParent<Transform>().position; // 到物体的方向
            }
            else
            {
                distance = Vector3.Distance(secondFloor.position, playerCamera.GetComponent<Transform>().position);
                direction2 = secondFloor.position - playerCamera.GetComponentInParent<Transform>().position; // 到物体的方向
            }
            
            
            // 如果物体大方向在人视线背后的话，就不显示了
            if (Vector3.Dot(direction1, direction2) <= 0)
                isShowTarget = false;
            else
                isShowTarget = true;
        }

        void LateUpdate()
        {
            //当看到物品时
            if (isFocus)
            {

                if (Input.GetKey(KeyCode.F))
                {
                    isSwitchOpen = true;
                    gameObject.GetComponent<NetSyncController>().SyncVariables();
                }
            }
            //按G打开夜视仪
            if (Input.GetKeyDown(KeyCode.G))
            {
                //通过只有一个后处理，减少post processing的pass
                if (!isOpenDepthSensor)
                {
                    playerCamera.GetComponent<depthSensor>().enabled = true;
                    playerCamera.GetComponent<becomeDark>().enabled = false;

                    playerMirror.GetComponent<depthSensor>().enabled = true;
                    playerMirror.GetComponent<becomeDark>().enabled = false;

                    isOpenDepthSensor = true;
                }
                else
                {
                    if (!isSwitchOpen)
                    {
                        playerCamera.GetComponent<becomeDark>().enabled = true;
                        playerMirror.GetComponent<becomeDark>().enabled = true;
                    }
                    playerCamera.GetComponent<depthSensor>().enabled = false;
                    playerMirror.GetComponent<depthSensor>().enabled = false;
                    isOpenDepthSensor = false;
                }
            }

            if (isSwitchOpen)
            {
                playerCamera.GetComponent<becomeDark>().enabled = false;
                playerMirror.GetComponent<becomeDark>().enabled = false;
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
                    GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yF^w与物品交互", mCamera, 12, 0.5f);
                }
                else
                {
                    GUIUtil.DisplaySubtitleInDefaultPosition("干得漂亮", mCamera, 12, 0.5f);
                }
            }
            //深度摄像头是否开启，是否黑
            bool open = playerCamera.GetComponent<becomeDark>().enabled;
            bool open2 = playerCamera.GetComponent<depthSensor>().enabled;
            if (!isSwitchOpen)
            {
                GUIUtil.DisplayMissionTargetInMessSequently("清除附近敌人，打通到电源室的道路！",
                   Camera.main,
                   GUIUtil.yellowColor,
                   0.5f, 0.1f, 16);

                if (!open2 && open)
                {
                    GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yG^w开启/关闭探测器", mCamera, 12, 0.5f);
                }

                GUIStyle style = GUIUtil.GetDefaultTextStyle(GUIUtil.FadeAColor(GUIUtil.greyColor, 60.0f));
                Rect rect = GUIUtil.GetFixedRectDirectlyFromWorldPosition(switchPos.position + new Vector3(0.0f, 3.0f, 0.0f), mCamera);
                // 指定颜色
                if (isShowTarget)
                {
                    GUI.Label(rect, (int)distance + "m", style);
                }
            }
            else
            {
                if (!isIntoSecondFloor)
                {
                    GUIUtil.DisplayMissionTargetInMessSequently("挺进2楼！",
                       Camera.main,
                       GUIUtil.yellowColor,
                       0.5f, 0.1f, 16);
                    GUIStyle style = GUIUtil.GetDefaultTextStyle(GUIUtil.FadeAColor(GUIUtil.greyColor, 60.0f));
                    Rect rect = GUIUtil.GetFixedRectDirectlyFromWorldPosition(secondFloor.position + new Vector3(0.0f, 3.0f, 0.0f), mCamera);
                    // 指定颜色
                    if (isShowTarget)
                    {
                        GUI.Label(rect, (int)distance + "m", style);
                    }
                }
                if (open2 && !isFocus)
                {
                    GUIUtil.DisplaySubtitleInGivenGrammar("^w按^yG^w关闭探测器", mCamera, 12, 0.5f);
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
