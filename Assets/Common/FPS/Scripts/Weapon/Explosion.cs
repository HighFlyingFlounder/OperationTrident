using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OperationTrident.FPS.Player;

namespace OperationTrident.FPS.Weapons {
    public class Explosion : MonoBehaviour {
        public string LayerMaskName;
        //爆炸击退力
        public float ExplosionForce = 5.0f;
        //爆炸的半径
        public float ExplosionRadius = 10.0f;
        //是否晃动摄像机
        public bool ShakeCamera = true;
        //摄像机晃动剧烈程度
        public float CameraShakeViolence = 0.5f;
        //是否造成伤害
        public bool CauseDamage = true;
        //伤害大小
        public float Damage = 10.0f;

        //将Start设置为协程，自动开始协程
        IEnumerator Start() {
            ////暂停一帧，避免有碎片还没被创建
            //yield return null;
            yield return new WaitForSeconds(0.1f);

            //获取物理层
            int layer = LayerMask.NameToLayer(LayerMaskName);
            Collider[] cols;
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

            //造成伤害
            if (CauseDamage) {
                foreach (Collider col in cols) {
                    float damageAmount = Damage * (1 / Vector3.Distance(transform.position, col.transform.position));

                    //造成伤害
                    ReactiveTarget target = col.gameObject.GetComponent<ReactiveTarget>();
                    if (target) {
                        target.OnHit(this.gameObject.name, false, Damage);
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

                //如果子对象包含Vibration脚本，就调用函数进行抖动
                if (ShakeCamera && col.transform.GetComponentInChildren<Vibration>() != null) {
                    float shakeViolence = 1 / (Vector3.Distance(transform.position, col.transform.position) * CameraShakeViolence);
                    col.transform.GetComponentInChildren<Vibration>().StartShakingRandom(-shakeViolence, shakeViolence, -shakeViolence, shakeViolence);
                }
            }

            foreach (Rigidbody rb in rigidbodies) {
                rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 1, ForceMode.Impulse);
            }
        }
    }
}
