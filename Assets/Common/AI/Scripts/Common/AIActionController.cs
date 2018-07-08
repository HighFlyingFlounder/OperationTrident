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
            m_Controller.RPC(this, func.Method.Name, args);
            func(args);
        }

        public void RPC(Action func)
        {
            m_Controller.RPC(this, func.Method.Name);
            func();
        }

        public virtual void Move(bool isStart) { }

        public virtual void FindTarget(bool isStart) { }

        public virtual void DetectedTarget(bool isStart) { }

        public virtual void Shoot(Vector3 shootingPoint) { }

        public virtual void LookAt(Vector3 interestPoint) { }

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
    }
}
