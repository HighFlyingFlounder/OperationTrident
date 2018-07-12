using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OperationTrident.Common;

namespace OperationTrident.FPS.Weapons {
    public class Explosion : MonoBehaviour {
        [Tooltip("不检测的物理层名称")]
        public string LayerMaskName;
        [Tooltip("爆炸击退力")]
        public float ExplosionForce = 5.0f;
        [Tooltip("爆炸的半径")]
        public float ExplosionRadius = 10.0f;
        [Tooltip("是否晃动摄像机")]
        public bool ShakeCamera = true;
        [Tooltip("摄像机晃动剧烈程度")]
        public float CameraShakeViolence = 0.5f;
        [Tooltip("是否造成伤害")]
        public bool CauseDamage = true;
        [Tooltip("伤害大小")]
        public float Damage = 10.0f;

        //将Start设置为协程，自动开始协程
        IEnumerator Start() {
            //暂停一帧，避免有碎片还没被创建
            yield return null;

            //获取物理层
            int layer = LayerMask.NameToLayer(LayerMaskName);
            Collider[] cols;
            Collider[] colsWithoutMask;

            if (layer != -1) {
                //创建物理遮罩
                LayerMask mask = ~(1 << layer);
                //爆炸范围内的物体
                cols = Physics.OverlapSphere(transform.position, ExplosionRadius, mask);
            } else {
                Debug.LogWarning("Can not find the layer to create a layer mask");
                //爆炸范围内的物体
                cols = Physics.OverlapSphere(transform.position, ExplosionRadius);
            }

            colsWithoutMask = Physics.OverlapSphere(transform.position, ExplosionRadius);

            //造成伤害
            if (CauseDamage) {
                foreach (Collider col in cols) {
                    float damageAmount = Damage * (1 / Vector3.Distance(transform.position, col.transform.position));

                    //造成伤害
                    ReactiveTarget target = col.gameObject.GetComponent<ReactiveTarget>();
                    if (target) {
                        //第二个参数为true，不分敌我进行伤害
                        target.OnHit(this.gameObject.name, true, Damage);
                    }
                }
            }

            //用来保存爆炸范围内所有Rigidbody信息
            List<Rigidbody> rigidbodies = new List<Rigidbody>();

            foreach (Collider col in cols) {
                //保存范围内所有Rigidbody信息
                if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody)) {
                    rigidbodies.Add(col.attachedRigidbody);
                }
            }

            foreach (Rigidbody rb in rigidbodies) {
                rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 1, ForceMode.Impulse);
            }

            foreach(Collider col in colsWithoutMask) {
                //如果子对象包含Vibration脚本，就调用函数进行抖动
                if (ShakeCamera && col.transform.GetComponentInChildren<Vibration>() != null) {
                    float shakeViolence = 1 / (Vector3.Distance(transform.position, col.transform.position) * CameraShakeViolence);
                    col.transform.GetComponentInChildren<Vibration>().StartShakingRandom(-shakeViolence, shakeViolence, -shakeViolence, shakeViolence);
                }
            }
        }
    }
}
