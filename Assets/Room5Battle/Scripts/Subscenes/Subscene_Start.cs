using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;
using System;

using OperationTrident.FPS.Common;

namespace OperationTrident.Room5
{
    public class Subscene_Start : Subscene, NetSyncInterface
    {
        //可交互对象（在unity editor中初始化）
        public InteractiveObject m_ControlPanel;
        public InteractiveObject m_TorCore;

        //托卡马克之心是否已启动冷却（要对着控制台按住F十秒）
        private bool m_isTokamakeStartToCoolDown;

        //player的camera引用
        private GameObject mCamera;

        //BGM播放
        public AudioSource m_AudioSource;
        public AudioClip m_BGM_Start;


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
            m_AudioSource.Play();
        }

        //@brief 善后工作
        public override void onSubsceneDestory()
        {
        }

        /***************************************************************
         *                            Subscene's controller
         * **************************************************************/
        private void Start()
        {
            m_ControlPanel.Initialize(
                "Room5ControlPanel", KeyCode.F, 5.0f,//Camera.main
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
                    
                    //设置Camera用于交互
                    /*if (SceneNetManager.instance.list.ContainsKey(GameMgr.instance.id))
                    {
                        //mCamera = (SceneNetManager.instance.list[GameMgr.instance.id]).transform.Find("Camera").gameObject;
                        /*GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                        foreach (GameObject player in players)
                        {
                            m_GetCamera = player.GetComponent<GetCamera>();
                            if (m_GetCamera != null)
                            {
                                break;
                            }
                        }

                        //设置当前的Camera用于Raycast交互
                        m_ControlPanel.SetGUICamera(Room5.GetCameraUtil.GetCurrentCamera());
                        m_TorCore.SetGUICamera(Room5.GetCameraUtil.GetCurrentCamera());
                        isNetworkInitialized = true;
                    }*/
                }
            }
        }

        private void OnGUI()
        {
            //任务目标
            GUIUtil.DisplayMissionTargetInMessSequently("前往控制台启动核心冷却程序.", GetCameraUtil.GetCurrentCamera(), Color.white);

            //左下角任务细节
            string[] missionDetails =
            {
                "2048.8.1 15:00 p.m. GMT+8",
                "鲲内部  核聚变反应室",
                "三叉戟行动"
            };
            GUIUtil.DisplayMissionDetailDefault(missionDetails, GetCameraUtil.GetCurrentCamera(), Color.white,18,0.005f,0.1f,0.5f);

            //控制台的交互GUI
            m_ControlPanel.RenderGUI();

            //控制台目标距离
            GUIUtil.DisplayMissionPoint(m_ControlPanel.transform.position, GetCameraUtil.GetCurrentCamera(), Color.white);

            //字幕
            string[] subtitles =
            {
                "",
                "^g地球指挥部^w：这里是鲲的核心，核聚变反应室",
                "^g地球指挥部^w：你们要在这里取回反应核心——^y托卡马克之心",
                "^g地球指挥部^w：反应核心的冷却需要一段时间，期间可能会有大量防御机器人持续涌入，保持警惕"
            };

            float[] lastingTime = { 7.0f, 2.5f, 3.5f, 6.0f };
            float[] intervals = { 0.2f, 0.2f, 0.2f, 0.2f };
            GUIUtil.DisplaySubtitlesInGivenGrammarWithTimeStamp(subtitles, GetCameraUtil.GetCurrentCamera(), 20, 0.9f, lastingTime, intervals);

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