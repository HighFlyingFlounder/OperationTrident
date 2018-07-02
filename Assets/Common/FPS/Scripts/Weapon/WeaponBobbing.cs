using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.FPS.Common;

namespace OperationTrident.FPS.Weapons {
    public class WeaponBobbing : MonoBehaviour {
        [Tooltip("将枪移动到跑步位置所需要的时间")]
        public float SwitchTime = 0.5f;
        [Tooltip("停止射击之后恢复到行走状态所需要的时间")]
        public float DelayTimeAfterShoot;
        [Tooltip("枪在跑步时所处的位置")]
        public Transform RunningPosition;
        [Tooltip("枪抖动参数")]
        [SerializeField] private Bobbing m_Bobbing = new Bobbing();

        private Coroutine m_Coroutine;
        private Vector3 m_OriginalPosition;
        private Quaternion m_OriginalRotation;

        private float m_Speed;
        private bool m_IsRunning;
        private bool m_IsWalking;
        private bool m_IsFiring;
        private bool m_IsUsingMirror;
        private float m_FireTimer;

        // Use this for initialization
        void Start() {
            m_Bobbing.Init(this.transform);
            m_OriginalPosition = transform.localPosition;
            m_OriginalRotation = transform.localRotation;

            m_IsRunning = false;
            m_IsWalking = false;
            m_IsFiring = false;
            m_IsUsingMirror = false;
            m_FireTimer = 0f;
        }

        private void Update() {
            if (m_IsFiring) {
                m_FireTimer += Time.deltaTime;

                //DelayTimeAfterShoot秒之内没有开枪则认为开枪结束
                if (m_FireTimer >= DelayTimeAfterShoot) {
                    m_FireTimer = 0f;
                    m_IsFiring = false;

                    //停止射击，还在跑步则进入跑步状态
                    if (!m_IsUsingMirror && m_IsRunning) {
                        //如果当前在跑步，重置枪的位置
                        StartCoroutine(SwitchToRun());
                    }
                }
            }
        }

        //改变移动速度
        private void ChangeMovementSpeed(float speed) {
            m_Speed = speed;
            if (m_IsRunning) {
                m_IsRunning = false;

                //停止跑步，重置枪的位置为初始位置
                this.transform.localPosition = m_OriginalPosition;
                this.transform.localRotation = m_OriginalRotation;
                m_Bobbing.Init(this.transform);
            }

            m_Bobbing.ChangeSpeed(speed);
        }

        private void StartWalking() {
            m_IsWalking = true;

            //按下加速键并且此时没有在开枪时，可以切换为跑步姿势
            if (!m_IsUsingMirror && !m_IsFiring && m_IsRunning) {
                //开始跑步，设置枪的位置为跑步时的位置
                StartCoroutine(SwitchToRun());
            }

            m_Coroutine = StartCoroutine(m_Bobbing.DoBobbing());
        }

        private void StartRunning() {
            m_IsRunning = true;

            //当正在行走并且没有在开枪时，可以转换为跑步姿势
            if (!m_IsUsingMirror && !m_IsFiring && m_IsWalking) {
                //开始跑步，设置枪的位置为跑步时的位置
                StartCoroutine(SwitchToRun());
            }
        }

        //停止走路
        private void StopWalking() {
            //走路停止
            m_IsWalking = false;
            //跑步也停止
            m_IsRunning = false;

            if(m_Coroutine != null) {
                StopCoroutine(m_Coroutine);
            }

            //停止行走，恢复枪的位置
            transform.localPosition = m_OriginalPosition;
            transform.localRotation = m_OriginalRotation;
        }

        private void StartShooting() {
            //如果是连续开枪或者静止，那么后面的跳过
            if (m_IsFiring || !m_IsWalking) {
                //重置计时器
                m_FireTimer = 0f;
                return;
            }

            //恢复枪的位置为初始位置
            this.transform.localPosition = m_OriginalPosition;
            this.transform.localRotation = m_OriginalRotation;
            m_Bobbing.Init(this.transform);

            m_IsFiring = true;
        }

        private void UpdateMirrorState(bool mirrorState) {
            if(m_IsUsingMirror == true && mirrorState == false) {
                if (m_IsRunning) {
                    StartCoroutine(SwitchToRun());
                }
            }
            m_IsUsingMirror = mirrorState;
        }

        private IEnumerator SwitchToRun() {
            float timer = 0f;
            while(timer <= SwitchTime && !m_IsFiring) {
                this.transform.localPosition = Vector3.Lerp(m_OriginalPosition, RunningPosition.localPosition, timer / SwitchTime);
                this.transform.localRotation = Quaternion.Lerp(m_OriginalRotation, RunningPosition.localRotation, timer / SwitchTime);
                m_Bobbing.Init(this.transform);

                timer += Time.deltaTime;

                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
    }
}
