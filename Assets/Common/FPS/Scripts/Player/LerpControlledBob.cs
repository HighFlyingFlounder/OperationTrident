using System;
using System.Collections;
using UnityEngine;

namespace OperationTrident.Player {
    [Serializable]
    public class LerpControlledBob {
        public float BobDuration;
        public float BobAmount;

        private float m_Offset = 0f;


        // 获取m_Offset的值
        public float Offset() {
            return m_Offset;
        }

        //在这个写成里面不断改变m_Offset的值
        public IEnumerator DoBobCycle() {
            // 让镜头往下移动
            float t = 0f;
            while (t < BobDuration) {
                m_Offset = Mathf.Lerp(0f, BobAmount, t / BobDuration);
                t += Time.deltaTime;
                //暂停FixedUpdate()的时间
                yield return new WaitForFixedUpdate();
            }

            // 让镜头往回移动
            t = 0f;
            while (t < BobDuration) {
                m_Offset = Mathf.Lerp(BobAmount, 0f, t / BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            m_Offset = 0f;
        }
    }
}
