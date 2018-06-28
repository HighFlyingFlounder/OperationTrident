using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class NetSyncTransform : MonoBehaviour
{

    public enum CtrlType
    {
        none,
        player,
        computer,
        net,
    }
    [HideInInspector]
    public CtrlType ctrlType = CtrlType.none;
    private string sync_id;
    //last 上次的位置信息
    Vector3 lPos;
    Vector3 lRot;
    //forecast 预测的位置信息
    Vector3 fPos;
    Vector3 fRot;
    //时间间隔
    float delta = 1;
    //上次发送的时间
    private float lastSendInfoTime = float.MinValue;
    //上次接收的时间
    float lastRecvInfoTime = float.MinValue;
    [SerializeField]
    private float syncPerSceond = 10;
    [SerializeField]
    private bool ForcastSync = true;
    // Use this for initialization
    void Start()
    {
        if (!GameMgr.instance)//GameMgr.instance没被初始化，则此时是离线状态
            return;
        //用名字来标识，GetInstanceID()可以获得任何对象独一无二的id，但在不同客户端或许不同
        sync_id = this.gameObject.name;
        NetMgr.srvConn.msgDist.AddListener(sync_id + "NetSyncTransform", RecvNetSync);
    }

    //玩家控制
    public void PlayerCtrl()
    {
        if (syncPerSceond == 0f)
        {
            return;
        }
        //网络同步
        if (Time.time - lastSendInfoTime > 1.0f / syncPerSceond)
        {
            SendNetSync();
            lastSendInfoTime = Time.time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //网络同步
        if (ctrlType == CtrlType.net)
        {
            if (ForcastSync)
                NetUpdate();
            return;
        }
        if (ctrlType == CtrlType.player)
            PlayerCtrl();

    }

    public void NetForecastInfo(Vector3 nPos, Vector3 nRot)
    {
        //预测的位置
        fPos = lPos + (nPos - lPos) * 2;
        fRot = lRot + (nRot - lRot) * 2;
        if (Time.time - lastRecvInfoTime > 0.3f)
        {
            fPos = nPos;
            fRot = nRot;
        }
        //时间
        delta = Time.time - lastRecvInfoTime;
        //更新
        lPos = nPos;
        lRot = nRot;
        lastRecvInfoTime = Time.time;
    }

    public void NetTransformInfo(Vector3 nPos, Vector3 nRot)
    {
        transform.position = nPos;
        //transform.rotation = Quaternion.Euler(nRot);
        transform.eulerAngles = nRot;
    }

    //初始化位置预测数据
    public void InitNetCtrl()
    {
        lPos = transform.position;
        lRot = transform.eulerAngles;
        fPos = transform.position;
        fRot = transform.eulerAngles;
        //Rigidbody r = GetComponent<Rigidbody>();
        //r.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void NetUpdate()
    {
        //当前位置
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        //更新位置
        if (delta > 0)
        {
            transform.position = Vector3.Lerp(pos, fPos, delta);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(rot),
                                            Quaternion.Euler(fRot), delta);
        }
    }

    void SendNetSync()
    {
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        //消息
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("BroadCast");
        proto.AddString(sync_id + "NetSyncTransform");
        //sync_id
        proto.AddString(sync_id);
        //POS
        proto.AddFloat(pos.x);
        proto.AddFloat(pos.y);
        proto.AddFloat(pos.z);
        //rot
        proto.AddFloat(rot.x);
        proto.AddFloat(rot.y);
        proto.AddFloat(rot.z);

        NetMgr.srvConn.Send(proto);
    }

    public void RecvNetSync(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        if (!protoName.Equals(sync_id + "NetSyncTransform"))
            return;

        string player_id = proto.GetString(start, ref start);
        if (player_id == GameMgr.instance.id)//丢弃自己发的信息
        {
            return;
        }

        string _sync_id = proto.GetString(start, ref start);
        if (_sync_id != sync_id)//不是该物体要同步信息
            return;
        //Pos
        float x = proto.GetFloat(start, ref start);
        float y = proto.GetFloat(start, ref start);
        float z = proto.GetFloat(start, ref start);
        //rotation
        float rotx = proto.GetFloat(start, ref start);
        float roty = proto.GetFloat(start, ref start);
        float rotz = proto.GetFloat(start, ref start);

        if (ForcastSync)
            NetForecastInfo(new Vector3(x, y, z), new Vector3(rotx, roty, rotz));
        else
            NetTransformInfo(new Vector3(x, y, z), new Vector3(rotx, roty, rotz));
    }
}
