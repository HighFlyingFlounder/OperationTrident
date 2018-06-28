using System;
using UnityEngine;
using Random = UnityEngine.Random;
using OperationTrident.CrossInput;

namespace OperationTrident.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerController : MonoBehaviour
    {
        //用于判断当前是在跑步还是走路
        [SerializeField] private bool m_IsWalking;
        //走路的速度
        [SerializeField] private float m_WalkSpeed;
        //跑步的速度
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        //跳跃的速度
        [SerializeField] private float m_JumpSpeed;
        //走路的间隔
        [SerializeField] private float m_StepInterval;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;

        [SerializeField] private bool m_UseHeadBob;

        //使用序列化，将这三个类定义的public参数保留在Inspector面板中
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private MouseLook m_MouseLook;

        // 存放走路时的音效
        [SerializeField] private AudioClip[] m_FootstepSounds;
        // 存放跳跃时的音效
        [SerializeField] private AudioClip m_JumpSound;
        // 存放跳跃时落地的音效
        [SerializeField] private AudioClip m_LandSound;

        //获取摄像机引用
        [SerializeField] private Camera m_Camera;
        //获取CharacterController的引用
        private CharacterController m_CharacterController;
        //获取音频播放器的引用
        private AudioSource m_AudioSource;
        //获取动画播放器的引用
        private Animator m_Animator;

        private InputManager m_InputManager;

        //判断当前是否在按了跳跃键
        private bool m_Jump;
        //判断当前是否处于跳跃状态
        private bool m_Jumping;
        //private float m_YRotation;
        //获取用户输入
        private Vector2 m_Input;
        //角色移动的方向
        private Vector3 m_MoveDir = Vector3.zero;

        private bool m_DisableMovemonet;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;

        private void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_AudioSource = GetComponent<AudioSource>();
            m_Animator = GetComponent<Animator>();
            m_InputManager = GetComponent<InputManager>();
        }

        // 初始化数据成员
        private void Start()
        {
            m_DisableMovemonet = false;

            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;

            //m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_MouseLook.Init(transform, m_InputManager);
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);

            m_Animator.SetFloat("speed", m_Input.magnitude);

            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                Debug.Log(Physics.gravity);
            }

            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }

        // Update is called once per frame
        private void Update()
        {
            RotateView();

            // 在跳跃的过程中不能重复跳跃
            if (!m_Jump)
            {
                m_Jump = m_InputManager.JumpButtonDown();

                if (m_Jump) {
                    m_Animator.SetTrigger("jump");
                }
            }

            //跳跃后刚刚落地
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                //开启落地协程
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }

            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        //播放落地的音效
        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + 0.5f;
        }


        //处理跳跃的函数
        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }

        //循环播放走路音效
        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        //摄像机抖动函数
        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }

            //通过修改camera的本地坐标让camera抖动
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }

        private void GetInput(out float speed)
        {
            // 获取用户输入
            float horizontal = m_InputManager.Horizontal();
            float vertical = m_InputManager.Vertical();
            m_Input = new Vector2(horizontal, vertical);

            //bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // 按住shift键进入跑步状态
            m_IsWalking = !m_InputManager.PressSpeedUpButton();
            m_Animator.SetBool("run", !m_IsWalking);
#endif
            // 根据当前的状态设置速度
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;

            // 归一化用户的输入
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }
        }

        //根据用户输入改变角色和摄像机的角度
        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }

        //PlayerController被禁用时调用
        private void OnDisable() {
            ////禁用控制移动的组件
            //m_CharacterController.enabled = false;

            //启用动画控制脚本
            GetComponent<AnimatorController>().enabled = true;
        }
    }
}
