using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1
{
    public class AISceneController : MonoBehaviour
    {

        // 场景中的敌人产生的位置链表
        [SerializeField]
        private List<Vector3> enemyCreateList;

        /*
        new Vector3(-50.95f, 2.755f, 21.0f); 
        new Vector3(-27.1f, 2.755f, 8.858f); 
        new Vector3(-27.1f, 5.784f, 13.2f); 
        new Vector3(5.79f, 1.985f, 25.23f);
        new Vectore(-44.271f, 2.281f, -8.47f);
        */

        // 敌人机器人的预设
        [SerializeField]
        private GameObject enemyPrefab;
        // 场景中所有敌人的链表
        private List<GameObject> enemysList;
        // 场景生成敌人的速度(多少秒产生一个)
        public float enemyCreateSpeed;
        // 场景中最大的敌人数量
        public int enemyMaxNum = 10;
        // 场景中当前敌人的数量
        private int nowEnemyNum;

        // Use this for initialization
        void Start()
        {
            enemysList = new List<GameObject>();
            enemyCreateAllAtPreSetPosition();
            nowEnemyNum = enemysList.Count;
            StartCoroutine(EnemyCreateRountine());
        }

        // Update is called once per frame
        void Update()
        {
            nowEnemyNum = enemysList.Count;
            for (int i=0;i<enemysList.Count;i++)
            {
                Common.ReactiveTarget rt = enemysList[i].GetComponent<Common.ReactiveTarget>();
                if (rt.isDeath)
                {
                    enemysList.RemoveAt(i);
                }
            }
        }

        // 产生敌人的协程
        IEnumerator EnemyCreateRountine()
        {
            // 这里暂时是每时每刻都生成？@！#￥
            while (true)
            {
                if (nowEnemyNum < enemyMaxNum)
                {
                    enemyCreate();
                }
                yield return new WaitForSeconds(enemyCreateSpeed);
            }
        }

        // 敌人产生
        private void enemyCreate()
        {
            GameObject nowEnemy = Instantiate(enemyPrefab) as GameObject;
            nowEnemy.transform.localPosition = enemyCreateList[UnityEngine.Random.Range(0, enemyCreateList.Count)];
            enemysList.Add(nowEnemy);

        }

        // 敌人一次性全部产生，在指定的位置各生成一个
        private void enemyCreateAllAtPreSetPosition()
        {
            foreach(var a in enemyCreateList)
            {
                GameObject nowEnemy = Instantiate(enemyPrefab) as GameObject;
                nowEnemy.transform.localPosition = a;
                enemysList.Add(nowEnemy);
            }
        }
    }
}