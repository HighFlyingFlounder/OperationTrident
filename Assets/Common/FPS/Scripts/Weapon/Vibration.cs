using UnityEngine;
using System.Collections;

namespace OperationTrident.Weapons {
    public class Vibration : MonoBehaviour {
        //是否在初始化的时候震动
        public bool VibrateOnAwake = true;
        //震动幅度
        public Vector3 StartingShakeDistance;   
        //摄像机旋转幅度
        public Quaternion StartingRotationAmount;
        //震动速度
        public float ShakeSpeed = 60.0f;
        //震动衰减的速率
        public float DecreaseMultiplier = 0.5f;
        //震动的次数
        public int NumberOfShakes = 8;
        //是否持续震动
        public bool ShakeContinuous = false;  

        //实际震动幅度
        private Vector3 m_ActualStartingShakeDistance;
        //实际旋转幅度
        private Quaternion m_ActualStartingRotationAmount;
        //实际震动速度 
        private float m_ActualShakeSpeed;
        //实际震动衰减的速率
        private float m_ActualDecreaseMultiplier;
        //实际震动的次数
        private int m_ActualNumberOfShakes;

        //震动开始前的初始位置
        private Vector3 m_OriginalLocalPosition; 
        //震动开始前的初始朝向
        private Quaternion m_OriginalLocalRotation;

        void Awake() {
            //初始化位置和朝向
            m_OriginalLocalPosition = transform.localPosition;
            m_OriginalLocalRotation = transform.localRotation;

            if (VibrateOnAwake) {
                StartShaking();
            }
        }

        //开始震动
        public void StartShaking() {
            m_ActualStartingShakeDistance = StartingShakeDistance;
            m_ActualStartingRotationAmount = StartingRotationAmount;
            m_ActualShakeSpeed = ShakeSpeed;
            m_ActualDecreaseMultiplier = DecreaseMultiplier;
            m_ActualNumberOfShakes = NumberOfShakes;
            StopShaking();
            StartCoroutine("Shake");
        }

        //函数重载
        public void StartShaking(Vector3 shakeDistance, Quaternion rotationAmount, float speed, float diminish, int numOfShakes) {
            m_ActualStartingShakeDistance = shakeDistance;
            m_ActualStartingRotationAmount = rotationAmount;
            m_ActualShakeSpeed = speed;
            m_ActualDecreaseMultiplier = diminish;
            m_ActualNumberOfShakes = numOfShakes;
            StopShaking();
            StartCoroutine("Shake");
        }

        //随机震动
        public void StartShakingRandom(float minDistance, float maxDistance, float minRotationAmount, float maxRotationAmount) {
            m_ActualStartingShakeDistance = new Vector3(Random.Range(minDistance, maxDistance), Random.Range(minDistance, maxDistance), Random.Range(minDistance, maxDistance));
            m_ActualStartingRotationAmount = new Quaternion(Random.Range(minRotationAmount, maxRotationAmount), Random.Range(minRotationAmount, maxRotationAmount), Random.Range(minRotationAmount, maxRotationAmount), 1);
            m_ActualShakeSpeed = ShakeSpeed * Random.Range(0.8f, 1.2f);
            m_ActualDecreaseMultiplier = DecreaseMultiplier * Random.Range(0.8f, 1.2f);
            m_ActualNumberOfShakes = NumberOfShakes + Random.Range(-2, 2);
            StopShaking();
            StartCoroutine("Shake");
        }

        public void StopShaking() {
            //停止协程
            StopCoroutine("Shake");

            //恢复位置和朝向
            transform.localPosition = m_OriginalLocalPosition;
            transform.localRotation = m_OriginalLocalRotation;
        }

        private IEnumerator Shake() {
            m_OriginalLocalPosition = transform.localPosition;
            m_OriginalLocalRotation = transform.localRotation;

            float hitTime = Time.time;
            float shake = m_ActualNumberOfShakes;

            float shakeDistanceX = m_ActualStartingShakeDistance.x;
            float shakeDistanceY = m_ActualStartingShakeDistance.y;
            float shakeDistanceZ = m_ActualStartingShakeDistance.z;

            float shakeRotationX = m_ActualStartingRotationAmount.x;
            float shakeRotationY = m_ActualStartingRotationAmount.y;
            float shakeRotationZ = m_ActualStartingRotationAmount.z;

            //开始震动
            while (shake > 0 || ShakeContinuous) {
                float timer = (Time.time - hitTime) * m_ActualShakeSpeed;
                float x = m_OriginalLocalPosition.x + Mathf.Sin(timer) * shakeDistanceX;
                float y = m_OriginalLocalPosition.y + Mathf.Sin(timer) * shakeDistanceY;
                float z = m_OriginalLocalPosition.z + Mathf.Sin(timer) * shakeDistanceZ;

                float xr = m_OriginalLocalRotation.x + Mathf.Sin(timer) * shakeRotationX;
                float yr = m_OriginalLocalRotation.y + Mathf.Sin(timer) * shakeRotationY;
                float zr = m_OriginalLocalRotation.z + Mathf.Sin(timer) * shakeRotationZ;

                transform.localPosition = new Vector3(x, y, z);
                transform.localRotation = new Quaternion(xr, yr, zr, 1);

                if (timer > Mathf.PI * 2) {
                    hitTime = Time.time;
                    shakeDistanceX *= m_ActualDecreaseMultiplier;
                    shakeDistanceY *= m_ActualDecreaseMultiplier;
                    shakeDistanceZ *= m_ActualDecreaseMultiplier;

                    shakeRotationX *= m_ActualDecreaseMultiplier;
                    shakeRotationY *= m_ActualDecreaseMultiplier;
                    shakeRotationZ *= m_ActualDecreaseMultiplier;

                    shake--;
                }
                yield return true;
            }

            //震动结束，恢复位置和朝向
            transform.localPosition = m_OriginalLocalPosition;
            transform.localRotation = m_OriginalLocalRotation;
        }
    }
}