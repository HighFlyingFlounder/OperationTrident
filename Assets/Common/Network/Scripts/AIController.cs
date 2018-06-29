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
        try
        {
            is_master_client = GameMgr.instance.isMasterClient;
        }
        catch(NullReferenceException ex)
        {
            Debug.Log("Exception:" + ex);
        }
        AI_List = new Dictionary<string, GameObject>();
    }

    void Start()
    {
    }

    public void createAI(int num, int type, string swopPoints, params object[] args)
    {
        Transform sp = GameObject.Find(swopPoints).transform;
        Transform swopTrans;
        int begin_id = AI_List.Count;
        for (int i = 0; i < num; i++)
        {
            swopTrans = sp.GetChild(i);
            if (swopTrans == null)
            {
                Debug.LogError("GeneratePlayer出生点错误！");
                return;
            }
            GameObject AI = (GameObject)Instantiate(AIPrefabs[type]);
            AI.name = "AI" + (i + begin_id);
            AI.transform.position = swopTrans.position;
            AI.transform.rotation = swopTrans.rotation;
            AI_List.Add(AI.name, AI);
        }
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

    public void DestroyAI(string AI_name)
    {
        AI_List.Remove(AI_name);
    }

    public void ClearAI()
    {
        AI_List.Clear();
    }

    public void RecvData(SyncData data)
    {
        foreach (var ai in AI_List)
        {
            string id = data.GetString();
            if (AI_List.ContainsKey(id))
            {
                AI_List[id].transform.position = (Vector3)data.Get(typeof(Vector3));
                AI_List[id].transform.eulerAngles = (Vector3)data.Get(typeof(Vector3));
            }
            else
            {
                data.Get(typeof(Vector3));
                data.Get(typeof(Vector3));
            }
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