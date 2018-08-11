using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

using OperationTrident.FPS.Common;
using OperationTrident.Common.AI;
using System;
using OperationTrident.Common;
using OperationTrident.Common.UI;

namespace room2Battle
{
    //小boss大战
    public class room2_battle : Subscene, NetSyncInterface
    {
        protected GetCamera getCamera;

        [SerializeField]
        protected Camera mCamera;

        [SerializeField]
        protected UnityEngine.Playables.PlayableDirector director;

        //timeline播放程度
        protected bool isTimelinePaused = false;

        [SerializeField]
        protected GameObject nextScene_;
        //台词
        public string[] line = { "" };
        //boss，可能到时候有两个，一个过场，一个动作
        [SerializeField]
        protected GameObject boss;

        //真·boss
        [SerializeField]
        protected GameObject trueBoss;
        //boss的摆放位置
        [SerializeField]
        protected Transform bossInitPos;
        //门
        [SerializeField]
        protected GameObject door;

        [SerializeField]
        protected Transform doorPos;
        //同步控制器
        protected NetSyncController mController;

        //语音只播放一次
        protected bool playOnce = false;

        protected bool playOnce_ = false;
        //bgm音源
        [SerializeField]
        protected AudioSource source;
        //台词音源
        [SerializeField]
        protected AudioSource TimelineSource;
        //音轨
        [SerializeField]
        protected AudioClip[] clips;
        //AI参数
        [SerializeField]
        protected WanderAIAgentInitParams[] wanderAIAgentParams;

        [SerializeField]
        protected TurretAIAgentInitParams turretAIAgentParams;
        //只删一次boss
        protected bool destoryBoss = false;

        protected float lastTimeInitAI = 0.0f;

        [SerializeField]
        protected GameObject escapeElevator;



        private void Start()
        {
        }

        public override void notify(int i)
        {

        }
        //没下一个小场景
        public override bool isTransitionTriggered()
        {
            return false;
        }

        public override void onSubsceneDestory()
        {
        }

        /// <summary>
        /// @brief 播放timeline，关闭玩家的操作
        /// </summary>
        public override void onSubsceneInit()
        {
            TimelineSource.Stop();
            if (GameMgr.instance)
            {
                getCamera = (SceneNetManager.instance.list[GameMgr.instance.id]).GetComponent<GetCamera>();
            }

            foreach (var p in SceneNetManager.instance.list)
            {
                p.Value.GetComponent<ReactiveTarget>().CanBeHurt = false;
                p.Value.SetActive(false);
            }

            GamingUIManager.Instance.HidePlayerInfoUI();
            director.Play();
        }

        /// <summary>
        /// @brief 判断timeline播放阶段，生成AI，boss，激活玩家
        /// </summary>
        void Update()
        {
            if (getCamera != null)
                mCamera = getCamera.GetCurrentUsedCamera();
            if (!isTimelinePaused)//bool值作为flag
            {
                if (director.time > 30.0f)
                {
                    isTimelinePaused = true;
                    trueBoss.transform.position = bossInitPos.position;
                    trueBoss.SetActive(true);

                    Destroy(boss.gameObject);
                    //动画位置同步
                    AIController.instance.AddAIObject(trueBoss);

                    foreach (var p in SceneNetManager.instance.list)
                    {
                        p.Value.GetComponent<ReactiveTarget>().CanBeHurt = true;
                        p.Value.SetActive(true);
                    }
                    GamingUIManager.Instance.ShowPlayerInfoUI();

                    AIController.instance.CreateAI(1, 0, "EnemyInitPos4", wanderAIAgentParams[4]);
                    AIController.instance.CreateAI(1, 0, "EnemyInitPos4", wanderAIAgentParams[5]);
                    AIController.instance.CreateAI(1, 0, "EnemyInitPos4", wanderAIAgentParams[6]);
                    AIController.instance.CreateAI(3, 2, "EnemyInitPos5", turretAIAgentParams);
                    AIController.instance.CreateAI(3, 1, "EnemyInitPos6", turretAIAgentParams);
                    AIController.instance.CreateAI(1, 0, "EnemyInitPos7", wanderAIAgentParams[0]);
                    AIController.instance.CreateAI(1, 0, "EnemyInitPos7", wanderAIAgentParams[1]);
                    AIController.instance.CreateAI(1, 0, "EnemyInitPos7", wanderAIAgentParams[2]);
                    AIController.instance.CreateAI(1, 0, "EnemyInitPos7", wanderAIAgentParams[3]);
                    AIController.instance.CreateAI(2, 2, "EnemyInitPos7", turretAIAgentParams);
                }
            }
            else//播放台词
            {
                if (!playOnce)
                {
                    playOnce = true;
                    source.clip = clips[0];
                    source.Play();
                    source.priority = TimelineSource.priority + 1;
                }

                if (lastTimeInitAI >= 10.0f)
                {
                    AIController.instance.CreateAI(1, 0, "EnemyInitPos5", wanderAIAgentParams[3]);
                    AIController.instance.CreateAI(1, 0, "EnemyInitPos4", wanderAIAgentParams[4]);
                    lastTimeInitAI = 0.0f;
                }
                else
                {
                    lastTimeInitAI += Time.deltaTime;
                }
            }
            //TODO:测试，删除
            if (trueBoss == null)
            {
                if (!destoryBoss)
                {
                    openDoor_Room2();
                    mController.RPC(this, "openDoor_Room2");
                    destoryBoss = true;
                }
            }
        }

        /// <summary>
        /// @brief RPC的关门
        /// </summary>
        public void openDoor_Room2()
        {
            if (door.gameObject)
            {
                Destroy(door.gameObject);
                escapeElevator.GetComponent<UnityEngine.Playables.PlayableDirector>().Play();
                nextScene_.SetActive(true);
            }
        }

        void OnGUI()
        {
            if (mCamera != null)
            {
                if (isTimelinePaused)
                {
                    GUIUtil.DisplaySubtitlesInGivenGrammar(line, mCamera, 16, 0.9f, 0.2f, 1.2f);

                    GUIUtil.DisplayMissionTargetInMessSequently("击退敌人，继续前进！",
                          mCamera,
                          GUIUtil.whiteColor,
                          0.5f, 0.1f, 16);
                }
            }
        }

        public void RecvData(SyncData data)
        {

        }

        public SyncData SendData()
        {
            return null;
        }

        public void Init(NetSyncController controller)
        {
            mController = controller;
        }
    }

}