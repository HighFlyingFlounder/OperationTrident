using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using OperationTrident.EventSystem;

namespace OperationTrident.Room1
{
    public class ReactiveTarget : MonoBehaviour,NetSyncInterface
    {
        NetSyncController m_NetSyncController;
        //  血量
        public int health = 5;

        // 是否死亡
        private bool dead;
        private bool sendDeadMessage = false;

        public bool Dead
        {
            get
            {
                return dead;
            }
        }

        [SerializeField]
        private bool isPlayer;

        public bool IsPlayer
        {
            get
            {
                return isPlayer;
            }
        }

        // Use this for initialization
        void Start()
        {
            dead = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnHit(string id,bool fromAI,int damage)
        {
            //被射击打中的动画效果

            //单机状态
            if(GameMgr.instance==null)
                HitImplement(damage);
            else if (GameMgr.instance.id == id)//本地玩家造成伤害
            {
                if (gameObject.name == id)//自己对自己不能造成的伤害
                    return;
                HitImplement(damage);
                m_NetSyncController.RPC(this, "HitImplement", damage);
            }
            else if(fromAI && GameMgr.instance.isMasterClient)//MasterClient机器上的AI造成了伤害
            {
                HitImplement(damage);
                m_NetSyncController.RPC(this, "HitImplement", damage);
            }
        }

        public void HitImplement(int damage)
        {
            health -= damage;
            //Debug.Log(health);
            if (health <= 0)
            {
                if (!dead)
                {
                    dead = true;
                    AIController.instance.DestroyAI(gameObject.name);
                    StartCoroutine(Die());
                }
            }
        }

        

        private IEnumerator Die()
        {
            transform.Rotate(-75, 0, 0);
            if (!isPlayer)
            {
                yield return new WaitForSeconds(1.5f);
                Destroy(gameObject);
            }
            else
            {
                GetComponent<FPS.Player.MovementController>().enabled = false;
                GetComponent<FPS.Player.MouseRotator>().enabled = false;
                if (!sendDeadMessage)
                {
                    SendDead();
                    sendDeadMessage = true;
                }

                
            }
        }

        public void SendDead()
        {
            ProtocolBytes proto = new ProtocolBytes();
            Debug.Log("dead");
            proto.AddString("Dead");
            NetMgr.srvConn.Send(proto);
        }



        public void RecvData(SyncData data)
        {
        }

        public SyncData SendData()
        {
            SyncData data = new SyncData();
            return data;
        }

        public void Init(NetSyncController controller)
        {
            m_NetSyncController = controller;
        }
    }
}