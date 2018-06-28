using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Player {
    public enum RotationAxes {
        AxesX,
        AxesY,
        AxesXandY
    }

    public class MouseRotator : MonoBehaviour {

        public RotationAxes axes = RotationAxes.AxesXandY;

        [Tooltip("左右旋转灵敏度")]
        public float XSensitivity = 2f;
        [Tooltip("上下旋转灵敏度")]
        public float YSensitivity = 2f;
        [Tooltip("摄像机旋转最小角度")]
        public float MinAngle = -90F;
        [Tooltip("摄像机旋转最大角度")]
        public float MaxAngle = 90F;

        private float m_MaxAngle;
        private float m_MinAngle;
        private float m_RotationX;

        private void Start() {
            m_MaxAngle = 360f - MaxAngle;
            m_MinAngle = 0f - MinAngle;

            m_RotationX = 0f;
        }

        private void Update() {
            float yRot = 0f;
            float xRot = 0f;

            if(axes == RotationAxes.AxesX) {
                yRot = Input.GetAxis("Mouse X");
                transform.Rotate(0f, yRot * XSensitivity, 0f);
            } else if(axes == RotationAxes.AxesY) {
                xRot = Input.GetAxis("Mouse Y");
                m_RotationX -= xRot * YSensitivity;

                m_RotationX = Mathf.Clamp(m_RotationX, MinAngle, MaxAngle);
                float rotationY = transform.localEulerAngles.y;
                transform.localEulerAngles = new Vector3(m_RotationX, rotationY, 0f);

                //ClampRotationAngle();
            } else if(axes == RotationAxes.AxesXandY) {
                yRot = Input.GetAxis("Mouse X");
                xRot = Input.GetAxis("Mouse Y");

                m_RotationX -= xRot * YSensitivity;
                m_RotationX = Mathf.Clamp(m_RotationX, MinAngle, MaxAngle);

                float delta = yRot * XSensitivity;
                float rotationY = transform.localEulerAngles.y + delta;
                transform.localEulerAngles = new Vector3(m_RotationX, rotationY, 0f);
                //ClampRotationAngle();
            }
        }

        //后坐力函数
        private void RecoilEffect(Hashtable param) {
            StartCoroutine(Recoil((float)param["shootInterval"], (float)param["maxAngle"]));
        }

        IEnumerator Recoil(float shootInterval, float maxAngle) {
            float timer = 0f;
            float downTime = 0.5f * shootInterval;

            while (timer <= shootInterval) {
                m_RotationX -= Mathf.Lerp(0, maxAngle, timer / shootInterval);
                //transform.Rotate(currentEulerAngle, Space.Self);

                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            timer = 0f;
            while (timer <= downTime) {
                m_RotationX += Mathf.Lerp(maxAngle, 0, timer / downTime);

                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
