﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class NetSyncController : MonoBehaviour
{
    //public UnityEngine.Object controller;
    public Component[] sync_scripts;
    public enum CtrlType
    {
        none,
        player,
        computer,
        net,
    }
    public CtrlType ctrlType = CtrlType.player;
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
    private int syncPerSceond = 10;
    [SerializeField]
    private bool syncPos = true;
    [SerializeField]
    private bool synRot = true;
    // Use this for initialization
    void Start()
    {
        //用名字来标识，GetInstanceID()可以获得任何对象独一无二的id，但在不同客户端或许不同
        sync_id = this.gameObject.name;
        NetMgr.srvConn.msgDist.AddListener(sync_id + "NetSync", RecvNetSync);
    }

    //玩家控制
    public void PlayerCtrl()
    {
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
            NetUpdate();
            return;
        }
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
            if (syncPos)
                transform.position = Vector3.Lerp(pos, fPos, delta);
            if (synRot)
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
        proto.AddString(sync_id + "NetSync");
        //sync_id
        proto.AddString(sync_id);
        if (syncPos)
        {
            //POS
            proto.AddFloat(pos.x);
            proto.AddFloat(pos.y);
            proto.AddFloat(pos.z);
        }
        if (synRot)
        {
            //rot
            proto.AddFloat(rot.x);
            proto.AddFloat(rot.y);
            proto.AddFloat(rot.z);
        }
        //sync_scripts
        for (int i = 0; i < sync_scripts.Length; i++)
        {
            Component temp = sync_scripts[i];
            SyncData data = (temp as NetSyncInterface).GetData();
            proto.AddSyncData(data);
        }
        NetMgr.srvConn.Send(proto);
    }

    public void RecvNetSync(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        if (!protoName.Equals(sync_id + "NetSync"))
            return;

        string player_id = proto.GetString(start, ref start);
        if (player_id == GameMgr.instance.id)//丢弃自己发的信息
        {
            return;
        }

        string _sync_id = proto.GetString(start, ref start);
        if (_sync_id != sync_id)//不是该物体要同步信息
            return;

        float x = 0, y = 0, z = 0;
        if (syncPos)
        {
            //Pos
            x = proto.GetFloat(start, ref start);
            y = proto.GetFloat(start, ref start);
            z = proto.GetFloat(start, ref start);
        }
        float rotx = 0, roty = 0, rotz = 0;
        if (synRot)
        {
            //rotation
            rotx = proto.GetFloat(start, ref start);
            roty = proto.GetFloat(start, ref start);
            rotz = proto.GetFloat(start, ref start);
        }

        //sync_scripts
        for (int i = 0; i < sync_scripts.Length; i++)
        {
            SyncData data = proto.GetSyncData(start, ref start);
            Component temp = sync_scripts[i];
            (temp as NetSyncInterface).SetData(data);
        }

        NetForecastInfo(new Vector3(x, y, z), new Vector3(rotx, roty, rotz));
    }
}