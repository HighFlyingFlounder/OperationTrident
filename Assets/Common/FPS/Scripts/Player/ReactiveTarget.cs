using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.FPS.Player {
    public class ReactiveTarget : MonoBehaviour,NetSyncInterface {
        public bool IsPlayer = true;
        public bool CanBeHurt = true;
        public bool ShowHealth = true;
        public float MaxHealth = 100;
        public bool ReplaceWhenDie = false;
        public GameObject DeadReplacement;
        public bool MakeExplosion = false;
        public GameObject Explosion;
        public bool UseDeadCamera = false;
        public GameObject DeathCamera;

        private float m_CurrentHealth;
        private bool m_Death;
        private bool m_HasSendDeadMessage;
        private NetSyncController m_NetSyncController;

        // Use this for initialization
        void Start() {
            m_CurrentHealth = MaxHealth;
            m_Death = false;
            m_HasSendDeadMessage = false;
        }

        private void OnGUI() {
            if (ShowHealth) {
                GUI.color = Color.red;
                GUI.Label(new Rect(10, Screen.height - 20, 100, 20), "Health: " + m_CurrentHealth);
            }
        }

        public void OnHit(string id, bool fromAI, float damage) {
            //单机状态
            if (GameMgr.instance == null) {
                HitImplement(damage);
            }
            //本地玩家造成伤害
            else if (GameMgr.instance.id == id){
                //自己对自己不能造成的伤害
                if (gameObject.name == id) {
                    return;
                }
                HitImplement(damage);
                m_NetSyncController.RPC(this, "HitImplement", damage);
            }
            //MasterClient上的AI造成的伤害
            else if (fromAI && GameMgr.instance.isMasterClient){
                HitImplement(damage);
                m_NetSyncController.RPC(this, "HitImplement", damage);
            }
        }

        #region RPC函数
        public void HitImplement(float damage) {
            //如果不能收到伤害，不执行任何操作
            if (!CanBeHurt) {
                return;
            }

            m_CurrentHealth -= damage;

            if(m_CurrentHealth <= 0f && m_Death == false) {
                m_Death = true;
                //调用死亡函数
                Die();
            }
        }
        #endregion

        private void Die() {
            if (IsPlayer) {
                //生成替代模型
                if (ReplaceWhenDie) {
                    if(DeadReplacement != null) {
                        Instantiate(DeadReplacement, transform.position, transform.rotation);
                    } else {
                        Debug.LogWarning("Can not find DeadReplacement to Instantiate");
                    }
                }

                if (UseDeadCamera) {
                    if(DeathCamera != null) {
                        DeathCamera.SetActive(true);
                        DeathCamera.transform.SetParent(null);
                        //这里可以设置镜头上移
                    } else {
                        Debug.LogWarning("Can not find DeathCamera to active");
                    }
                }

                if (!m_HasSendDeadMessage) {
                    SendDead();
                    m_HasSendDeadMessage = true;
                }

                //销毁自身
                Destroy(this.gameObject);
            } else {
                if (MakeExplosion) {
                    if (Explosion) {
                        Instantiate(Explosion, transform.position, transform.rotation);
                    } else {
                        Debug.LogWarning("Can not find the Explosion to Instantiate");
                    }
                }

                //如果是AI，进行销毁
                Destroy(this.gameObject);
                //AIController.instance.DestroyAI(gameObject.name);
            }
        }

        public void SendDead() {
            ProtocolBytes proto = new ProtocolBytes();
            proto.AddString("Dead");
            NetMgr.srvConn.Send(proto);
        }

        #region 接口
        public void Init(NetSyncController controller) {
            m_NetSyncController = controller;
        }

        public void RecvData(SyncData data) {
            throw new System.NotImplementedException();
        }

        public SyncData SendData() {
            return new SyncData();
        }
        #endregion
    }
}
