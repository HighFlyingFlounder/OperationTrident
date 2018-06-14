using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room5Battle
{
    public class Subscene_CountDown : Subscene
    {
        //五分钟 300s
        private float m_CountDownTime = 300.0f;

        public override bool isTransitionTriggered()
        {
            //时间倒数完毕
            return m_CountDownTime<0.0f;
        }

        //@brief 返回下一个子场景
        public override int GetNextSubscene()
        {
            return (int)GameState.ESCAPING;
        }

        //@brief 善后工作
        public override void onSubsceneDestory()
        {
        }

        //@brief 子场景的初始化，可以在初始化阶段将所有元素的行为模式改为此状态下的逻辑
        public override void onSubsceneInit()
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
            m_CountDownTime -= Time.deltaTime;
        }
    }
}