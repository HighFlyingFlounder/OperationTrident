using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

using OperationTrident.FPS.Common;

namespace room2Battle {
    //小boss大战
    public class room2_battle :  Subscene{
        protected GetCamera getCamera;

        [SerializeField]
        protected Camera mCamera;

        [SerializeField]
        protected UnityEngine.Playables.PlayableDirector director;

        [SerializeField]
        protected Transform[] enemyInitPositions;

        [SerializeField]
        protected GameObject enemyPrefabs;
        //敌人列表方便管理
        protected ArrayList enemyList = new ArrayList();

        //当前敌人数目，用于补充敌人数目
        protected int currentEnemyNum = 0;
        //最多敌人数目
        protected int maxEnemyNum = 20;

        protected bool isTimelinePaused = false;

        [SerializeField]
        protected GameObject nextScene_;

        public string[] line = {"" };

        [SerializeField]
        protected GameObject boss;


        private void Start()
        {
            AIController.instance.AddAIObject(boss);
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
            foreach (GameObject obj in enemyList)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }

        public override void onSubsceneInit()
        {
            if (GameMgr.instance)
            {
                getCamera = (SceneNetManager.instance.list[GameMgr.instance.id]).GetComponent<GetCamera>();
            }

            Debug.Log(director.isActiveAndEnabled);
            director.Play();
            
        }

        void Update()
        {
            mCamera = getCamera.GetCurrentUsedCamera();
            if (!isTimelinePaused)
            {
                if (director.time >= 30.0f)
                {
                    isTimelinePaused = true;
                }
            }
            else {
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
                    OperationTrident.Util.GUIUtil.DisplayMissionTargetInMessSequently("击退敌人，继续前进！",
                          mCamera,
                          OperationTrident.Util.
                          GUIUtil.yellowColor,
                          0.5f, 0.1f, 16);
                }
            }      
        }
    }

}