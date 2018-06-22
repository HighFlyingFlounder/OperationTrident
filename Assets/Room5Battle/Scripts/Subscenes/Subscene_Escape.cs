using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room5Battle
{
    //第一次进入房间后，
    public class Subscene_Escape : Subscene
    {
        //反应柱（下降）
        public TokamakeReactorPillar m_ReactorPillar;
        //核心（下降）
        public TokamakeCore m_ReactorCore;


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
            
        }

        private void OnGUI()
        {
            //提示按住F
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontSize = 16;
            textStyle.normal.textColor = new Color(1.0f, 0.3f, 0.3f);
            textStyle.alignment = TextAnchor.UpperLeft;

            Rect rect1 = new Rect();
            rect1.xMin = 20.0f;
            rect1.xMax = 200.0f;
            rect1.yMin = 20.0f;
            rect1.yMax = 50.0f;
            GUI.Label(rect1, "逃脱鲲的核心控制室！ ", textStyle);
        }

    }
}