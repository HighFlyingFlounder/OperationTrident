using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;
using OperationTrident.Common.AI;

namespace OperationTrident.Room5
{
    public class Subscene_CountDown : Subscene
    {
        //反应柱开始冷却（变色）
        public TokamakReactorPillar m_ReactorPillar;

        public WanderAIAgentInitParams[] wanderAIAgentInitParams;
        public TurretAIAgentInitParams[] turretAIAgentInitParams;

        public TurretActionController[] turretActionController;

        //敌人的prefab
       // public GameObject m_EnemyPrefab1;

        //四个高台可以生成敌人
        //public Transform[] m_EnemyGenPos = new Transform[4];

        //三分钟 180s
        private float m_CountDownTime = 180.0f;
        private float m_EnemySpawnDeltaTime = 20.0f;

        //BGM播放
        public AudioSource m_AudioSource;
        public AudioClip m_BGM_CountingDown;

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
            m_ReactorPillar.StartCoolDownProcedure();

            for (int i = 0; i < (int)m_CountDownTime / m_EnemySpawnDeltaTime; ++i)
            {
                StartCoroutine(spawnEnemies1(i * m_EnemySpawnDeltaTime));
            }

            StartCoroutine(spawnEnemyTurret());

            //开始倒计时，换BGM
            m_AudioSource.clip = m_BGM_CountingDown;
            m_AudioSource.Play();
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

        //生成一小波敌人
        private IEnumerator spawnEnemies1(float t)
        {
            yield return new WaitForSeconds(t);
            for (int i = 0; i < 3; ++i)
            {
                AIController.instance.CreateAI(4, 0, "AI-SpawnPositions", wanderAIAgentInitParams[0]);
            }
            //EnemyGenerator.SpawnEnemy_ExactPos(m_EnemyPrefab1,m_EnemyGenPos[i]);
        }

        //生成一大波敌人
        private IEnumerator spawnEnemyTurret()
        {
            yield return new WaitForSeconds(30.0f);
            AIController.instance.CreateAI(4, 1, "AI-SpawnPositions-turret", turretAIAgentInitParams[0]);
            yield return new WaitForSeconds(60.0f);
            AIController.instance.CreateAI(4, 2, "AI-SpawnPositions-turret2", turretAIAgentInitParams[0]);
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
            GUIUtil.DisplayMissionTargetDefault("托卡马克之心冷却剩余时间: " + getMinSecStrFromSeconds(), Room5.GetCameraUtil.GetCurrentCamera(), Color.white);
        }
    }
}