using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class FlyerController : MonoBehaviour, NetSyncInterface
{
    private NetSyncController m_NetSyncController;
    private Rigidbody m_RigidBody;
    private Animator m_Animator;
    public float Speed = 8.0f;//向前的速度（向前不可控制）
    public float OffsetSpeed = 4.0f;//在xy轴上的偏移速度（上下左右偏移可控制）
    public float Hp = 100f;
    private float t = 0.0f;//计时器,在喷射系统从零开始加速时使用
    private bool isPushed = false;

    // Use this for initialization
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
    }

    private Vector2 GetInput()
    {
        Vector2 input = new Vector2
        {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical")
        };
        return input;
    }

    private void FixedUpdate()
    {
        if (!isPushed)
        {
            t += 0.01f;
            t = Mathf.Clamp(t, 0f, 1f);
            m_RigidBody.velocity = transform.forward * Speed * t;//一直向前的速度

            Vector2 input = GetInput();

            m_Animator.SetFloat("Vertical", input.y);
            m_Animator.SetFloat("Horizontal", input.x);
            m_Animator.SetBool("isPushed", isPushed);//播放被推开的动画，要不放到碰撞检测的函数里？

            if (Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon)
            {
                //Vector3 move = new Vector3(input.x, input.y, 0f);
                Vector3 move = transform.up * input.y + transform.right * input.x;
                move = move.normalized * OffsetSpeed;
                m_RigidBody.AddForce(move, ForceMode.Impulse);

                //move = move * OffsetSpeed;
                //move.z = Speed * t;
                //m_RigidBody.velocity = move;
            }
        }
        else
        {
            Vector2 input = GetInput();

            //m_Animator.SetFloat("Vertical", input.y);
            //m_Animator.SetFloat("Horizontal", input.x);
            m_Animator.SetBool("isPushed", isPushed);//播放被推开的动画，要不放到碰撞检测的函数里？

            t -= 0.01f;
            t = Mathf.Clamp(t, 0f, 1f);//计时器在清零后恢复控制
            if (t == 0f)
            {
                isPushed = false;
                m_NetSyncController.SyncVariables();
            }
        }

        //空气墙
        /*
        if (transform.position.x > 1900f || transform.position.x < 0f || transform.position.y > 300f || transform.position.y < -300f || transform.position.z > 200f || transform.position.z < -200f)
        {
            float x = Mathf.Clamp(transform.position.x, 0f, 1900f);
            float y = Mathf.Clamp(transform.position.y, -300f, 300f);
            float z = Mathf.Clamp(transform.position.z, -200f, 200f);
            transform.position = new Vector3(x, y, z);
        }
        */
    }

    void ChangeHp(float x)
    {
        Hp += x;
        Hp = Mathf.Clamp(Hp, 0f, 100f);
    }

    void OnCollisionEnter(Collision other)
    {
        //只有本地玩家才会触发这些事件
        if (transform.GetComponent<NetSyncTransform>().ctrlType == NetSyncTransform.CtrlType.player)
        {
            ChangeHp(other.gameObject.GetComponent<Hinder>().damage);
            //ChangeHp(-40.0f);
            Vector3 Force = this.transform.position - other.transform.position;
            //m_RigidBody.velocity = Force * 2f;//往反方向推
            m_RigidBody.velocity = new Vector3(0f, 0f, 0f);
            m_RigidBody.AddForce(Force * 80f);//往反方向推
            isPushed = true;
            m_NetSyncController.SyncVariables();
        }
        if (Hp == 0f)
        {
            this.GetComponent<Collider>().enabled = false;
            SendDead();
        }
    }

    public void RecvData(SyncData data)
    {
        isPushed = (bool)(data.Get(typeof(bool)));
    }

    public SyncData SendData()
    {
        //消息
        SyncData data = new SyncData();
        data.Add(isPushed);
        return data;
    }

    public void Init(NetSyncController controller)
    {
        m_NetSyncController = controller;
    }

    private void OnTriggerEnter(Collider other)
    {
        //只有本地玩家才会触发这些事件
        if (transform.GetComponent<NetSyncTransform>().ctrlType == NetSyncTransform.CtrlType.player)
        {
            if (other.tag == "End")
            {
                //Flyer.GetComponent<UserController>().enabled = false;
                //Flyer.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                //Flyer.transform.position = other.transform.position;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                this.gameObject.SetActive(false);
            }
        }

    }

    public void SendDead()
    {
        ProtocolBytes proto = new ProtocolBytes();
        Debug.Log("dead");
        proto.AddString("Dead");
        NetMgr.srvConn.Send(proto);
    }

}
