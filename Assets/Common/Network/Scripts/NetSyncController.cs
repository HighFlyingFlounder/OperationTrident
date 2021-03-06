﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Reflection;


public class NetSyncController : MonoBehaviour
{
    //public UnityEngine.Object controller;
    [SerializeField]
    private List<Component> sync_scripts=new List<Component>();
    private string sync_id;
    public static bool isMasterClient = false;
    // Use this for initialization
    public void AddSyncScripts(Component Component){
        sync_scripts.Add(Component);
        (sync_scripts[sync_scripts.Count - 1] as NetSyncInterface).Init(this);
    }

    public void RemoveSyncScripts(Component Component)
    {
        sync_scripts.Remove(Component);
    }

    void Awake()
    {
        for (int i = 0; i < sync_scripts.Count; i++)
        {
            (sync_scripts[i] as NetSyncInterface).Init(this);
        }
        if (!GameMgr.instance)//GameMgr.instance没被初始化，则此时是离线状态
            return;
        //用名字来标识，GetInstanceID()可以获得任何对象独一无二的id，但在不同客户端或许不同
        sync_id = this.gameObject.name;
        NetMgr.srvConn.msgDist.AddListener(sync_id + "NetSyncController", RecvNetSync);
        NetMgr.srvConn.msgDist.AddListener(sync_id + "RPC", RecvRPC);
    }

    void OnDestroy()
    {
        NetMgr.srvConn.msgDist.DelListener(sync_id + "NetSyncController", RecvNetSync);
        NetMgr.srvConn.msgDist.DelListener(sync_id + "RPC", RecvRPC);
    }

    public void setSyncID(string _sync_id)
    {
        if (!GameMgr.instance)//GameMgr.instance没被初始化，则此时是离线状态
            return;
        NetMgr.srvConn.msgDist.DelListener(sync_id + "NetSyncController", RecvNetSync);
        NetMgr.srvConn.msgDist.DelListener(sync_id + "RPC", RecvRPC);
        sync_id = _sync_id;
        NetMgr.srvConn.msgDist.AddListener(sync_id + "NetSyncController", RecvNetSync);
        NetMgr.srvConn.msgDist.AddListener(sync_id + "RPC", RecvRPC);
    }

    void SendNetSync()
    {
        if (!GameMgr.instance)//GameMgr.instance没被初始化，则此时是离线状态
            return;
        //消息
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("BroadCast");
        proto.AddString(sync_id + "NetSyncController");
        //sync_id
        proto.AddString(sync_id);
        //sync_scripts
        for (int i = 0; i < sync_scripts.Count; i++)
        {
            //移除禁用脚本
            if( ((MonoBehaviour)sync_scripts[i]).enabled == false)
            {
                sync_scripts.Remove(sync_scripts[i]);
            }
            Component temp = sync_scripts[i];
            SyncData data = (temp as NetSyncInterface).SendData();
            //加入空检测
            if(data == null) {
                continue;
            }
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
        SyncData data = proto.GetSyncData(start, ref start);
        for (int i = 0; i < sync_scripts.Count; i++)
        {
            // SyncData data = proto.GetSyncData(start, ref start);
            Component temp = sync_scripts[i];
            (temp as NetSyncInterface).RecvData(data);
        }
    }

    //强制同步变量
    public void SyncVariables()
    {
        SendNetSync();
    }
    /// <summary> 
    /// 同步调用其他玩家客户端中此GameObject中此脚本中的此方法        
    /// </summary> 
    /// <param name="_this">此脚本类的object，统一用this</param> 
    /// <param name="methodName">函数名字</param>  
    /// <param name="args">函数的参数，可动态扩充长度</param>         
    /// <returns></returns> 
    public void RPC(object _this, string methodName, params object[] args)
    {
        if (!GameMgr.instance)//GameMgr.instance没被初始化，则此时是离线状态
            return;
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("RPC");
        proto.AddString(sync_id + "RPC");
        // componentName
        proto.AddString(_this.GetType().ToString());

        proto.AddString(methodName);
        // 问题1， 发送obj数组
        proto.AddObjects(args);
        NetMgr.srvConn.Send(proto);
    }

    public void RecvRPC(ProtocolBase protoBase)
    {
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        if (!protoName.Equals(sync_id + "RPC"))
            return;
        //丢弃自己发的信息
        string player_id = proto.GetString(start, ref start);
        //Debug.Log("player_id:" + player_id);
        if (player_id == GameMgr.instance.id)//丢弃自己发的信息
        {
            return;
        }

        string componentName = proto.GetString(start, ref start);
        string methodName = proto.GetString(start, ref start);

        object[] parameters = proto.GetObjects(start);

        for (int i = 0; i < parameters.Length; i++)
        {
            //Debug.Log("Parameters:" + parameters[i].ToString());
        }

        for (int i = 0; i < sync_scripts.Count; i++)
        {
            if (sync_scripts[i].GetType().ToString() != componentName)
                continue;
            //反射调用
            Type t = sync_scripts[i].GetType();
            MethodInfo method = t.GetMethod(methodName);
            if (method == null)
            {
                Debug.LogError("No public method in class " + t);
            }
            //Debug.Log("sync_scripts.GetType" + sync_scripts[i].GetType().ToString());
            method.Invoke(sync_scripts[i], parameters);
        }
    }
}
