using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room1
{
    public class ReactiveTarget : MonoBehaviour,NetSyncInterface
    {
        NetSyncController m_NetSyncController;
        //  血量
        public int health = 5;

        // 是否死亡
        private bool dead;

        public bool Dead
        {
            get
            {
                return dead;
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

        public void OnHit(string id,int damage)
        {
            //被射击打中的动画效果

            //
            if(GameMgr.instance==null) HitImplement(damage);
            if (GameMgr.instance.id == id)
            {
                HitImplement(damage);
                m_NetSyncController.RPC(this, "HitImplement", damage);
            }
        }

        public void HitImplement(int damage)
        {
            health -= damage;
            Debug.Log(health);
            if (health <= 0)
            {
                if (!dead)
                {
                    dead = true;
                    StartCoroutine(Die());
                }
            }
        }

        private IEnumerator Die()
        {
            this.transform.Rotate(-75, 0, 0);
            yield return new WaitForSeconds(1.5f);
            Destroy(this.gameObject);
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