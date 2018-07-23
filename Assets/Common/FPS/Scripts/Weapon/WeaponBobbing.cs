using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.FPS.Common;

namespace OperationTrident.FPS.Weapons {
    [RequireComponent(typeof(Weapon))]
    public class WeaponBobbing : MonoBehaviour {
        [Tooltip("将枪移动到跑步位置所需要的时间")]
        public float SwitchTime = 0.5f;
        [Tooltip("停止射击之后恢复到行走状态所需要的时间")]
        public float DelayTimeAfterShoot;
        [Tooltip("枪在跑步时所处的位置")]
        public Transform RunningPosition;
        [Tooltip("枪在换弹时所处的位置")]
        public Transform ReloadingPosition;
        [Tooltip("枪抖动参数")]
        [SerializeField] private Bobbing m_Bobbing = new Bobbing();

        private Coroutine m_Coroutine;

        private Vector3 m_OriginalPosition;
        private Quaternion m_OriginalRotation;

        private Vector3 m_CurrentPosition;
        private Quaternion m_CurrentRotation;


        private float m_Speed;
        private bool m_IsRunning;
        private bool m_IsWalking;
        private bool m_IsFiring;
        private bool m_IsReloading;
        private bool m_IsUsingMirror;
        private float m_FireTimer;

        private float m_WalkSpeed;
        private float m_RunSpeed;

        // Use this for initialization
        void Start() {
            m_Bobbing.Init(this.transform);
            m_OriginalPosition = transform.localPosition;
            m_OriginalRotation = transform.localRotation;

            m_IsRunning = false;
            m_IsWalking = false;
            m_IsFiring = false;
            m_IsReloading = false;
            m_IsUsingMirror = false;
            m_FireTimer = 0f;
        }

        private void Update() {
            m_CurrentPosition = this.transform.localPosition;
            m_CurrentRotation = this.transform.localRotation;

            if (m_IsFiring) {
                m_FireTimer += Time.deltaTime;

                //DelayTimeAfterShoot秒之内没有开枪，且当前不在换弹，则认为开枪结束
                if (m_FireTimer >= DelayTimeAfterShoot && !m_IsReloading) {
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

        private void SetWalkSpeed(float speed) {
            m_WalkSpeed = speed;
        }

        private void SetRunSpeed(float speed) {
            m_RunSpeed = speed;
        }

        //改变移动速度
        private void ChangeMovementSpeed(float speed) {
            m_Speed = speed;

            if (speed < m_RunSpeed && m_IsRunning) {
                m_IsRunning = false;

                //停止跑步，重置枪的位置为初始位置
                this.transform.localPosition = m_OriginalPosition;
                this.transform.localRotation = m_OriginalRotation;
                m_Bobbing.Init(this.transform);
            }

            if(speed == m_RunSpeed && !m_IsRunning) {
                m_IsRunning = true;
                //当正在行走并且没有在开枪时，可以转换为跑步姿势
                if (!m_IsUsingMirror && !m_IsFiring && m_IsWalking && !m_IsReloading) {
                    //开始跑步，设置枪的位置为跑步时的位置
                    StartCoroutine(SwitchToRun());
                }
            }

            m_Bobbing.ChangeSpeed(speed);
        }

        private void StartWalking() {
            m_IsWalking = true;

            //按下加速键并且此时没有在开枪时，可以切换为跑步姿势
            if (!m_IsUsingMirror && !m_IsFiring && m_IsRunning && !m_IsReloading) {
                //开始跑步，设置枪的位置为跑步时的位置
                StartCoroutine(SwitchToRun());
            }

            m_Coroutine = StartCoroutine(m_Bobbing.DoBobbing());
        }

        //停止走路
        private void StopWalking() {
            //走路停止
            m_IsWalking = false;
            m_IsRunning = false;

            if(m_Coroutine != null) {
                StopCoroutine(m_Coroutine);
            }

            //如果正在换弹，不修改枪的位置
            if (m_IsReloading) {
                return;
            }

            //停止行走，恢复枪的位置
            transform.localPosition = m_OriginalPosition;
            transform.localRotation = m_OriginalRotation;
            m_Bobbing.Init(this.transform);
        }

        private void StartShooting() {
            //如果是连续开枪或者静止，那么后面的跳过
            if (m_IsFiring) {
                //重置计时器
                m_FireTimer = 0f;
                return;
            }

            m_IsFiring = true;

            //恢复枪的位置为初始位置
            this.transform.localPosition = m_OriginalPosition;
            this.transform.localRotation = m_OriginalRotation;
            m_Bobbing.Init(this.transform);
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
            //只有在不开枪、不开镜且正在跑步的时候执行
            while(timer <= SwitchTime && !m_IsFiring && m_IsRunning) {
                //不断更新枪的位置
                this.transform.localPosition = Vector3.Lerp(m_OriginalPosition, RunningPosition.localPosition, timer / SwitchTime);
                this.transform.localRotation = Quaternion.Lerp(m_OriginalRotation, RunningPosition.localRotation, timer / SwitchTime);
                m_Bobbing.Init(this.transform);

                timer += Time.deltaTime;

                yield return null;
            }

            //确保能到达终点
            this.transform.localPosition = RunningPosition.localPosition;
            this.transform.localRotation = RunningPosition.localRotation;
            m_Bobbing.Init(this.transform);
        }

        private void UpdateReloadState(bool reloading) {
            m_IsReloading = reloading;
            Debug.Log(m_IsReloading);

            if (m_IsReloading) {
                StartCoroutine(SwitchToReload());
            } else {
                if (m_IsRunning) {
                    //当正在行走并且没有在开枪时，可以转换为跑步姿势
                    if (!m_IsUsingMirror && !m_IsFiring && m_IsWalking) {
                        //开始跑步，设置枪的位置为跑步时的位置
                        StartCoroutine(SwitchToRun());
                    }
                } else {
                    //停止跑步，重置枪的位置为初始位置
                    this.transform.localPosition = m_OriginalPosition;
                    this.transform.localRotation = m_OriginalRotation;
                    m_Bobbing.Init(this.transform);
                }
            }
        }

        private IEnumerator SwitchToReload() {
            float timer = 0f;
            //只有在换弹的时候执行
            while (timer <= SwitchTime && m_IsReloading) {
                //不断更新枪的位置
                this.transform.localPosition = Vector3.Lerp(m_CurrentPosition, ReloadingPosition.localPosition, timer / SwitchTime);
                this.transform.localRotation = Quaternion.Lerp(m_CurrentRotation, ReloadingPosition.localRotation, timer / SwitchTime);
                m_Bobbing.Init(this.transform);

                timer += Time.deltaTime;

                yield return null;
            }
        }
    }
}
