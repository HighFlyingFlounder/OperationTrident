using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace OperationTrident.Common.AI
{
    public abstract class AIActionController : MonoBehaviour, NetSyncInterface
    {
        public AudioSource explosionAudio;
        protected NetSyncController m_Controller;
        public void RPC<T>(Action<T> func, T args)
        {
            if (m_Controller == null) Debug.LogError("m_Controller");
            if (func.Method.Name == null) Debug.LogError("func.Method.Name");
            if (AIController.instance != null)
                AIController.instance.AIRPC(gameObject.name, func.Method.Name, args);
            func(args);
        }

        public void RPC(Action func)
        {
            m_Controller.RPC(this, func.Method.Name);
            func();
        }

        public virtual void Move(bool isStart) { }

        public virtual void FindTarget(bool isStart) { }

        public virtual void Shoot() { }
        public virtual void ShootWithVector3(Vector3 shootingPoint) { }
        public virtual void ShootWithTargetName(string shootingTargetName) { }
        public virtual void StopShoot() { }

        public virtual void LookAtWithVector3(Vector3 interestPoint) { }
        public virtual void LookAtWithTargetName(string targetName) { }
        public virtual void StopLookAt() { }

        public void StopAllAction()
        {
            StopShoot();
            StopLookAt();
        }

        public void Die()
        {
            StopAllAction();
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
