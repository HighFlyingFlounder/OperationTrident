using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room5Battle
{
    public class Subscene_Start : Subscene
    {
        //托卡马克之心是否已启动冷却（要对着控制台按住F十秒）
        private bool m_IsTokamakeStartToCoolDown;

        //对着控制台按着F
        private bool m_isControlPanelPressingKeyF=false;
        private float m_ControlPanelKeyFPressingTime = 0.0f;

        public override bool isTransitionTriggered()
        {
            return m_IsTokamakeStartToCoolDown;
        }

        //@brief 返回下一个子场景
        public override int GetNextSubscene()
        {
            return (int)GameState.COUNTING_DOWN_5MIN;
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

        /***************************************************
         *                     Subscene's controller
         * *************************************************/
        private void  Update()
        {
            Debug.Log(m_ControlPanelKeyFPressingTime.ToString());
            //摄像机中心发出的射线
            Vector3 centerCoordPixel = new Vector3(Camera.main.pixelWidth/2, Camera.main.pixelHeight/2);
            Ray viewRay = Camera.main.ScreenPointToRay(centerCoordPixel);
            RaycastHit hitInfo;
            if (Physics.Raycast(viewRay, out hitInfo, 2f))
            {
                //如果玩家看着控制台
                if (hitInfo.transform.CompareTag("ControlPanel"))
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        m_isControlPanelPressingKeyF = false;
                        m_ControlPanelKeyFPressingTime = 0;
                    }
                    else
                    {
                        m_isControlPanelPressingKeyF = true;
                        m_ControlPanelKeyFPressingTime += Time.deltaTime;
                    }
                }
                else
                {
                    m_isControlPanelPressingKeyF = false;
                }
            }

            //按住F 10s就可以了
            if(m_ControlPanelKeyFPressingTime>2.0f)
            {
                //isTransitionTriggered= true, 开始冷却
                Debug.Log("按完了啊");
                m_IsTokamakeStartToCoolDown = true;
            }
        }

        private void OnGUI()
        {

        }

    }
}