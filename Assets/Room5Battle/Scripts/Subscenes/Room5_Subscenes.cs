using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room5Battle
{
    public enum GameState
    {
        START,//玩家刚进，有少量敌人生成，,准备开始冷却托卡马克之心
        COUNTING_DOWN,//核心开始冷却,倒计时,开始生成新的敌人
        ESCAPING
    }

    public class Room5_Subscenes : MonoBehaviour
    {
        public  SubsceneController m_SubsceneController;

        // Use this for initialization
        void Start()
        {
            m_SubsceneController.addSubscene((int)GameState.START, "Subscene_Start");
            m_SubsceneController.addSubscene((int)GameState.COUNTING_DOWN, "Subscene_CountDown");
            m_SubsceneController.addSubscene((int)GameState.ESCAPING, "Subscene_Escape");

            m_SubsceneController.setInitialSubscene((int)GameState.START);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}