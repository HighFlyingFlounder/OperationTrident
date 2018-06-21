using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.CrossInput;

namespace OperationTrident.Weapon {
    public class Throw : MonoBehaviour {
        [Tooltip("手雷跟随摄像机移动的速度")]
        public float MoveSpeed;

        [Tooltip("手雷准备丢出去时的位置")]
        public Transform ThrowTarget;

        //使用Inspector进行赋值
        [SerializeField]private Camera m_MainCamera;
        [SerializeField]private InputManager m_InputManager;
        [SerializeField]private Animator m_Animator;

        private GrenadeIKController m_GrenadeIKController;

        private Vector3 m_RelativeGrenadeThrowPosition;
        private Quaternion m_RelativeGrenadeThrowRotation;
        private bool m_LockGrenade;
        private bool m_ReadyToThrow;
        private bool m_Throwing;

        private void Awake() {
            m_GrenadeIKController = GetComponent<GrenadeIKController>();
        }

        private void OnEnable() {
            m_LockGrenade = true;
            m_ReadyToThrow = false;
            m_Throwing = false;
        }

        private void Start() {
            //获取抛出点的相对坐标
            Quaternion rotationInWorld = ThrowTarget.rotation;
            Quaternion worldToCameraRot = Quaternion.Inverse(m_MainCamera.transform.rotation);
            m_RelativeGrenadeThrowRotation = worldToCameraRot * rotationInWorld;

            m_RelativeGrenadeThrowPosition = m_MainCamera.transform.InverseTransformPoint(ThrowTarget.position);
        }

        void Update() {
            if (m_InputManager.AttackButtonDown()) {
                m_ReadyToThrow = true;
            }

            if (m_InputManager.AttackButtonUp()) {
                m_ReadyToThrow = false;
                ThrowOut();
            }

            if (m_ReadyToThrow) {
                ReadyToThrow();
            } else {
                if (!m_Throwing) {
                    if (m_LockGrenade) {
                        UpdateGunTransform();
                    }
                }
            }
        }

        private void OnDisable() {
            m_LockGrenade = false;
        }

        private void UpdateGunTransform() {
            Vector3 position = m_GrenadeIKController.GetGrenadePosition();
            //relativeRotation是camera坐标系下的grenade姿态
            Quaternion rotation = m_GrenadeIKController.GetGrenadeRotation();

            this.transform.rotation = rotation;
            this.transform.position = Vector3.MoveTowards(this.transform.position, position, MoveSpeed * Time.deltaTime);
        }


        private void ReadyToThrow() {
            this.transform.position = m_MainCamera.transform.TransformPoint(m_RelativeGrenadeThrowPosition);
            this.transform.rotation = m_MainCamera.transform.rotation * m_RelativeGrenadeThrowRotation;
        }


        //这里要实例化一个炸弹抛出去，并播放投掷动画
        private void ThrowOut() {
            m_Throwing = true;
            m_Animator.SetTrigger("throw");

            AnimatorStateInfo state = m_Animator.GetCurrentAnimatorStateInfo(0);
            if(state.normalizedTime >= 1f) {
                m_Throwing = false;
            }
        }
    }
}
