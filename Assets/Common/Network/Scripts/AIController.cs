using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIController : MonoBehaviour, NetSyncInterface
{
    public static AIController instance;
    //时间间隔
    float delta = 1;
    //上次发送的时间
    private float lastSendInfoTime = float.MinValue;
    //上次接收的时间
    float lastRecvInfoTime = float.MinValue;
    bool is_master_client = false;
    private float syncPerSecond = 10;
    private Dictionary<string,GameObject> AI_List;
    public GameObject[] AIPrefabs;
    private NetSyncController m_NetSyncController;

    void Awake()
    {
        instance = this;
    }

    public void createAI(int num, params object[] args)
    {

    }

    public void Update() 
    {
        if (syncPerSecond == 0f)
        {
            return;
        }
        if (is_master_client)//master_client发布信息
        {
            if (Time.time - lastSendInfoTime > 1.0f / syncPerSecond)
            {
                m_NetSyncController.SyncVariables();
                lastSendInfoTime = Time.time;
            }
        }
        else //非master_client接受网络同步并预测更新
        {
            //NetUpdate();
        }
        
    }

    public void DestroyAI(int id)
    {

    }

    public void RecvData(SyncData data)
    {
        foreach (var ai in AI_List)
        {
            string id = data.GetString();
            AI_List[id].transform.position = (Vector3)data.Get(typeof(Vector3));
            AI_List[id].transform.eulerAngles = (Vector3)data.Get(typeof(Vector3));
        }
    }

    public SyncData SendData()
    {
        SyncData data = new SyncData();
        foreach(var ai in AI_List)
        {
            data.AddString(ai.Key);
            data.Add(ai.Value.transform.position);
            data.Add(ai.Value.transform.eulerAngles);
        }
        return data;
    }

    public void Init(NetSyncController controller)
    {
        m_NetSyncController = controller;
    }
}