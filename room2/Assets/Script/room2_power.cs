using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle {
    //能源房大战
    public class room2_power :  Subscene{
        [SerializeField]
        protected GameObject enemyPrefabs;

        protected ArrayList enemyList = new ArrayList();

        protected int currentEnemyNum = 0;

        protected int maxEnemyNum = 20;

        [SerializeField]
        protected Transform[] enemyInitPositions;

        [SerializeField]
        protected GameObject playerControl;

        //钥匙
        [SerializeField]
        protected GameObject key;

        //主相机，需要判断是否拿到钥匙
        protected Camera mCamera;

        protected System.DateTime sw = new System.DateTime();

        protected System.TimeSpan span;

        protected bool isFocus = false;

        protected bool startTiming = false;

        protected bool isSwitchOpen = false;

        protected bool isIntoSecondFloor = false;

        //获取相机句柄
        void Start()
        {
            mCamera = Camera.main;
        }

        public override bool isTransitionTriggered()
        {
            return isIntoSecondFloor && isSwitchOpen;
        }

        public override string GetNextSubscene()
        {
            return "room2_battle";
        }

        public override void onSubsceneDestory()
        {
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
            //通过摄像机的蓝色轴（即Z轴），射向对应物体，判断标签
            RaycastHit hit;
            if (Physics.Raycast(mCamera.transform.position, mCamera.transform.forward,out hit))
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

            //生成敌人
            if (enemyList.Count < maxEnemyNum)
            {
                GameObject obj = Instantiate(enemyPrefabs, enemyInitPositions[Random.Range(0, enemyInitPositions.Length)].position, Quaternion.identity);
                enemyList.Add(obj);
            }
            else
            {
                for (int i = 0; i < enemyList.Count; ++i)
                {
                    if (enemyList[i] == null)
                    {
                        enemyList[i] = Instantiate(enemyPrefabs, enemyInitPositions[Random.Range(0, enemyInitPositions.Length)].position, Quaternion.identity);
                        break;
                    }
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

            if (Input.GetKeyDown(KeyCode.K))
            {
                inDark dark = playerControl.GetComponent<inDark>();
                Debug.Log(dark);
                dark.gameObject.SetActive(true);
            }
        }

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
        }
    }
}
