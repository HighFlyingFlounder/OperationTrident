using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace OperationTrident.Common.AI
{
    public abstract class AIActionController : MonoBehaviour, NetSyncInterface
    {
        protected NetSyncController m_Controller;
        public void RPC<T>(Action<T> func, T args)
        {
            if (m_Controller == null) Debug.LogError("m_Controller");
            if (func.Method.Name == null) Debug.LogError("func.Method.Name");
            //m_Controller.RPC(this, func.Method.Name, args);
            AIController.instance.AIRPC(gameObject.name, func.Method.Name, args);
            func(args);
        }

        public void RPCDie()
        {
            m_Controller.RPC(this, "Die");
            //AIController.instance.AIRPC(gameObject.name, func.Method.Name);
            Die();
        }

        public void RPC(Action func)
        {
            m_Controller.RPC(this, func.Method.Name);
            //AIController.instance.AIRPC(gameObject.name, func.Method.Name);
            func();
        }

        public virtual void Move(bool isStart) { }

        public virtual void FindTarget(bool isStart) { }

        public virtual void DetectedTarget(bool isStart) { }

        public virtual void Shoot() { }
        public virtual void Shoot(Vector3 shootingPoint) { }
        public virtual void Shoot(string shootingTargetName) { }
        public virtual void StopShoot() { }

        public virtual void LookAt(Vector3 interestPoint) { }
        public virtual void LookAt(string targetName) { }
        public virtual void StopLookAt() { }

        public void Die() {
            StartCoroutine(Destroy());
        }

        public abstract IEnumerator Destroy();

        public void RecvData(SyncData data)
        {
        }

        public SyncData SendData()
        {
            return null;
        }

        public void Init(NetSyncController controller)
        {
            m_Controller = controller;
        }

        // 不需要RPC的函数
        public abstract bool DetectPlayer(Transform player);
    }
}
