using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;
using System;

namespace OperationTrident.Room5
{
    public class Subscene_Start : Subscene, NetSyncInterface
    {
        //可交互对象（在unity editor中初始化）
        public InteractiveObject m_ControlPanel;
        public InteractiveObject m_TorCore;

        //托卡马克之心是否已启动冷却（要对着控制台按住F十秒）
        private bool m_isTokamakeStartToCoolDown;

        //对着控制台按着F
        /*private bool m_isLookingAtControlPanel = false;
        private bool m_isPressingKeyF=false;
        private float m_ControlPanelKeyFPressingTime = 0.0f;
        private const float c_RequiredKeyFPressingTime = 1.0f;*/

        //player的camera引用
        private GameObject mCamera;

        //单机测试
        private GameObject playerOffline;

        //是否联网初始化完毕
        private bool isNetworkInitialized = false;

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
            //Debug.Log("Entering Subscene:start");
        }

        //@brief 善后工作
        public override void onSubsceneDestory()
        {
            //Debug.Log("Leaving Subscene:start");
        }

        /***************************************************************
         *                            Subscene's controller
         * **************************************************************/
        private void Start()
        {
            m_ControlPanel.Initialize(
                "ControlPanel", Camera.main, KeyCode.F, 2.0f,
                "^w按住^yF^w开始核心冷却程序", "^w正在启动冷却程序...");
        }

        private void  Update()
        {
            m_ControlPanel.UpdateState();
            if(m_ControlPanel.IsInteractionDone())
            {
                //交互完毕，可以转到“核心开始冷却”场景了
                m_isTokamakeStartToCoolDown = true;
                gameObject.GetComponent<NetSyncController>().SyncVariables();
            }
            //时刻检查是否联网，避免一开始联网玩家没有加载出来出现null引用的问题
            if (GameMgr.instance)
            {
                if (!isNetworkInitialized)
                {
                    
                    if (SceneNetManager.instance.list.ContainsKey(GameMgr.instance.id))
                    {
                        mCamera = (SceneNetManager.instance.list[GameMgr.instance.id]).transform.Find("Camera").gameObject;
                        m_ControlPanel.SetGUICamera(mCamera.GetComponent<Camera>());
                        m_TorCore.SetGUICamera(mCamera.GetComponent<Camera>());
                        isNetworkInitialized = true;
                    }
                }
            }
        }

        private void OnGUI()
        {
            m_ControlPanel.RenderGUI();
        }

        public void RecvData(SyncData data)
        {
            m_isTokamakeStartToCoolDown = (bool)data.Get(typeof(bool));
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            data.Add(m_isTokamakeStartToCoolDown);
            return data;
        }

        public void Init(NetSyncController controller)
        {
            
        }
    }
}