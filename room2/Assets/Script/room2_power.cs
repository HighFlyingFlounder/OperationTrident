using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle {
    //能源房大战
    public class room2_power :  Subscene{

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

        //获取相机句柄
        void Start()
        {
            mCamera = Camera.main;
        }

        public override bool isTransitionTriggered()
        {
            return false;
        }

        public override string GetNextSubscene()
        {
            return "room2_battle";
        }

        public override void onSubsceneDestory()
        {
            
        }

        public override void onSubsceneInit()
        {
            
        }

        public override void notify(int i)
        {
        }

        void Update()
        {
            //通过摄像机的蓝色轴（即Z轴），射向对应物体，判断标签
            RaycastHit hit;
            if (Physics.Raycast(mCamera.transform.position, mCamera.transform.forward, out hit))
            {
                //获取物体
                GameObject obj = hit.transform.gameObject;
                //判断标签
                if (obj.tag == "switch")
                {
                    UnityEngine.Debug.Log(obj.ToString());
                    //开始计时
                    isFocus = true;
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

                    Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    System.DateTime after = System.DateTime.Now;
                    span = after.Subtract(sw);
                    if (span.TotalSeconds >= 5.0f)
                    {
                        isSwitchOpen = true;
                        Debug.Log("========================");
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
                else
                {
                    GUI.Label(new Rect(posX, posY, 100, 100), "按住F");
                }
            }
        }
    }
}
