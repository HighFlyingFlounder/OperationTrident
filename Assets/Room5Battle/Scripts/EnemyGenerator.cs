using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room5
{
    public class EnemyGenerator : MonoBehaviour
    {
        //在一个正方体区域内随机instantiate一堆prefab
        public static void SpawnEnemy_BoxRandom(GameObject prefab, int enemyNum, Transform center, Vector3 boxSize)
        {
            //敌人在某个位置隔壁一定box范围内生成
            for (int i = 0; i < enemyNum; ++i)
            {
                float x_offset = Random.Range(-boxSize.x / 2.0f, boxSize.x / 2.0f);
                float y_offset = Random.Range(-boxSize.y / 2.0f, boxSize.y / 2.0f);
                float z_offset = Random.Range(-boxSize.z / 2.0f, boxSize.z / 2.0f);
                Instantiate(
                    prefab,
                    center.position + new Vector3(x_offset, y_offset, z_offset),
                    center.rotation);
            }
        }

        //在指定位置内随机instantiate一个prefab
        public static void SpawnEnemy_ExactPos(GameObject prefab, Transform center)
        {
           //敌人在某个位置隔壁一定box范围内生成
           Instantiate(prefab,center.position , center.rotation);
        }

    }
}