using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.FPS.Player {
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class MovementController : MonoBehaviour, NetSyncInterface {
        [Tooltip("当前的Object是否为本地Object，如果不是，则只接受网络同步信息")]
        public bool IsLocalObject;
        [Tooltip("走路的速度")]
        [SerializeField] private float m_WalkSpeed;
        [Tooltip("跑步的速度")]
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        //跳跃的速度
        [SerializeField] private float m_JumpSpeed;
        //走路的间隔
        [SerializeField] private float m_StepInterval;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;

        // 存放走路时的音效
        [SerializeField] private AudioClip[] m_FootstepSounds;
        // 存放跳跃时的音效
        [SerializeField] private AudioClip m_JumpSound;
        // 存放跳跃时落地的音效
        [SerializeField] private AudioClip m_LandSound;

        //获取CharacterController的引用
        private CharacterController m_CharacterController;
        //获取音频播放器的引用
        private AudioSource m_AudioSource;

        //用于判断当前是在跑步还是走路
        private bool m_IsRunning;
        //用于判断当前是否蹲下
        private bool m_IsCrouching;
        //判断当前是否在按了跳跃键
        private bool m_Jump;
        //判断当前是否处于跳跃状态
        private bool m_Jumping;
        //获取用户输入
        private Vector2 m_Input;
        //保存用户上一帧的输入
        private Vector2 m_PreInput;
        //保存用户的速度
        private float m_Speed;
        //角色移动的方向
        private Vector3 m_MoveDir = Vector3.zero;

        private bool m_DisableMovemonet;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;

        private NetSyncController m_NetSyncController;

        private void Awake() {
            m_CharacterController = GetComponent<CharacterController>();
            m_AudioSource = GetComponents<AudioSource>()[0];
        }

        // 初始化数据成员
        private void Start() {
            m_DisableMovemonet = false;

            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_IsCrouching = false;

            m_Input = Vector2.zero;
            m_PreInput = Vector2.zero;

            //设置速度
            BroadcastMessage("SetWalkSpeed", m_WalkSpeed, SendMessageOptions.DontRequireReceiver);
            BroadcastMessage("SetRunSpeed", m_RunSpeed, SendMessageOptions.DontRequireReceiver);
        }

        private void FixedUpdate() {
            //不是本地玩家，不执行任何操作
            if (!IsLocalObject) {
                return;
            }

            //只有站在地面上时才能移动
            if (m_CharacterController.isGrounded) {
                //获取用户的输入
                GetInput();

                //用户即将移动的方向
                Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

                // get a normal for the surface that is being touched to move along it
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                   m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
                //确定x和z方向上移动的距离
                m_MoveDir.x = desiredMove.x * m_Speed;
                m_MoveDir.z = desiredMove.z * m_Speed;

                //保持Player贴在地面上
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump) {
                    //蹲下时不能直接起跳，需要先站起来
                    if (m_IsCrouching) {
                        //调用RPC函数
                        StandUp();
                    } else {
                        m_MoveDir.y = m_JumpSpeed;
                        //调用RPC函数
                        Jump();
                        PlayJumpSound();
                        m_Jumping = true;
                    }
                    m_Jump = false;

                }
            } else {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }

            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
            ProgressStepCycle(m_Speed);
        }

        // Update is called once per frame
        private void Update() {
            //不是本地玩家，不执行任何操作
            if (!IsLocalObject) {
                return;
            }

            //跳跃后刚刚落地
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded) {
                //播放落地音效
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }

            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded) {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        //播放落地的音效
        private void PlayLandingSound() {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + 0.5f;
        }

        //处理跳跃的函数
        private void PlayJumpSound() {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }

        //循环播放走路音效
        private void ProgressStepCycle(float speed) {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0)) {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsRunning ? m_RunstepLenghten : 1f))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep)) {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio() {
            if (!m_CharacterController.isGrounded) {
                return;
            }
            //随机选择一个走路音效进行播放
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            //避免重复播放同一个音效
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        private void GetInput() {
            // 在跳跃的过程中不能重复跳跃
            if (!m_Jump) {
                m_Jump = Input.GetButtonDown("Jump");
            }

            //水平移动
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            m_Input = new Vector2(horizontal, vertical);
            

            //归一化用户的输入
            if (m_Input.sqrMagnitude > 1) {
                m_Input.Normalize();
            }

            //这里需要调用RPC
            Walk(horizontal, vertical);

            bool currentRunningState = Input.GetKey(KeyCode.LeftShift);
#if !MOBILE_INPUT
            if(currentRunningState != m_IsRunning) {
                // 按住shift键进入跑步状态
                m_IsRunning = currentRunningState;

                //这里需要调用RPC函数
                Run();
            }
#endif
            // 根据当前的状态设置速度
            m_Speed = m_IsRunning ? m_RunSpeed : m_WalkSpeed;

            //站立和蹲下
            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                if (m_IsCrouching) {
                    //这里需要调用RPC
                    StandUp();
                } else {
                    //这里需要调用RPC
                    Underarm();
                }
            }
            //如果是蹲着，那么速度减半
            if (m_IsCrouching) {
                m_Speed /= 2;
            }

            //更新速度信息
            BroadcastMessage("ChangeMovementSpeed", m_Speed, SendMessageOptions.DontRequireReceiver);

            //发送开始行走的消息
            if (m_Input.magnitude > 0f && m_PreInput.magnitude == 0f) {
                BroadcastMessage("StartWalking", SendMessageOptions.DontRequireReceiver);
            }

            //停止行走
            if (m_PreInput.magnitude > 0f && m_Input.magnitude == 0f) {
                BroadcastMessage("StopWalking", SendMessageOptions.DontRequireReceiver);
            }

            //重置变量
            m_PreInput = m_Input;
        }

        //碰到物体时让玩家往后退
        private void OnControllerColliderHit(ControllerColliderHit hit) {
            Rigidbody body = hit.collider.attachedRigidbody;
            if (m_CollisionFlags == CollisionFlags.Below) {
                return;
            }

            if (body == null || body.isKinematic) {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }

        #region RPC函数
        //行走函数
        public void Walk(float h, float v) {
            //调用RPC
            if (IsLocalObject) {
                if (Mathf.Abs(m_Input.x - m_PreInput.x) > 0.05f || Mathf.Abs(m_Input.y - m_PreInput.y) > 0.05f) {
                    m_NetSyncController.RPC(this, "Walk", h, v);
                }
            }


            SendMessage("SetWalkAnimationParamters", new Vector2(h, v), SendMessageOptions.DontRequireReceiver);
        }

        public void Run() {
            //调用RPC
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "Run");
            }

            SendMessage("SetRunAnimationParamters", m_IsRunning, SendMessageOptions.DontRequireReceiver);
        }

        public void Jump() {
            //调用RPC
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "Jump");
            }

            SendMessage("SetJumpAnimationParamters", SendMessageOptions.DontRequireReceiver);
        }


        //站立函数
        public void StandUp() {
            //调用RPC
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "StandUp");
            }

            //碰撞体高度减少一半
            m_CharacterController.height *= 2;
            //m_CharacterController.center *= 2;

            m_IsCrouching = false;

            SendMessage("SetCrouchAnimationParamters", m_IsCrouching, SendMessageOptions.DontRequireReceiver);
            BroadcastMessage("MoveWeaponUp", SendMessageOptions.DontRequireReceiver);
        }

        //蹲下函数
        public void Underarm() {
            //调用RPC
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "Underarm");
            }

            float distacne = m_CharacterController.height / 2;
            //碰撞体高度减少一半
            m_CharacterController.height -= distacne;
            //m_CharacterController.center /= 2;

            m_IsCrouching = true;

            SendMessage("SetCrouchAnimationParamters", m_IsCrouching, SendMessageOptions.DontRequireReceiver);
            BroadcastMessage("MoveWeaponDown", distacne, SendMessageOptions.DontRequireReceiver);
        }
        #endregion

        public void RecvData(SyncData data) {
            throw new System.NotImplementedException();
        }

        public SyncData SendData() {
            return new SyncData();
        }

        public void Init(NetSyncController controller) {
            m_NetSyncController = controller;
        }
    }
}
