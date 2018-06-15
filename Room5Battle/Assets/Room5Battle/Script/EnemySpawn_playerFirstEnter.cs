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
            //生成敌人（出生pos用serializeField绑定在unity的gameObject里面）
            EnemyGenerator.SpawnEnemy_BoxRandom(
                m_EnemyPrefab1, 
                5, 
                m_EnemySpawnPos_OnGround1, 
                new Vector3(5.0f, 1.0f, 5.0f));

            Destroy(this.gameObject);
        }

        /***************************************************************
         *                                     PRIVATE
         * **************************************************************/
    }
}