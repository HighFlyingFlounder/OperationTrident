using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room5Battle
{
    public class Subscene_Start : Subscene
    {
        [SerializeField]
        private Texture m_TexturePureGrey;

        //托卡马克之心是否已启动冷却（要对着控制台按住F十秒）
        private bool m_isTokamakeStartToCoolDown;

        //对着控制台按着F
        private bool m_isLookingAtControlPanel = false;
        private bool m_isPressingKeyF=false;
        private float m_ControlPanelKeyFPressingTime = 0.0f;

        private const float c_RequiredKeyFPressingTime = 1.0f;

        public override bool isTransitionTriggered()
        {
            return m_isTokamakeStartToCoolDown;
        }

        //@brief 返回下一个子场景
        public override int GetNextSubscene()
        {
            return (int)GameState.COUNTING_DOWN;
        }

        //@brief 子场景的初始化，可以在初始化阶段将所有元素的行为模式改为此状态下的逻辑
        public override void onSubsceneInit()
        {
            Debug.Log("Entering Subscene:start");
        }

        //@brief 善后工作
        public override void onSubsceneDestory()
        {
            Debug.Log("Leaving Subscene:start");
        }

        /***************************************************************
         *                            Subscene's controller
         * **************************************************************/
        private void  Update()
        {
            //摄像机中心发出的射线
            Vector3 centerCoordPixel = new Vector3(Camera.main.pixelWidth/2, Camera.main.pixelHeight/2);
            Ray viewRay = Camera.main.ScreenPointToRay(centerCoordPixel);
            RaycastHit hitInfo;
            if (Physics.Raycast(viewRay, out hitInfo, 2f))
            {
                //如果玩家看着控制台
                if (hitInfo.transform.CompareTag("ControlPanel"))
                {
                    m_isLookingAtControlPanel = true;
                }
                else
                {
                    m_isLookingAtControlPanel = false;
                }
            }

            //如果玩家按下F
            if (Input.GetKey(KeyCode.F))
            {
                m_isPressingKeyF = true;
            }
            else
            {
                m_isPressingKeyF = false;

            }

            //是否对着控制台按F
            if(m_isPressingKeyF && m_isLookingAtControlPanel)
                m_ControlPanelKeyFPressingTime += Time.deltaTime;
            else
                m_ControlPanelKeyFPressingTime = 0;

            //按住F 10s就可以了
            if (m_ControlPanelKeyFPressingTime> c_RequiredKeyFPressingTime)
            {
                //isTransitionTriggered= true, 开始冷却,转入下一场景
                Debug.Log("按完了啊");
                m_isTokamakeStartToCoolDown = true;
            }
        }

        private void OnGUI()
        {
            //提示按住F
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontSize = 24;
            textStyle.normal.textColor = new Color(0.3f, 0.3f, 0.3f);
            textStyle.alignment = TextAnchor.MiddleCenter;

            if (m_isLookingAtControlPanel)
            {
                //显示按F的进度条
                if (m_isPressingKeyF)
                {
                    float halfW = Camera.main.pixelWidth / 2;
                    float halfH = Camera.main.pixelHeight / 2;
                    Rect rect1 = new Rect(halfW, halfH, 30.0f, 20.0f);
                    GUI.Label(rect1, "正在启动冷却程序...", textStyle);

                    float barWidth = 200.0f;
                    Rect rect2 = new Rect(halfW, halfH + 30.0f, 50.0f, 20.0f);
                    rect2.xMin = halfW - barWidth / 2;
                    rect2.xMax = rect2.xMin + barWidth * (m_ControlPanelKeyFPressingTime / c_RequiredKeyFPressingTime);
                    rect2.yMin = halfH + 30.0f;
                    rect2.yMax = halfH + 40.0f;
                    GUI.DrawTexture(rect2, m_TexturePureGrey);
                }
                else
                {
                    float halfW = Camera.main.pixelWidth / 2;
                    float halfH = Camera.main.pixelHeight / 2;
                    Rect rect1 = new Rect(halfW, halfH+100.0f, 30.0f, 20.0f);
                    GUI.Label(rect1, "按住F启动冷却程序", textStyle);
                }
            }
        }

    }
}