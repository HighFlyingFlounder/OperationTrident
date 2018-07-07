using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Room5
{
    public class TokamakReactorPillar : MonoBehaviour
    {

        //冷却的时候要改变反应柱Material的Emissive
        public Material m_ReactorPillarCoolingDownMat;

        //关闭的时候要改变反应柱Material的Emissive
        public Material m_ReactorPillarShutdownMat;

        //还要播放下降的动画
        public UnityEngine.Playables.PlayableDirector m_DescendingAnimDirector;

        private bool m_IsShuttingDown = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //关闭托卡马卡反应柱
        public void Shutdown()
        {
            //柱子关掉Emissive（替换成无Emissive的material）
            GetComponentsInChildren<MeshRenderer>()[0].material = m_ReactorPillarShutdownMat;
            GetComponentsInChildren<MeshRenderer>()[1].material = m_ReactorPillarShutdownMat;
            GetComponentsInChildren<MeshRenderer>()[2].material = m_ReactorPillarShutdownMat;
            m_DescendingAnimDirector.Play();
        }

        //托卡马卡反应柱开始冷却（变色）
        public void StartCoolDownProcedure()
        {
            //柱子关掉Emissive（替换成无Emissive的material）
            GetComponentsInChildren<MeshRenderer>()[0].material = m_ReactorPillarCoolingDownMat;
            GetComponentsInChildren<MeshRenderer>()[1].material = m_ReactorPillarCoolingDownMat;
            GetComponentsInChildren<MeshRenderer>()[2].material = m_ReactorPillarCoolingDownMat;
        }
    }
}