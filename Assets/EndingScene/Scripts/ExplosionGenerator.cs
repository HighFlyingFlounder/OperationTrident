using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.EndingScene
{
    //instantiate爆炸prefab的工具类
    public class ExplosionGenerator : MonoBehaviour {

        //爆炸prefab
        public GameObject m_ExplosionPrefab;
        public Transform m_KunTransform;

        //随机取得一个时间阈值，过了这个阈值就在box里面随机位置生成一个新的爆炸粒子系统
        private float m_ExplodeMaxInterval;
        private float m_CurrentTimeThrehold;
        private float m_Time;

        void Start()
        {
            m_ExplodeMaxInterval = 0.0f;
        }

        // Update is called once per frame
        public void GenerateExplosion() {
            m_Time += Time.deltaTime;
            if (m_Time > m_CurrentTimeThrehold)
            {
                //敌人在某个位置隔壁一定box范围内生成
                Vector3 boxSize = new Vector3(900, 500, 600);
                    float x_offset = Random.Range(-boxSize.x / 2.0f, boxSize.x / 2.0f);
                    float y_offset = Random.Range(-boxSize.y / 2.0f, boxSize.y / 2.0f);
                    float z_offset = Random.Range(-boxSize.z / 2.0f, boxSize.z / 2.0f);
                    GameObject obj = Instantiate(
                        m_ExplosionPrefab,
                        m_KunTransform.position + new Vector3(x_offset, y_offset, z_offset),
                         m_KunTransform.rotation);

                //过几秒删除爆炸的gameObject
                StartCoroutine(DestroyExplosionPS(obj));

                //重置一下爆炸的计时器
                m_Time = 0.0f;
                m_CurrentTimeThrehold = Random.Range(0, m_ExplodeMaxInterval);
            }
        }

        public void SetExplodeMaxInterval(float maxInterval)
        {
            m_ExplodeMaxInterval = maxInterval;
        }

        private IEnumerator DestroyExplosionPS(GameObject obj)
        {
            yield return new WaitForSeconds(6.0f);
            Destroy(obj);
        }
    }
}