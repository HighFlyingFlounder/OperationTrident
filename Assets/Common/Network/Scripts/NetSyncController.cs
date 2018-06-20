using UnityEngine;
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
    private string sync_id;
    // Use this for initialization

    private void Awake() {
        for(int i = 0; i < sync_scripts.Length; i++) {
            (sync_scripts[i] as NetSyncInterface).Init(this);
        }
    }

    void Start()
    {
        //用名字来标识，GetInstanceID()可以获得任何对象独一无二的id，但在不同客户端或许不同
        sync_id = this.gameObject.name;
        NetMgr.srvConn.msgDist.AddListener(sync_id + "NetSyncController", RecvNetSync);
    }

    void SendNetSync()
    {
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        //消息
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("BroadCast");
        proto.AddString(sync_id + "NetSyncController");
        //sync_id
        proto.AddString(sync_id);
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
        if (!protoName.Equals(sync_id + "NetSyncController"))
            return;

        string player_id = proto.GetString(start, ref start);
        if (player_id == GameMgr.instance.id)//丢弃自己发的信息
        {
            return;
        }

        string _sync_id = proto.GetString(start, ref start);
        if (_sync_id != sync_id)//不是该物体要同步信息
            return;

        //sync_scripts
        for (int i = 0; i < sync_scripts.Length; i++)
        {
            SyncData data = proto.GetSyncData(start, ref start);
            Component temp = sync_scripts[i];
            (temp as NetSyncInterface).SetData(data);
        }
    }

    //强制同步变量
    public void SyncVariables() {
        SendNetSync();
    }
}
