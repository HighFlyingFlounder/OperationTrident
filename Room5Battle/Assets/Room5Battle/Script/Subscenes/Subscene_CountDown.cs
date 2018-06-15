using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room5Battle
{
    public class Subscene_CountDown : Subscene
    {
        //五分钟 300s
        private float m_CountDownTime = 15.0f;

        //敌人的prefab
        [SerializeField]
        private GameObject m_EnemyPrefab1;

        //四个高台可以生成敌人
        [SerializeField]
        private Transform[] m_EnemyGenPos = new Transform[4];

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
            StartCoroutine(spawnEnemies1());
            StartCoroutine(spawnEnemies2());
        }

        /***************************************************
         *                            PRIVATE
         * *************************************************/
         private string getMinSecStrFromSeconds()
        {
            int min = (int)m_CountDownTime / 60;
            int sec = (int)m_CountDownTime % 60;
            string minStr = min.ToString();
            if (min < 10) minStr = "0" + minStr;

            string secStr = sec.ToString();
            if (sec<10) secStr = "0" + secStr;

            return minStr + ":" + secStr;
        } 

        //生成第一波敌人
        private IEnumerator spawnEnemies1()
        {
            yield return new WaitForSeconds(5.0f);
            for(int i=0;i<4;++i) EnemyGenerator.SpawnEnemy_ExactPos(m_EnemyPrefab1,m_EnemyGenPos[i]);
        }

        //生成第二波敌人
        private IEnumerator spawnEnemies2()
        {
            yield return new WaitForSeconds(10.0f);
            for (int i = 0; i < 4; ++i)
                EnemyGenerator.SpawnEnemy_ExactPos(m_EnemyPrefab1, m_EnemyGenPos[i]);
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

        private void OnGUI()
        {
            //提示按住F
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontSize = 16;
            textStyle.normal.textColor = new Color(0.3f, 0.3f, 1.0f);
            textStyle.alignment = TextAnchor.UpperLeft;

            Rect rect1 = new Rect();
            rect1.xMin = 20.0f;
            rect1.xMax = 200.0f;
            rect1.yMin = 20.0f;
            rect1.yMax = 50.0f;
            GUI.Label(rect1, "托卡马克之心冷却剩余时间: " + getMinSecStrFromSeconds(), textStyle);
        }
    }
}