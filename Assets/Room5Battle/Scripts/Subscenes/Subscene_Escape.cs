using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;
using System;

namespace OperationTrident.Room5
{
    //第一次进入房间后，
    public class Subscene_Escape : Subscene
    {
        //反应柱（下降）
        public TokamakReactorPillar m_ReactorPillar;
        //核心（下降）
        public TokamakCore m_ReactorCore;

        public GameObject nextScene;

        //是否已经获取核心
        //protected bool isGetTorcore = false;


        public override bool isTransitionTriggered()
        {
            //Room5 battle逃脱完就结束啦
            return false;
        }

        //@brief 返回下一个子场景
        public override int GetNextSubscene()
        {
            //Room5 battle逃脱完就结束啦，没有下一个状态了
            return c_InvalidStateId;
        }

        //@brief 子场景的初始化，可以在初始化阶段将所有元素的行为模式改为此状态下的逻辑
        public override void onSubsceneInit()
        {
            m_ReactorCore.Shutdown();
            m_ReactorPillar.Shutdown();
        }

        //@brief 善后工作
        public override void onSubsceneDestory()
        {
        }

        /***************************************************
         *                     Subscene's controller
         * *************************************************/
        private void Start()
        {
            
        }

        private void Update()
        {
            if (m_ReactorCore == null)
            {
                nextScene.SetActive(true);
            }
        }

        private void OnGUI()
        {
            if (m_ReactorCore != null)
            {
                GUIUtil.DisplayMissionTargetDefault("夺回托卡马克之心.", Camera.main, Color.white);
            }
            else
            {
                GUIUtil.DisplayMissionTargetDefault("逃离中央控制室.", Camera.main, Color.white);
            }
        }
        /*
        public void RecvData(SyncData data)
        {
            isGetTorcore = (bool)data.Get(typeof(bool));
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            data.Add(isGetTorcore);
            return data;
        }

        public void Init(NetSyncController controller)
        {
            
        }
        */
    }
}