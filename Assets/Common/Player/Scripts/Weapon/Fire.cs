using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.CrossInput;

namespace OperationTrident.Weapon {
    [RequireComponent(typeof(AudioSource))]
    public class Fire : MonoBehaviour {
        [Tooltip("射击音效")]
        public AudioClip ShootSound;
        [Tooltip("射击间隔")]
        public float ShootInterval;
        [Tooltip("换弹时长")]
        public float BulletSupplementInterval;
        [Tooltip("弹夹容量")]
        public int ClipCapacity;
        [Tooltip("总子弹数")]
        public int BulletNum;
        [Tooltip("瞄准镜摄像机")]
        public Camera MirrorCamera;
        [Tooltip("射击后下落的时间")]
        public float DownTime;
        [Tooltip("枪向后运动的最大偏移量")]
        public float MaxBackwordOffset;
        [Tooltip("镜头往上跳的最大角度")]
        public float MaxUpAngle;
        [Tooltip("枪跟随摄像机移动的速度")]
        public float MoveSpeed;
        [Tooltip("开镜时枪的位置")]
        public Transform MirrorTarget;

        //[SerializeField]
        private Transform m_InitTransform;

        //当前弹夹剩余子弹数
        private int m_ClipBulletNum;
        //用于开枪的计时器
        private float m_ShootTimer;
        //当前是否处于射击状态
        private bool m_IsShooting;
        //当前是否开镜射击
        private bool m_IsUsingMirror;

        //保存枪口位置偏移量
        private float m_GunPosShakeOffet;
        //将枪运动的最大偏移量进行缩放，避免设置的值太小
        private float m_ScaledMaxBackwordOffset;
        //镜头当前欧拉角
        private Vector3 m_CurrentEulerAngleOfCamera;
        //开镜位置在摄像机坐标系下的相对位置
        private Vector3 m_RelativeMirrorTargetPosition;
        private Quaternion m_RelativeMirrorTargetRotation;
        //瞄准镜相对枪的位置
        private Vector3 m_RelativeMirrorPositionToGun;

        //场景主摄像机
        [SerializeField]private Camera m_MainCamera;
        [SerializeField]private InputManager m_InputManager;
        //音源播放器
        private AudioSource m_AudioSource;
        //gun的IK控制器
        private GunIKController m_GunIKController;

        private Vector3 m_InitPosition;
        private Quaternion m_InitRotation;

        private void Awake() {
            //获取引用
            m_AudioSource = GetComponent<AudioSource>();
            m_GunIKController = GetComponent<GunIKController>();

            m_InitTransform = this.transform;
        }

        private void OnEnable() {
            //不使用瞄准镜
            MirrorCamera.enabled = false;
            //重置射击状态
            m_IsShooting = false;
            //重置瞄准镜使用状态
            m_IsUsingMirror = false;
            //重置计时器
            m_ShootTimer = 0f;

            m_InitPosition = this.transform.localPosition;
            m_InitRotation = this.transform.localRotation;
        }

        // 初始化数据成员
        void Start() {
            m_ClipBulletNum = ClipCapacity;

            m_ScaledMaxBackwordOffset = MaxBackwordOffset / 100;

            Quaternion mirrorTargetInWorld = MirrorTarget.rotation;
            Quaternion worldToCamera = Quaternion.Inverse(m_MainCamera.transform.rotation);
            m_RelativeMirrorTargetRotation = worldToCamera * mirrorTargetInWorld;

            m_RelativeMirrorTargetPosition = m_MainCamera.transform.InverseTransformPoint(MirrorTarget.position);

            m_RelativeMirrorPositionToGun = MirrorCamera.transform.localPosition;
        }

        // Update is called once per frame
        void Update() {
            if (m_IsShooting) {
                m_ShootTimer += Time.deltaTime;
                if(m_ShootTimer >= ShootInterval) {
                    m_IsShooting = false;
                }
            } else {
                if (m_InputManager.PressAttackButton()) {
                    Shoot();

                    m_IsShooting = true;
                    m_ShootTimer = 0f;
                }
            }

            if (m_InputManager.MirrorButtonDown()) {
                if (m_IsUsingMirror) {
                    MirrorCamera.enabled = false;
                    m_IsUsingMirror = false;

                    this.transform.localPosition = m_InitPosition;
                    this.transform.localRotation = m_InitRotation;
                } else {
                    MirrorCamera.enabled = true;
                    m_IsUsingMirror = true;

                    this.transform.localPosition = m_RelativeMirrorTargetPosition;
                    this.transform.localRotation = m_RelativeMirrorTargetRotation;
                }
            }
        }

        private void OnDisable() {
            MirrorCamera.enabled = false;
            m_IsUsingMirror = false;
        }

        private void Shoot() {
            //m_AudioSource.Play();
            
            if (m_IsUsingMirror) {
                StartCoroutine(ShakeMirror());
            } else {
                StartCoroutine(ShakeGun());
            }

            StartCoroutine(ShakeCamera());
        }

        //Z值大于零，向后运动
        private IEnumerator ShakeGun() {
            float timer = 0f;
            float interval = ShootInterval / 2;
            Vector3 direction = transform.forward + transform.up / 2;

            while (timer <= interval) {
                m_GunPosShakeOffet = Mathf.Lerp(0, m_ScaledMaxBackwordOffset, timer / interval);
                this.transform.position += direction * m_GunPosShakeOffet;

                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            timer = 0f;
            while (timer <= interval) {
                m_GunPosShakeOffet = Mathf.Lerp(m_ScaledMaxBackwordOffset, 0, timer / interval);
                this.transform.position -= direction * m_GunPosShakeOffet;

                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            //恢复枪的位置
            this.transform.localPosition = m_InitPosition;
        }

        //旋转角为负数往上动
        private IEnumerator ShakeCamera() {
            float timer = 0f;

            while (timer <= ShootInterval) {
                m_CurrentEulerAngleOfCamera.x = Mathf.Lerp(0, MaxUpAngle, timer / ShootInterval);
                m_MainCamera.transform.Rotate(m_CurrentEulerAngleOfCamera);

                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            timer = 0f;
            while (timer <= DownTime) {
                m_CurrentEulerAngleOfCamera.x = -Mathf.Lerp(MaxUpAngle, 0, timer / DownTime);
                m_MainCamera.transform.Rotate(m_CurrentEulerAngleOfCamera);

                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator ShakeMirror() {
            float timer = 0f;
            Vector3 direction = transform.forward + transform.up / 2;

            //开镜缩小抖动效果
            float offset = m_ScaledMaxBackwordOffset / 10;

            while (timer <= ShootInterval) {
                m_GunPosShakeOffet = Mathf.Lerp(0, offset, timer / ShootInterval);

                MirrorCamera.transform.position -= direction * m_GunPosShakeOffet;

                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            timer = 0f;
            while (timer <= DownTime) {
                m_GunPosShakeOffet = Mathf.Lerp(offset, 0, timer / DownTime);

                MirrorCamera.transform.position += direction * m_GunPosShakeOffet;

                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            MirrorCamera.transform.localPosition = m_RelativeMirrorPositionToGun;
        }
    }
}
