using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room5Battle
{
    public class Subscene_Start : Subscene
    {
        //托卡马克之心是否已启动冷却（要对着控制台按住F十秒）
        private bool m_IsTokamakeStartToCoolDown;

        //对着控制台按着F的持续时间
        private float m_ControllerOperatingTime = 0.0f;

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
         *                      PRIVATE
         * *************************************************/


    }
}