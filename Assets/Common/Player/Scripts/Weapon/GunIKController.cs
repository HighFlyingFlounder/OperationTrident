/* *
 * 枪移动到某个角度的时候，机器人的手会出现消失的bug
 * 需要勾选机器人模型Skined Mesh Renderer的Update When Offscreen
 * 保持机器人模型一直更新
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Weapon {
    //使用IK，需要在当前脚本绑定的物体上添加一个Animator
    //并且这个Animator要绑定一个开启了IK Pass的Controller
    [RequireComponent(typeof(Animator))]
    public class GunIKController : MonoBehaviour {
        [SerializeField]
        private Transform LookAtObj = null;
        [SerializeField]
        private Transform RightHandObj = null;
        [SerializeField]
        private Transform LeftHandObj = null;
        [SerializeField]
        private float PutGunTime = 5.0f;
        [SerializeField]
        private Transform CurveTarget;
        [SerializeField]
        private Transform LooseTarget;
        [SerializeField]
        private Transform PutTarget;

        //使用Inspector进行赋值
        [SerializeField]
        private Camera m_MainCamera;
        [SerializeField]
        private Animator m_Animator;

        //用于判断当前是否在执行协程
        public bool RuningCoroutine { get; private set; }

        private Transform m_Gun;
        //private Fire m_Fire;

        //保存gun在camera坐标系下的相对参数
        private Vector3 m_RelativePosition;
        private Quaternion m_RelativeRotation;

        private bool m_PickGun;
        private bool m_PuttingGun;
        private bool m_PickingGun;

        // 获取组件的引用
        void Start() {
            RuningCoroutine = false;

            m_Gun = this.transform;
            //m_Fire = GetComponent<Fire>();

            //gun在世界坐标系下的姿态
            Quaternion gunRotationInWorld = this.transform.rotation;
            //获得世界坐标系转换到camera坐标系的四元数
            Quaternion invWorldToCamQuaternion = Quaternion.Inverse(m_MainCamera.transform.rotation);
            //四元数相乘顺序与矩阵相乘顺序相同，后面的先做
            Quaternion gunRotationInCam = invWorldToCamQuaternion * gunRotationInWorld;
            //分别记录gun在camera坐标系下的坐标和姿态
            m_RelativePosition = m_MainCamera.transform.InverseTransformPoint(this.transform.position);
            m_RelativeRotation = gunRotationInCam;

            m_PickGun = true;
            m_PuttingGun = false;
            m_PickingGun = false;
        }

        // 使用IK的逻辑，这个函数在Update之后调用，在LateUpdate之前调用
        void OnAnimatorIK() {
            if (m_PickGun) {
                //拿枪的过程，保持右手拿枪
                //拿完枪之后，设置左右手拿枪
                if (m_PickingGun) {
                    if (RightHandObj != null) {
                        m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        m_Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandObj.position);
                        m_Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandObj.rotation);
                    }
                } else {
                    if (LookAtObj != null) {
                        m_Animator.SetLookAtWeight(1);
                        m_Animator.SetLookAtPosition(LookAtObj.position);
                    }

                    if (RightHandObj != null) {
                        m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        m_Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandObj.position);
                        m_Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandObj.rotation);
                    }

                    if (LeftHandObj != null) {
                        m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                        m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                        m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandObj.position);
                        m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandObj.rotation);
                    }
                }
            } else {
                //收枪的过程，保持右手拿枪
                if (m_PuttingGun) {
                    if (RightHandObj != null) {
                        m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        m_Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandObj.position);
                        m_Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandObj.rotation);
                    }
                }
            }
        }

        //返回gun的位置
        public Vector3 GetGunPosition() {
            return m_MainCamera.transform.TransformPoint(m_RelativePosition);
        }

        //返回gun的姿态
        public Quaternion GetGunRotation() {
            return m_MainCamera.transform.rotation * m_RelativeRotation;
        }

        public IEnumerator PutGun() {
            RuningCoroutine = true;
            m_PickGun = false;
            m_PuttingGun = true;
            ////开始放枪，解除摄像机和枪的绑定
            //m_Fire.enabled = false;

            Vector3 p0 = m_Gun.position;
            Vector3 p1 = CurveTarget.position;
            Vector3 p2 = LooseTarget.position;

            Quaternion q0 = m_Gun.rotation;
            Quaternion q1 = CurveTarget.rotation;
            Quaternion q2 = LooseTarget.rotation;

            Quaternion Q1, Q2;

            float t = 0f;
            float deltaT;
            while (t < PutGunTime) {
                deltaT = t / PutGunTime;

                m_Gun.position = (1 - deltaT) * (1 - deltaT) * p0 + 2 * deltaT * (1 - deltaT) * p1 + deltaT * deltaT * p2;

                Q1 = Quaternion.Slerp(q0, q1, deltaT);
                Q2 = Quaternion.Slerp(q1, q2, deltaT);
                m_Gun.rotation = Quaternion.Slerp(Q1, Q2, deltaT);
                t += Time.deltaTime;
                //暂停FixedUpdate()的时间
                yield return new WaitForFixedUpdate();
            }

            m_Gun.position = PutTarget.position;
            m_Gun.rotation = PutTarget.rotation;

            m_PuttingGun = false;
            RuningCoroutine = false;
        }

        public IEnumerator PickGun() {
            RuningCoroutine = true;
            m_PickGun = true;
            m_PickingGun = true;

            //修改枪的位置
            m_Gun.position = LooseTarget.position;
            m_Gun.rotation = LooseTarget.rotation;

            Vector3 p0 = LooseTarget.position;
            Vector3 p1 = CurveTarget.position;
            Vector3 p2 = GetGunPosition();


            Quaternion q0 = LooseTarget.rotation;
            Quaternion q1 = CurveTarget.rotation;
            Quaternion q2 = GetGunRotation();

            Quaternion Q1, Q2;

            float t = 0f;
            float deltaT;

            while (t < PutGunTime) {
                deltaT = t / PutGunTime;

                m_Gun.position = (1 - deltaT) * (1 - deltaT) * p0 + 2 * deltaT * (1 - deltaT) * p1 + deltaT * deltaT * p2;

                Q1 = Quaternion.Slerp(q0, q1, deltaT);
                Q2 = Quaternion.Slerp(q1, q2, deltaT);
                m_Gun.rotation = Quaternion.Slerp(Q1, Q2, deltaT);
                t += Time.deltaTime;
                //暂停FixedUpdate()的时间
                yield return new WaitForFixedUpdate();
            }

            ////拿完枪，锁定摄像机
            //m_Fire.enabled = true;
            m_PickingGun = false;
            RuningCoroutine = false;
        }
    }
}
