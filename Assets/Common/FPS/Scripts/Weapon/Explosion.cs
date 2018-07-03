using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OperationTrident.FPS.Weapons {
    public class Explosion : MonoBehaviour {
        //public bool shooterAISupport = false; 
        //public bool bloodyMessSupport = false;
        //public int weaponType = 0;             

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

        IEnumerator Start() {
            //暂停一帧，避免有碎片还没被创建
            yield return null;

            //爆炸范围内的物体
            Collider[] cols = Physics.OverlapSphere(transform.position, ExplosionRadius);

            //造成伤害
            if (CauseDamage) {
                foreach (Collider col in cols) {
                    float damageAmount = Damage * (1 / Vector3.Distance(transform.position, col.transform.position));

                    //col.GetComponent<Collider>().gameObject.SendMessageUpwards("ChangeHealth", -damageAmount, SendMessageOptions.DontRequireReceiver);

                    //if (shooterAISupport) {
                    //    col.transform.SendMessageUpwards("Damage", damageAmount, SendMessageOptions.DontRequireReceiver);
                    //}

                    //if (bloodyMessSupport) {
                    //    if (col.gameObject.layer == LayerMask.NameToLayer("Limb")) {
                    //        Vector3 directionShot = col.transform.position - transform.position;

                    //        /*
                    //        if (col.gameObject.GetComponent<Limb>())
                    //        {
                    //            RaycastHit limbHit;

                    //            if (Physics.Raycast(transform.position, directionShot, out limbHit))
                    //            {
                    //                if (limbHit.collider.gameObject.tag == col.gameObject.tag)
                    //                {
                    //                    GameObject parent = col.gameObject.GetComponent<Limb>().parent;
                    //                    CharacterSetup character = parent.GetComponent<CharacterSetup>();
                    //                    character.ApplyDamage(damage, col.gameObject, weaponType, directionShot, Camera.main.transform.position);
                    //                }
                    //            }
                    //        }
                    //        */
                    //    }
                    //}


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
