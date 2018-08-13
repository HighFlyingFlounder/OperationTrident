using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room5
{
    public class TokamakCore : MonoBehaviour,NetSyncInterface
    {
        private NetSyncController mSynControler;

        //可交互对象（在unity editor中初始化）
        public InteractiveObject m_CoreInteractiveObj;

        //停止运作之后的材质
        public Material m_ShutdownCoreMat;

        //漂浮&旋转的原点
        private Vector3 m_OriginPos;

        //用于漂浮的递增factor
        private static float m_Time;

        //漂浮的幅度
        public float m_FloatingAmplitude;

        //漂浮的频率
        public float m_FloatingFrequency;

        //旋转的速率
        public float m_RotatingVelocity;

        //是否已停止运作
        private bool m_IsStop = false;

        // Use this for initialization
        void Start()
        {
            m_CoreInteractiveObj.Initialize(
                "TokamakCore", KeyCode.F, 1.0f,
                "^w按住^yF^w拿取","^w正在拿取...");
            m_OriginPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            m_Time = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_IsStop)
            {
                m_CoreInteractiveObj.UpdateState();
                //拿到了托卡马克之心就
                if(m_CoreInteractiveObj.IsInteractionDone())
                {
                    mSynControler.RPC(this, "destroyCore");
                    destroyCore();
                }
            }
            else
            {
                //Quaternion rot =  Quaternion.Euler(0, Time.deltaTime * m_RotatingVelocity, 0);
                this.transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * m_RotatingVelocity, Space.World);

                m_Time += Time.deltaTime;
                this.transform.position = m_OriginPos + new Vector3(0, Mathf.Sin(m_Time * m_FloatingFrequency), 0) * m_FloatingAmplitude;
            }
        }

        //关闭反应柱后，关灯，改Emissive，停止旋转。应该由外界调用
        public void Shutdown()
        {
            m_IsStop = true;
            //换个关了灯的材质
            GetComponent<MeshRenderer>().material = m_ShutdownCoreMat;
            //关个核心自带的灯
            GetComponentInChildren<Light>().enabled = false;
        }

        public void OnGUI()
        {
            //核心还没被拿到的话
            if (m_IsStop && m_CoreInteractiveObj!=null)
            {
                m_CoreInteractiveObj.RenderGUI();
            }
        }

        public void destroyCore()
        {
            Destroy(this.gameObject);
            //GetComponent<MeshRenderer>().enabled = false;
        }

        public void RecvData(SyncData data)
        {
            
        }

        public SyncData SendData()
        {
            return null;
        }

        public void Init(NetSyncController controller)
        {
            mSynControler = controller;
        }
    }
}