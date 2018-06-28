using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Weapon {
    [RequireComponent(typeof(Animator))]
    public class GrenadeIKController : MonoBehaviour {
        [SerializeField]
        private Transform RightHandTarget = null;

        [SerializeField]
        private float PutGrenadeTime;

        [SerializeField]
        private Transform CurveTarget = null;
        [SerializeField]
        private Transform PutTarget = null;

        //用于判断当前是否在执行协程
        public bool RuningCoroutine { get; private set; }

        [SerializeField]private Camera m_MainCamera;
        //private GameObject m_Player;
        [SerializeField]private Animator m_Animator;
        private Transform m_Grenade;
        private Throw m_Throw;


        //保存grenade在camera坐标系下的相对参数
        private Vector3 m_RelativePosition;
        private Quaternion m_RelativeRotation;

        private bool m_PickGrenade;

        // Use this for initialization
        void Start() {
            //m_MainCamera = Camera.main;
            //m_Player = GameObject.FindGameObjectWithTag("Player");
            //m_Animator = m_Player.GetComponent<Animator>();
            m_Grenade = this.transform;
            m_Throw = GetComponent<Throw>();

            //grenade在世界坐标系下的姿态
            Quaternion grenadeRotationInWorld = this.transform.rotation;
            //获得世界坐标系转换到camera坐标系的四元数
            Quaternion invWorldToCamQuaternion = Quaternion.Inverse(m_MainCamera.transform.rotation);
            //四元数相乘顺序与矩阵相乘顺序相同，后面的先做
            Quaternion grenadeRotationInCam = invWorldToCamQuaternion * grenadeRotationInWorld;
            //分别记录grenade在camera坐标系下的坐标和姿态
            m_RelativePosition = m_MainCamera.transform.InverseTransformPoint(this.transform.position);
            m_RelativeRotation = grenadeRotationInCam;

            //默认不拿手雷
            m_PickGrenade = false;
            m_Throw.enabled = false;
            //手雷默认处于放置状态
            m_Grenade.position = PutTarget.position;
            m_Grenade.rotation = PutTarget.rotation;
        }


        void OnAnimatorIK() {
            if (m_PickGrenade) {
                if (RightHandTarget != null) {
                    m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    m_Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
                    m_Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
                }
            }
        }

        //返回grenade的位置
        public Vector3 GetGrenadePosition() {
            return m_MainCamera.transform.TransformPoint(m_RelativePosition);
        }

        //返回grenade的姿态
        public Quaternion GetGrenadeRotation() {
            return m_MainCamera.transform.rotation * m_RelativeRotation;
        }

        public IEnumerator PutGrenade() {
            RuningCoroutine = true;
            //开始放手雷，解锁手雷和摄像机的绑定
            //m_Throw.m_LockGrenade = false;
            m_Throw.enabled = false;

            Vector3 p0 = m_Grenade.position;
            Vector3 p1 = CurveTarget.position;
            Vector3 p2 = PutTarget.position;

            Quaternion q0 = m_Grenade.rotation;
            Quaternion q1 = CurveTarget.rotation;
            Quaternion q2 = PutTarget.rotation;

            Quaternion Q1, Q2;

            float t = 0f;
            float deltaT;
            while (t < PutGrenadeTime) {
                deltaT = t / PutGrenadeTime;

                m_Grenade.position = (1 - deltaT) * (1 - deltaT) * p0 + 2 * deltaT * (1 - deltaT) * p1 + deltaT * deltaT * p2;

                Q1 = Quaternion.Slerp(q0, q1, deltaT);
                Q2 = Quaternion.Slerp(q1, q2, deltaT);
                m_Grenade.rotation = Quaternion.Slerp(Q1, Q2, deltaT);

                t += Time.deltaTime;
                //暂停FixedUpdate()的时间
                yield return new WaitForFixedUpdate();
            }

            m_PickGrenade = false;
            RuningCoroutine = false;
        }

        public IEnumerator PickGrenade() {
            RuningCoroutine = true;
            m_PickGrenade = true;

            //修改手雷的位置
            m_Grenade.position = PutTarget.position;
            m_Grenade.rotation = PutTarget.rotation;

            Vector3 p0 = PutTarget.position;
            Vector3 p1 = CurveTarget.position;
            Vector3 p2 = GetGrenadePosition();

            Quaternion q0 = PutTarget.rotation;
            Quaternion q1 = CurveTarget.rotation;
            Quaternion q2 = GetGrenadeRotation();

            Quaternion Q1, Q2;

            float t = 0f;
            float deltaT;

            while (t < PutGrenadeTime) {
                deltaT = t / PutGrenadeTime;

                m_Grenade.position = (1 - deltaT) * (1 - deltaT) * p0 + 2 * deltaT * (1 - deltaT) * p1 + deltaT * deltaT * p2;

                Q1 = Quaternion.Slerp(q0, q1, deltaT);
                Q2 = Quaternion.Slerp(q1, q2, deltaT);
                m_Grenade.rotation = Quaternion.Slerp(Q1, Q2, deltaT);
                t += Time.deltaTime;
                //暂停FixedUpdate()的时间
                yield return new WaitForFixedUpdate();
            }

            //拿完手雷，锁定摄像机
            //m_Throw.m_LockGrenade = true;
            m_Throw.enabled = true;
            RuningCoroutine = false;
        }
    }
}
