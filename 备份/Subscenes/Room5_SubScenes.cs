using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room5Battle
{
    public enum GameState
    {
        START = 0,//玩家刚进，有少量敌人生成，,准备开始冷却托卡马克之心
        COUNTING_DOWN_5MIN,//核心开始冷却,倒计时,开始生成新的敌人
        COUNTING_DOWN_3MIN,
        COUNTING_DOWN_1MIN,
        ESCAPING
    }

    public class Room5_Subscenes : MonoBehaviour
    {
        private SubsceneController m_Subscenes;

        // Use this for initialization
        void Start()
        {
            m_Subscenes.addSubscene((int)GameState.START, "Subscene_Start");
            m_Subscenes.addSubscene((int)GameState.COUNTING_DOWN_5MIN, "Subscene_5min");
            m_Subscenes.addSubscene((int)GameState.COUNTING_DOWN_3MIN, "Subscene_3min");

            m_Subscenes.setInitialSubscene((int)GameState.START);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}