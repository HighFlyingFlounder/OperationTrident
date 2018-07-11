using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Common.AI;

namespace OperationTrident.Common {
    public class ReactiveTarget : MonoBehaviour,NetSyncInterface, AI.AIReacitiveInterface {
        public bool IsPlayer = false;
        public bool CanBeHurt = true;
        public bool ShowHealth = false;
        public float MaxHealth = 100;
        public bool ReplaceWhenDie = false;
        public GameObject DeadReplacement;
        public bool MakeExplosion = false;
        public GameObject Explosion;
        public bool UseDeadCamera = false;
        public GameObject DeathCamera;

        public AudioClip AC = null;

        private float m_CurrentHealth;
        private bool m_Death;
        private bool m_HasSendDeadMessage;

        private bool _isParalyzed;
        private float _EMPEffectTime;

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

                //AudioSource.PlayClipAtPoint(AC, this.transform.position);

                HitImplement(damage);
                m_NetSyncController.RPC(this, "HitImplement", damage);
            }
            //MasterClient上的AI造成的伤害，其他玩家只进行伤害同步
            else if (fromAI && GameMgr.instance.isMasterClient){
                HitImplement(damage);
                m_NetSyncController.RPC(this, "HitImplement", damage);
            }
        }

        #region RPC函数
        public void HitImplement(float damage) {
            //如果不能收到伤害，不执行任何操作
            Debug.LogFormat("GameObject {0} get Damage {1}", this.gameObject.name, damage);
            if (!CanBeHurt) {
                return;
            }
            m_CurrentHealth -= damage;
            if (m_CurrentHealth <= 0f && m_Death == false) {
                m_Death = true;
                //调用死亡函数, 机器人不进行调用
                if (IsPlayer)
                    PlayerDie();
                // 从controller List中移除现有AI
                else
                {
                    AIController.instance.DestroyAI(this.gameObject.name);
                    AIActionController controller =  gameObject.GetComponent<AIActionController>();
                    if (controller == null) Debug.LogError("Controller Not Found");
                    controller.Die();
                }
                    
            }
        }
        #endregion

        private void PlayerDie() {
            //生成替代模型
            if (ReplaceWhenDie) {
                if(DeadReplacement != null) {
                    GameObject replacement = Instantiate(DeadReplacement, transform.position + Vector3.up * 0.3f, transform.rotation);
                    replacement.transform.localScale = this.transform.localScale;
                } else {
                    Debug.LogWarning("Can not find DeadReplacement to Instantiate");
                }
            }

            if (UseDeadCamera) {
                if(DeathCamera != null) {
                    DeathCamera.SetActive(true);
                    DeathCamera.transform.SetParent(null);
                    //对摄像机进行移动
                    CameraAnimation();
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
            /*
            else {
                if (MakeExplosion) {
                    if (Explosion) {
                        Instantiate(Explosion, transform.position, transform.rotation);
                    } else {
                        Debug.LogWarning("Can not find the Explosion to Instantiate");
                    }
                }

                // 从controller List中移除现有AI
                AIController.instance.DestroyAI(this.gameObject.name);

                //如果是AI，进行销毁
                Destroy(this.gameObject);
                //AIController.instance.DestroyAI(gameObject.name);
            }
            */
        }

        //这里可以设置镜头的动画
        private void CameraAnimation() {

        }

        public void SendDead() {
            ProtocolBytes proto = new ProtocolBytes();
            proto.AddString("Dead");
            NetMgr.srvConn.Send(proto);
        }

        private void Update()
        {
            if (_isParalyzed)
            {
                _EMPEffectTime -= Time.deltaTime;
                if(_EMPEffectTime < 0)
                {
                    _EMPEffectTime = 0;
                    _isParalyzed = false;
                }
            }
        }

        public void OnEMP(float effectTime)
        {
            if (!IsAlive)
                return;

            OnEMPImplement(effectTime);
            m_NetSyncController.RPC(this, "OnEMPImplement", effectTime);
        }

        public void OnEMPImplement(float effectTime)
        {
            _isParalyzed = true;
            _EMPEffectTime = effectTime;
        }

        public bool isDeath
        {
            get
            {
                return m_Death;
            }
        }

        public bool IsParalyzed
        {
            get
            {
                return _isParalyzed;
            }
        }

        public bool IsAlive
        {
            get
            {
                return !m_Death;
            }
        }

        public float HPPercentage
        {
            get
            {
                return m_CurrentHealth / MaxHealth;
            }
        }

        #region 接口
        public void Init(NetSyncController controller) {
            m_NetSyncController = controller;
        }

        public void RecvData(SyncData data) {
        }

        public SyncData SendData() {
            return null;
        }
        #endregion
    }
}
