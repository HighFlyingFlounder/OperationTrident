using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

using OperationTrident.FPS.Common;
using OperationTrident.Common.AI;
using System;

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

        [SerializeField]
        protected GameObject trueBoss;

        [SerializeField]
        protected Transform bossInitPos;

        [SerializeField]
        protected GameObject door;

        [SerializeField]
        protected Transform doorPos;

        protected NetSyncController mController;

        //语音只播放一次
        protected bool playOnce = false;

        [SerializeField]
        protected AudioSource source;

        [SerializeField]
        protected AudioSource TimelineSource;

        [SerializeField]
        protected AudioClip[] clips;

        [SerializeField]
        protected WanderAIAgentInitParams wanderAIAgentParams;

        [SerializeField]
        protected TurretAIAgentInitParams turretAIAgentParams;

        private void Start()
        {
        }

        public override void notify(int i)
        {

        }

        public override bool isTransitionTriggered()
        {
            return false;
        }

        public override void onSubsceneDestory()
        {
        }

        public override void onSubsceneInit()
        {
            if (GameMgr.instance)
            {
                getCamera = (SceneNetManager.instance.list[GameMgr.instance.id]).GetComponent<GetCamera>();
            }
            (SceneNetManager.instance.list[GameMgr.instance.id]).SetActive(false);
            Debug.Log(director.isActiveAndEnabled);
            //director.Play();
        }

        void Update()
        {
            if (getCamera != null)
                mCamera = getCamera.GetCurrentUsedCamera();
            if (!isTimelinePaused)
            {
                //if (director.time > 30.0f)
                {
                    isTimelinePaused = true;
                    trueBoss.transform.position = bossInitPos.position;
                    trueBoss.SetActive(true);

                    Destroy(boss.gameObject);
                    //动画位置同步
                    AIController.instance.AddAIObject(trueBoss);
                    (SceneNetManager.instance.list[GameMgr.instance.id]).SetActive(true);
                    mController.RPC(this, "openDoor");

                    AIController.instance.CreateAI(3, 0, "EnemyInitPos4", wanderAIAgentParams);
                    AIController.instance.CreateAI(4, 1, "EnemyInitPos5", turretAIAgentParams);
                    AIController.instance.CreateAI(4, 1, "EnemyInitPos6", turretAIAgentParams);
                    AIController.instance.CreateAI(4, 0, "EnemyInitPos7", wanderAIAgentParams);
                }
            }
            else
            {
                if (!playOnce)
                {
                    playOnce = true;
                    source.clip = clips[0];
                    source.Play();
                    source.priority = TimelineSource.priority + 1;
                }
            }
            if (trueBoss == null)
            {
                openDoor();
                mController.RPC(this, "openDoor");
            }
        }

        public void openDoor()
        {
            if (trueBoss != null)
            {
                Destroy(trueBoss.gameObject);
            }
            if (door.gameObject)
            {
                Destroy(door.gameObject);
                nextScene_.SetActive(true);
            }
        }

        void OnGUI()
        {
            if (isTimelinePaused)
            {
                if (mCamera)
                {
                    GUIUtil.DisplaySubtitlesInGivenGrammar(line, mCamera, 16, 0.9f, 0.2f, 1.2f);

                    GUIUtil.DisplayMissionTargetInMessSequently("击退敌人，继续前进！",
                          mCamera,
                          GUIUtil.yellowColor,
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