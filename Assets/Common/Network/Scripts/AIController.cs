using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using OperationTrident.Common.AI;
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
    private Dictionary<string, GameObject> AI_List;
    private Dictionary<string, Vector3> AI_fPosition_List;
    private Dictionary<string, Vector3> AI_lPosition_List;
    private Dictionary<string, Vector3> AI_fRotation_List;
    private Dictionary<string, Vector3> AI_lRotation_List;
    public GameObject[] AIPrefabs;
    private NetSyncController m_NetSyncController;

    void Awake()
    {
        instance = this;
        try
        {
            is_master_client = GameMgr.instance.isMasterClient;
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("Exception:" + ex);
        }
        AI_List = new Dictionary<string, GameObject>();
        AI_fPosition_List = new Dictionary<string, Vector3>();
        AI_lPosition_List = new Dictionary<string, Vector3>();
        AI_fRotation_List = new Dictionary<string, Vector3>();
        AI_lRotation_List = new Dictionary<string, Vector3>();
    }

    void Start()
    {
    }

    public void createAI(int num, int type, string swopPoints)
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
            //非master_client需要对AI进行裁剪或者增添
            if (!is_master_client)
            {
                //禁用掉非master_client的ai组件
                AI.GetComponent<WanderAIAgent>().enabled = false;
                //AI.GetComponent<NavMeshAgent>().enabled = false;
            }

            //创建的AI初始化信息
            AI.name = "AI" + (i + begin_id);
            AI.transform.position = swopTrans.position;
            AI.transform.rotation = swopTrans.rotation;
            AI_List.Add(AI.name, AI);

            //初始化位置和转向预测数据
            AI_fPosition_List.Add(AI.name, AI.transform.position);
            AI_lPosition_List.Add(AI.name, AI.transform.position);
            AI_fRotation_List.Add(AI.name, AI.transform.eulerAngles);
            AI_lRotation_List.Add(AI.name, AI.transform.eulerAngles);
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
            NetUpdate();
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

    public void NetForecastInfo(string id, Vector3 nPos, Vector3 nRot)
    {
        //预测的位置
        AI_fPosition_List[id] = AI_lPosition_List[id] + (nPos - AI_lPosition_List[id]) * 2;
        AI_fRotation_List[id] = AI_lRotation_List[id] + (nRot - AI_lRotation_List[id]) * 2;
        if (Time.time - lastRecvInfoTime > 0.3f)
        {
            AI_fPosition_List[id] = nPos;
            AI_fRotation_List[id] = nRot;
        }
        //时间
        delta = Time.time - lastRecvInfoTime;
        //更新
        AI_lPosition_List[id] = nPos;
        AI_lRotation_List[id] = nRot;
        lastRecvInfoTime = Time.time;
    }

    public void NetUpdate()
    {
        foreach (var ai in AI_List)
        {
            //当前ai的key
            string id = ai.Key;
            //当前位置
            Vector3 pos = ai.Value.transform.position;
            Vector3 rot = ai.Value.transform.eulerAngles;
            //更新位置
            if (delta > 0)
            {
                ai.Value.transform.position = Vector3.Lerp(pos, AI_fPosition_List[id], delta);
                ai.Value.transform.rotation = Quaternion.Lerp(Quaternion.Euler(rot),
                                                Quaternion.Euler(AI_fRotation_List[id]), delta);
            }
        }
    }


    public void RecvData(SyncData data)
    {
        int notnull = (int)data.Get(typeof(int));
        foreach (var ai in AI_List)
        {
            string id = data.GetString();
            if (AI_List.ContainsKey(id))
            {
                //无预测同步
                //AI_List[id].transform.position = (Vector3)data.Get(typeof(Vector3));
                //AI_List[id].transform.eulerAngles = (Vector3)data.Get(typeof(Vector3));

                //预测同步
                Vector3 nPosition = (Vector3)data.Get(typeof(Vector3));
                Vector3 nRotation = (Vector3)data.Get(typeof(Vector3));
                NetForecastInfo(id, nPosition, nRotation);
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
        data.Add(1);//防止空
        foreach (var ai in AI_List)
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