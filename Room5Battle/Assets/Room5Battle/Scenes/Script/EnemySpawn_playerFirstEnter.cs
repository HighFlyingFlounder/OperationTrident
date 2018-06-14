using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Room5Battle
{
    public class EnemySpawn_playerFirstEnter : MonoBehaviour
    {
        //private List<GameObject> m_EnemyList;

        [SerializeField]
        private GameObject m_EnemyPrefab1;

        [SerializeField]
        private Transform m_EnemySpawnPos_OnGround1;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnTriggerEnter(Collider collider)
        {
            //生成敌人（出生pos用serializeField绑定在unity的对象里面）
            const int c_enemyNum = 5;

            //敌人在某个位置隔壁一定box范围内生成
            const float c_spawnboxWidth = 5.0f;
            const float c_spawnboxHeight = 1.0f;
            const float c_spawnboxDepth = 5.0f;
            for (int i = 0; i < c_enemyNum; ++i)
            {
                float x_offset = Random.Range(-c_spawnboxWidth, c_spawnboxWidth);
                float y_offset = Random.Range(-c_spawnboxHeight, c_spawnboxHeight);
                float z_offset = Random.Range(-c_spawnboxDepth, c_spawnboxDepth);
                Instantiate(
                    m_EnemyPrefab1,
                    m_EnemySpawnPos_OnGround1.position + new Vector3(x_offset, y_offset, z_offset),
                    m_EnemySpawnPos_OnGround1.rotation);
            }

            Destroy(this.gameObject);
        }
    }
}