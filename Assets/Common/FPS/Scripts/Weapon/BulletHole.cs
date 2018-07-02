using UnityEngine;
using System.Collections;

namespace OperationTrident.FPS.Weapons {
    public class BulletHole : MonoBehaviour {
        //拥有弹孔Mesh的Object
        public GameObject bulletHoleMesh;
        //是否使用弹孔池，如果不使用弹孔池，那么直接销毁；若使用，则不销毁以便重复利用
        public bool usePooling = true; 
        //弹孔持续的时间
        public float lifetime = 28.0f;
        //弹孔开始消失的时间
        public float startFadeTime = 10.0f;

        //用于计时
        private float m_Timer;
        //弹孔消失的速度，用于插值
        public float m_FadeRate = 0.001f;
        //弹孔材质最终的颜色
        private Color m_TargetColor;

        // Use this for initialization
        void Start() {
            //初始化计时器
            m_Timer = 0.0f;

            //初始化材质的颜色
            m_TargetColor = bulletHoleMesh.GetComponent<Renderer>().material.color;
            m_TargetColor.a = 0.0f;

            // Attach the bullet hole to the hit GameObject ***- no longer used because of the pooling system
            //AttachToParent();
        }


        // Update is called once per frame
        void Update() {
            if (!usePooling)
                FadeAndDieOverTime();

        }

        // This method is called when a bullet hole is moved to a different location/rotation, ready to be used again
        public void Refresh() {
            AttachToParent();
        }

        // Make the bullet hole "stick" to the object it hit by parenting it
        private void AttachToParent() {
            RaycastHit hit;
            if (Physics.Raycast(bulletHoleMesh.transform.position, -bulletHoleMesh.transform.up, out hit, 0.1f)) {
                transform.parent = hit.collider.transform;
            } else {
                Destroy(transform.gameObject);
            }
        }


        private void FadeAndDieOverTime() {
            //计时
            m_Timer += Time.deltaTime;

            //到了消失时间之后弹孔开始慢慢消失
            if (m_Timer >= startFadeTime) {
                bulletHoleMesh.GetComponent<Renderer>().material.color = Color.Lerp(
                    bulletHoleMesh.GetComponent<Renderer>().material.color, 
                    m_TargetColor, 
                    m_FadeRate * Time.deltaTime
                    );
            }

            //到了销毁时间之后销毁弹孔
            if (m_Timer >= lifetime) {
                Destroy(gameObject);
            }
        }
    }


}