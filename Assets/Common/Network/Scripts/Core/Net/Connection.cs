using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

//网络链接
public class Connection
{
    //常量
    const int BUFFER_SIZE = 1024;
    //Socket
    private Socket socket;
    //Buff
    private byte[] readBuff = new byte[BUFFER_SIZE];
    private int buffCount = 0;
    //沾包分包
    private Int32 msgLength = 0;
    private byte[] lenBytes = new byte[sizeof(Int32)];
    //协议
    public ProtocolBase proto;
    //心跳时间
    public float lastTickTime = 0;
    public float heartBeatTime = 30;
    //最新测量延时(单位:ms)
    public int RTT = 0;
    //延时测量间隔
    public float lastSendDelayTime = 0;
    public float lastRecvDelayTime = 0;
    public float testDelayTime = 1.5;
    //消息分发
    public MsgDistribution msgDist = new MsgDistribution();
    //高精度计时器
    public Stopwatch watch;




    ///状态
    public enum Status
    {
        None,
        Connected,
    };
    public Status status = Status.None;


    //连接服务端
    public bool Connect(string host, int port)
    {
        try
        {
            //socket
            socket = new Socket(AddressFamily.InterNetwork,
                      SocketType.Stream, ProtocolType.Tcp);
            //Connect
            socket.Connect(host, port);
            //BeginReceive
            socket.BeginReceive(readBuff, buffCount,
                      BUFFER_SIZE - buffCount, SocketFlags.None,
                      ReceiveCb, readBuff);
            Debug.Log("连接成功");
            //Init StopWatch
            watch = new Stopwatch();
            msgDist.AddListener("Delay", calcDelay);
            //状态
            status = Status.Connected;
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("连接失败:" + e.Message);
            return false;
        }
    }

    //关闭连接
    public bool Close()
    {
        try
        {
            socket.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("关闭失败:" + e.Message);
            return false;
        }
    }

    //接收回调
    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            buffCount = buffCount + count;
            ProcessData();
            socket.BeginReceive(readBuff, buffCount,
                     BUFFER_SIZE - buffCount, SocketFlags.None,
                     ReceiveCb, readBuff);
        }
        catch (Exception e)
        {
            Debug.Log("ReceiveCb失败:" + e.Message);
            status = Status.None;
        }
    }

    //消息处理
    private void ProcessData()
    {
        //小于长度字节
        if (buffCount < sizeof(Int32))
            return;
        //消息长度
        Array.Copy(readBuff, lenBytes, sizeof(Int32));
        msgLength = BitConverter.ToInt32(lenBytes, 0);
        if (buffCount < msgLength + sizeof(Int32))
            return;
        //处理消息
        ProtocolBase protocol = proto.Decode(readBuff, sizeof(Int32), msgLength);
        //Debug.Log("收到消息 " + protocol.GetDesc());
        lock (msgDist.msgList)
        {
            msgDist.msgList.Add(protocol);
        }
        //清除已处理的消息
        int count = buffCount - msgLength - sizeof(Int32);
        Array.Copy(readBuff, sizeof(Int32)+msgLength, readBuff, 0, count);
        buffCount = count;
        if (buffCount > 0)
        {
            ProcessData();
        }
    }


    public bool Send(ProtocolBase protocol)
    {
        if (status != Status.Connected)
        {
            Debug.LogError("[Connection]还没链接就发送数据是不好的");
            return true;
        }

        byte[] b = protocol.Encode();
        byte[] length = BitConverter.GetBytes(b.Length);

        byte[] sendbuff = length.Concat(b).ToArray();
        socket.Send(sendbuff);
        //Debug.Log("发送消息 " + protocol.GetDesc());
        return true;
    }

    public bool Send(ProtocolBase protocol, string cbName, MsgDistribution.Delegate cb)
    {
        if (status != Status.Connected)
            return false;
        msgDist.AddOnceListener(cbName, cb);
        return Send(protocol);
    }

    public bool Send(ProtocolBase protocol, MsgDistribution.Delegate cb)
    {
        string cbName = protocol.GetName();
        return Send(protocol, cbName, cb);
    }

    public int getDelay(){
        return RTT;
    }

    public void calcDelay(ProtocolBase protoBase){
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        string protoName = proto.GetString(start, ref start);
        long send_time = proto.GetLong(start, ref start);
        long end_time = Stopwatch.GetTimestamp();
        RTT = (end_time - send_time) / (Stopwatch.Frequency / 1000);
    }

    public void Update()
    {
        //消息
        msgDist.Update();
        //心跳
        if (status == Status.Connected)
        {
            if (Time.time - lastTickTime > heartBeatTime)
            {
                ProtocolBase protocol = NetMgr.GetHeatBeatProtocol();
                Send(protocol);
                lastTickTime = Time.time;
            }
            if (Time.time - lastDelayTime > testDelayTime)
            {
                // 上次发送的包还没有收到回包
                if( lastRecvDelayTime < lastSendDelayTime ){
                    RTT = 460; //说明网络延时过高,需要更新RTT为一个很大的值.
                }
                ProtocolBase protocol = NetMgr.GetDelayProtocol();
                Send(protocol);
                lastSendDelayTime = Time.time;
            }
        }
    }
}