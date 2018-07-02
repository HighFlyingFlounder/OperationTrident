using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;


namespace room2Battle {
    //小boss大战
    public class room2_battle :  Subscene{

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
            //Debug.Log(director.isActiveAndEnabled);
            //director.Play();
            boss.SetActive(true);
        }

        void Update()
        {
            if (!isTimelinePaused)
            {
                if (director.state != UnityEngine.Playables.PlayState.Playing)
                {
                    isTimelinePaused = true;
                    //播放完动画在生成敌人
                    for (int i = 0; i < maxEnemyNum; ++i)
                    {
                        GameObject obj = Instantiate(enemyPrefabs, enemyInitPositions[Random.Range(0, enemyInitPositions.Length)].position, Quaternion.identity);
                        enemyList.Add(obj);
                    }
                }
            }
            else {
                nextScene_.SetActive(true);
                //补充敌人
                for (int i = 0; i < enemyList.Count; ++i)
                {
                    if (enemyList[i] == null)
                    {
                        enemyList[i] = Instantiate(enemyPrefabs, enemyInitPositions[Random.Range(0, enemyInitPositions.Length)].position, Quaternion.identity);
                        break;
                    }
                }
            }
        }

        void OnGUI()
        {
            if (isTimelinePaused)
            {
                GUIUtil.DisplaySubtitlesInGivenGrammar(line, Camera.main, 16, 0.9f, 0.2f, 1.2f);
                OperationTrident.Util.GUIUtil.DisplayMissionTargetInMessSequently("击退敌人，继续前进！",
                      Camera.main,
                      OperationTrident.Util.GUIUtil.yellowColor,
                      0.5f, 0.1f, 16);
            }
            if (!isTimelinePaused)
            {
                GUIUtil.DisplayMissionTargetDefault("???", mCamera, OperationTrident.Util.GUIUtil.yellowColor);
            }
            
        }

    }

}