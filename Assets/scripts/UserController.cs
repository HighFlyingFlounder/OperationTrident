using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
public class UserController : MonoBehaviour
{
    //操控类型
    public enum CtrlType
    {
        none,
        player,
        computer,
        net,
    }
    public CtrlType ctrlType = CtrlType.player;

    public class MovementSettings
    {
        //都是一个速度感觉直接用一个变量算了= =
        /*public float ForwardSpeed = 4.0f;
        public float HorizontalSpeed = 4.0f;
        public float VerticalSpeed = 4.0f;
        */
        public float Speed = 2.0f;
        public float RunMultiplier = 2.0f;
        public KeyCode RunKey = KeyCode.LeftShift;
        public bool isRun = false;
        public bool isPushed = false;
        
        public float CurrentTargetSpeed = 2.0f;

        public void UpdateDesiredTargetSpeed(Vector2 input)
        {
            if (Input.GetKeyDown(RunKey))
            {
                isRun = true;
            }
            if (Input.GetKeyUp(RunKey))
            {
                isRun = false;
            }
            if (input == Vector2.zero) return;
            if (input.x > 0 || input.x < 0)
            {
                //horizontal
                CurrentTargetSpeed = /*Horizontal*/Speed * (isRun ? RunMultiplier : 1.0f);
            }
            if (input.y > 0 || input.y < 0)
            {
                //vertical
                CurrentTargetSpeed = /*Vertical*/Speed * (isRun ? RunMultiplier : 1.0f);
            }
        }
    }

    public Camera cam;
    public GameObject model;
    public MovementSettings movementSettings;
    private MouseLook mouseLook;
    public GameObject RunParticle;
    private float hp = 100f;
    private float t = 0.0f;//计时器,在喷射系统从零开始加速时使用

    public Rigidbody m_RigidBody;
    private Animator m_Animator;
    private float m_YRotation;

    public Vector3 Velocity
    {
        get { return m_RigidBody.velocity; }
    }

    private void Awake()
    {
        mouseLook = new MouseLook();
        movementSettings = new MovementSettings();
    }

    private void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Animator = model.GetComponent<Animator>();
        mouseLook.Init(transform, cam.transform);
    }


    private void Update()
    {
        
    }
    
    private void FixedUpdate()
    {
        if (movementSettings.isRun)
        {
            RunParticle.active = true;
        }
        else
        {
            RunParticle.active = false;
        }
        Vector3 ViewPortPos = cam.WorldToViewportPoint(model.transform.position);
        m_Animator.SetFloat("Vertical", 20 * (2 * ViewPortPos.y - 1.0f));
        m_Animator.SetFloat("Horizontal", 20 * (2 * ViewPortPos.x - 1.0f));
        m_Animator.SetBool("isPushed", movementSettings.isPushed);

        //网络同步
        if (ctrlType == CtrlType.net)
        {
            //NetUpdate();
            return;
        }
        //玩家操控
        //正常飞行情况下
        if (!movementSettings.isPushed)
        {
            t += 0.01f;
            t = Mathf.Clamp(t, 0f, 1f);
            m_RigidBody.velocity = cam.transform.forward * movementSettings.CurrentTargetSpeed * t;

            Vector2 input = GetInput();

            if (Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon)
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.up * input.y + cam.transform.right * input.x;
                desiredMove = desiredMove.normalized;

                desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
                m_RigidBody.AddForce(desiredMove, ForceMode.Impulse);
            }
        }
        //被推开的情况下，不受任何控制
        else
        {
            //RotateView();
            Vector2 input = GetInput();

            t -= 0.01f;
            t = Mathf.Clamp(t, 0f, 1f);//计时器在清零后恢复控制
            if (t == 0f)
            {
                movementSettings.isPushed = false;
            }
            movementSettings.isRun = false;
        }
    }

    private Vector2 GetInput()//api
    {
        RotateView();
        Vector2 input = new Vector2
        {
            x = CrossPlatformInputManager.GetAxis("Horizontal"),
            y = CrossPlatformInputManager.GetAxis("Vertical")
        };
        movementSettings.UpdateDesiredTargetSpeed(input);
        SendUnitInfo();
        return input;
    }

    private void RotateView()
    {
        //avoids the mouse looking if the game is effectively paused
        if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

        // get the rotation before it's changed
        float oldYRotation = transform.eulerAngles.y;

        mouseLook.LookRotation(transform, cam.transform);
    }

    public void NetUpdateUnitInfo(Vector3 pos,Vector3 rot,int _isRun, int _isPushed)
    {
        transform.position = pos;
        transform.eulerAngles = rot;
        movementSettings.isRun = _isRun == 1 ? true : false;
        movementSettings.isPushed = _isPushed == 1 ? true : false;
    }

    public void SendUnitInfo()
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("UpdateUnitInfo");
        //位置和旋转
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        proto.AddFloat(pos.x);
        proto.AddFloat(pos.y);
        proto.AddFloat(pos.z);

        proto.AddFloat(rot.x);
        proto.AddFloat(rot.y);
        proto.AddFloat(rot.z);
        //is running & is pushed
        proto.AddInt(movementSettings.isRun?1 : 0);
        proto.AddInt(movementSettings.isPushed ? 1 : 0);
        NetMgr.srvConn.Send(proto);
    }

}
