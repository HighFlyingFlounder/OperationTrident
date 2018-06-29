using System;
using UnityEngine;
using OperationTrident.CrossInput;

namespace OperationTrident.Player {
    [Serializable]
    public class MouseLook {
        [Tooltip("左右旋转灵敏度")]
        public float XSensitivity = 2f;
        [Tooltip("上下旋转灵敏度")]
        public float YSensitivity = 2f;
        [Tooltip("摄像机旋转最小角度")]
        public float MinAngle = -90F;
        [Tooltip("摄像机旋转最大角度")]
        public float MaxAngle = 90F;
        [Tooltip("是否锁定鼠标")]
        public bool lockCursor = true;

        private InputManager m_InputManager;

        private Quaternion m_CharacterTargetRot;
        private Vector3 m_RotateAngle = Vector3.zero;
        private bool m_cursorIsLocked = true;
        private float m_MaxAngle;
        private float m_MinAngle;

        public void Init(Transform character, InputManager input) {
            m_CharacterTargetRot = character.localRotation;
            m_InputManager = input;

            m_MaxAngle = 360f - MaxAngle;
            m_MinAngle = 0f - MinAngle;
        }

        public void LookRotation(Transform character, Transform camera) {
            float yRot = m_InputManager.MouseX();
            float xRot = m_InputManager.MouseY();

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot * XSensitivity, 0f);
            m_RotateAngle.x = -xRot * YSensitivity;

            character.localRotation = m_CharacterTargetRot;
            camera.Rotate(m_RotateAngle);

            //限制摄像机旋转的角度
            ClampRotationAngle(camera);

            //锁定光标
            UpdateCursorLock();
        }

        public void SetCursorLock(bool value) {
            lockCursor = value;
            // 光标不锁定，显示光标
            if (!lockCursor) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock() {
            // 设置需要锁定光标时，不断检测按键状态
            if (lockCursor)
                InternalLockUpdate();
        }

        //检测按键状态
        private void InternalLockUpdate() {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                m_cursorIsLocked = false;
            } else if (Input.GetMouseButtonUp(0)) {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else if (!m_cursorIsLocked) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        //eulerAngles返回的角度是0~360
        private void ClampRotationAngle(Transform camera) {
            Vector3 angle = camera.eulerAngles;
            float angleX = angle.x;
            if (angleX > m_MinAngle && angleX < 180f) {
                angle.x = m_MinAngle;
            } else {
                if (angleX < m_MaxAngle && angleX > 180f) {
                    angle.x = m_MaxAngle;
                }
            }

            camera.eulerAngles = angle;
        }
    }
}
