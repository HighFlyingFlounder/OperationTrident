using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.EndingScene
{
    public class SceneDirector : MonoBehaviour
    {
        //逃生舱
        public GameObject m_EscapingCabin;

        //鲲
        public GameObject m_Kun;

        //爆炸生成器
        public ExplosionGenerator m_ExplosionGenerator;

        enum CameraState
        {
            ROAMING,//一开始缓慢移动,和靠近，用Timeline
            THIRD_PERSON,//第三人称看着逃生舱（可以控制）
            LOOKING_AT_KUN,//脱离第三人称绑定，看着鲲爆炸，不能控制
            VIDEO,//播放视频
        }

        //BGM 4小节的时间
        private const float m_BgmBarTime = 60.0f / 140.0f * 4.0f;

        //当前Camera的状态
        private CameraState m_CamState;

        //场景流逝总时间
        public UnityEngine.Playables.PlayableDirector m_TimeLineDirector;
        private double m_Time = 0.0f;

        //Timeline控制一个Camera_Directed，第三人称控制一个CameraThirdPerson
        //在Timeline里的activation track控制两个camera的enabled
        public Camera m_CamDirected;
        public Camera m_CamFree;

        //第三人称环视的Camera信息
        public float m_MouseLookSensitivity;
        private Vector3 m_ThirdPersonCamOffset;
        private Vector3 m_DestCamPos;//真正的Cam.transform要线性插值跟随这个target pos
        private Vector3 m_DestLookat;

        // Use this for initialization
        void Start()
        {
            m_CamState = CameraState.ROAMING;
        }

        // Update is called once per frame
        void Update()
        {
            m_Time = m_TimeLineDirector.time;
            switch (m_CamState)
            {
                case CameraState.ROAMING:
                    Update_Roaming();
                    break;

                case CameraState.THIRD_PERSON:
                    Update_ThirdPerson();
                    break;

                case CameraState.LOOKING_AT_KUN:
                    Update_LookingAtKun();
                    break;

                case CameraState.VIDEO:
                    Update_Video();
                    break;
            }

        }

        /************************************************
         *                           PRIVATE
         * **********************************************/
        private void Update_Roaming()
        {
            if (m_Time > m_BgmBarTime * 8)
            {
                //切换至第三人称状态
                m_CamState = CameraState.THIRD_PERSON;
                //初始化第三人称观察的参数
                m_CamFree.transform.position = m_CamDirected.transform.position;
                m_DestLookat = m_EscapingCabin.transform.position;
                m_DestCamPos = m_CamDirected.transform.position;
                m_ThirdPersonCamOffset = m_DestCamPos - m_EscapingCabin.transform.position;
            }
        }

        private void Update_ThirdPerson()
        {

            if (m_Time > m_BgmBarTime * (8 + 16))
            {
                //切至下一状态，不再绑定在玩家的第三人称，禁用控制
                //并初始化camera的destPos和destLookat
                m_CamState = CameraState.LOOKING_AT_KUN;
                m_DestLookat = m_EscapingCabin.transform.position;
                m_DestCamPos = m_CamFree.transform.position;
            }
            else
            {
                //鼠标控制target transform的旋转角度
                //实际Camera transform以一定比例Lerp向target transform
                float mouseX = m_MouseLookSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
                float mouseY = -m_MouseLookSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;

                //旋转视向量
                Quaternion deltaRotation = Quaternion.Euler(new Vector3(mouseY, mouseX, 0));
                m_ThirdPersonCamOffset = deltaRotation * m_ThirdPersonCamOffset;



                //实际Camera位置向pos/lookat插值
                const float lerpScale = 5.0f;
                m_DestCamPos = m_EscapingCabin.transform.position + m_ThirdPersonCamOffset;
                m_CamFree.transform.position = Vector3.Lerp(m_CamFree.transform.position, m_DestCamPos, lerpScale * Time.deltaTime);

                m_DestLookat = Vector3.Lerp(m_DestLookat, m_EscapingCabin.transform.position, lerpScale * Time.deltaTime);
                m_CamFree.transform.LookAt(m_DestLookat);
            }
        }

        private void Update_LookingAtKun()
        {
            if (m_Time > m_BgmBarTime * (8 + 16 + 16))
            {
                //切至下一状态，不再绑定在玩家的第三人称，禁用控制
                m_CamState = CameraState.VIDEO;
            }
            else
            {
                //计算新的需要插值到的camera pos/lookat
                const float lerpScale = 2.0f;
                m_DestCamPos += new Vector3(0, 1.5f, -15.0f) * Time.deltaTime;
                m_CamFree.transform.position = Vector3.Lerp(m_CamFree.transform.position, m_DestCamPos, lerpScale * Time.deltaTime);

                m_DestLookat = Vector3.Lerp(m_DestLookat, m_Kun.transform.position, lerpScale * Time.deltaTime);
                m_CamFree.transform.LookAt(m_DestLookat);

                //爆炸特效
                m_ExplosionGenerator.GenerateExplosion();
            }
        }

        private void Update_Video()
        {

        }

    }

}
