using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using OperationTrident.Common.AI;
using UnityEngine.AI;
using System.Reflection;

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
    [SerializeField]
    private float syncPerSecond = 10;

    private Dictionary<string, GameObject> AI_List;
    // 提前获取Conponent, 提升Rpc速度
    private Dictionary<string, AIActionController> AI_Action_List;  
    private Dictionary<string, Vector3> AI_fPosition_List;
    private Dictionary<string, Vector3> AI_lPosition_List;
    private Dictionary<string, Vector3> AI_fRotation_List;
    private Dictionary<string, Vector3> AI_lRotation_List;
    public GameObject[] AIPrefabs;
    private NetSyncController m_NetSyncController;
    private int begin_id = 0;

    [SerializeField]
    private float Rpc_syncPerSecond = 20;
    float lastSendRpcTime = float.MinValue;
    private ProtocolBytes proto = new ProtocolBytes();
    private static object bufferlock = new object();
    private int protoCnt = 0;

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
        AI_Action_List = new Dictionary<string, AIActionController>();
        AI_fPosition_List = new Dictionary<string, Vector3>();
        AI_lPosition_List = new Dictionary<string, Vector3>();
        AI_fRotation_List = new Dictionary<string, Vector3>();
        AI_lRotation_List = new Dictionary<string, Vector3>();
        
        NetMgr.srvConn.msgDist.AddListener("AIRPC", RecvAIRPC);
    }

    public void OnDestroy()
    {
        NetMgr.srvConn.msgDist.DelListener("AIRPC", RecvAIRPC);
    }
    /// <summary> 
    /// 不会自动同步！将obj添加进AIController的管理，将确保AI的位置在所有客户端一致，以master-Client为准
    /// </summary> 
    /// <param name="obj">要添加的AI GameObject</param>     
    /// <returns></returns> 
    public void AddAIObject(GameObject obj)
    {
        if (!GameMgr.instance)//离线状态
        {
            return;
        }
        AI_List.Add(obj.name, obj);
        AI_Action_List.Add(obj.name, obj.GetComponent<AIActionController>());
        AI_fPosition_List.Add(obj.name, obj.transform.position);
        AI_lPosition_List.Add(obj.name, obj.transform.position);
        AI_fRotation_List.Add(obj.name, obj.transform.eulerAngles);
        AI_lRotation_List.Add(obj.name, obj.transform.eulerAngles);
    }

    /// <summary> 
    /// 在所有客户端，在名字为swopPoints的节点的子节点创建num个类型为type的AI，该事件会自动RPC同步，无需在调用的时候使用RPC来调用
    /// </summary> 
    /// <param name="num">创建的AI数量，应与swopPoints的节点的子节点数量一一对应</param> 
    /// <param name="type">AI Prefabs中的种类，索引从0开始</param>  
    /// <param name="swopPoints">场景中AI的生成点的节点名字，其子节点数量应与num对应</param>         
    /// <returns></returns> 
    [Obsolete]
    public void CreateAI(int num, int type, string swopPoints)
    {
        if (!GameMgr.instance)//离线状态
        {
            createAIImpl(num, type, swopPoints);
            return;
        }

        if (GameMgr.instance.isMasterClient)
        {
            createAIImpl(num, type, swopPoints);
            m_NetSyncController.RPC(this, "createAIImpl", num, type, swopPoints);
        }
    }

    public void CreateAI(int num, int type, string swopPoints, AIAgentInitParams args)
    {
        if (!GameMgr.instance)//离线状态
        {
            createAIImplement(num, type, swopPoints, args);
            return;
        }

        if (GameMgr.instance.isMasterClient)
        {
            Debug.Log("call CreateAI");
            createAIImplement(num, type, swopPoints, args);
            m_NetSyncController.RPC(this, "createAIImplement", num, type, swopPoints, args);
        }
    }

    public void createAIImplement(int num, int type, string swopPoints, AIAgentInitParams args)
    {
        Debug.Log("createAIImplement");
        if (GameObject.Find(swopPoints) == null)
        {
            Debug.LogError("SwopPoints Not Found!");
            return;
        }
        Transform sp = GameObject.Find(swopPoints).transform;
        Transform swopTrans;
        Debug.Log("createAIImplement");
        for (int i = 0; i < num; i++)
        {
            swopTrans = sp.GetChild(i);
            if (swopTrans == null)
            {
                Debug.LogError("GeneratePlayer出生点错误！");
                return;
            }
            GameObject AI = (GameObject)Instantiate(AIPrefabs[type], swopTrans.position, swopTrans.rotation);
            //非master_client需要对AI进行裁剪或者增添
            if (!is_master_client)
            {
                //尝试禁用掉非master_client的ai组件
                if (AI.GetComponent<WanderAIAgent>() != null)
                    AI.GetComponent<WanderAIAgent>().enabled = false;
                if (AI.GetComponent<NavMeshAgent>() != null)
                    AI.GetComponent<NavMeshAgent>().enabled = false;
            }

            //创建的AI初始化信息
            begin_id++;
            AI.name = "AI" + begin_id;
            
            AI.GetComponent<NetSyncController>().setSyncID(AI.name);

            AI.GetComponent<AIAgent>().SetInitParams(args);
            AI_List.Add(AI.name, AI);
            AI_Action_List.Add(AI.name, AI.GetComponent<AIActionController>());
            //初始化位置和转向预测数据
            AI_fPosition_List.Add(AI.name, AI.transform.position);
            AI_lPosition_List.Add(AI.name, AI.transform.position);
            AI_fRotation_List.Add(AI.name, AI.transform.eulerAngles);
            AI_lRotation_List.Add(AI.name, AI.transform.eulerAngles);
        }
    }

    //本地创建AI，不同步
    [Obsolete]
    public void createAIImpl(int num, int type, string swopPoints)
    {
        Transform sp = GameObject.Find(swopPoints).transform;
        Transform swopTrans;
        Debug.Log("createAIImpl");
        for (int i = 0; i < num; i++)
        {
            swopTrans = sp.GetChild(i);
            if (swopTrans == null)
            {
                Debug.LogError("GeneratePlayer出生点错误！");
                return;
            }
            GameObject AI = (GameObject)Instantiate(AIPrefabs[type], swopTrans.position, swopTrans.rotation);
            //非master_client需要对AI进行裁剪或者增添
            if (!is_master_client)
            {
                //尝试禁用掉非master_client的ai组件
                if (AI.GetComponent<WanderAIAgent>() != null)
                    AI.GetComponent<WanderAIAgent>().enabled = false;
                if (AI.GetComponent<NavMeshAgent>() != null)
                    AI.GetComponent<NavMeshAgent>().enabled = false;
            }

            //创建的AI初始化信息
            begin_id++;
            AI.name = "AI" + begin_id;
            //AI.transform.position = swopTrans.position;
            //AI.transform.rotation = swopTrans.rotation;
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
        if (!GameMgr.instance)//单机状态
        {
            return;
        }
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
            // 每秒发送Rpc_syncPerSecond次AI RPC
            if (Time.time - lastSendRpcTime > 1.0f / Rpc_syncPerSecond)
            {
                if (protoCnt!=0)
                {
                    lock (bufferlock)
                    {
                        ProtocolBytes AIRPC_proto = new ProtocolBytes();
                        AIRPC_proto.AddString("AIRPC");
                        AIRPC_proto.AddInt(protoCnt);
                        AIRPC_proto.bytes = AIRPC_proto.bytes.Concat(proto.bytes).ToArray();
                        NetMgr.srvConn.Send(AIRPC_proto);
                        //Debug.LogFormat("Reach Send AIRPC in Update, protoCnt = {0}, bytesLength = {1}", protoCnt, AIRPC_proto.bytes.Length);
                        // 更新缓冲区
                        proto = new ProtocolBytes();
                        protoCnt = 0;
                        lastSendRpcTime = Time.time;
                    }                    
                }
            }
        }
        else //非master_client接受网络同步并预测更新
        {
            NetUpdate();
        }

    }

    public void DestroyAI(string AI_name)
    {
        Debug.LogFormat("DestroyAI : {0}", AI_name);
        AI_List.Remove(AI_name);
        AI_Action_List.Remove(AI_name);  // 此时Remove会无法接受Destroy()
        AI_fPosition_List.Remove(AI_name);
        AI_lPosition_List.Remove(AI_name);
        AI_fRotation_List.Remove(AI_name);
        AI_lRotation_List.Remove(AI_name);
    }

    public void ClearAI()
    {
        AI_List.Clear();
        AI_Action_List.Clear();
        AI_fPosition_List.Clear();
        AI_lPosition_List.Clear();
        AI_fRotation_List.Clear();
        AI_lRotation_List.Clear();
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
        //更新
        AI_lPosition_List[id] = nPos;
        AI_lRotation_List[id] = nRot;

    }

    public void NetUpdate()
    {
        foreach (var ai in AI_List)
        {
            //当前ai的key
            string id = ai.Key;
            //Debug.Log("NetUpdate" + id);
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
        //时间
        delta = Time.time - lastRecvInfoTime;
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
                //Debug.Log("RecvData" + id);

                NetForecastInfo(id, nPosition, nRotation);
            }
            else
            {
                data.Get(typeof(Vector3));
                data.Get(typeof(Vector3));
            }
        }
        lastRecvInfoTime = Time.time;
    }

    public SyncData SendData()
    {
        SyncData data = new SyncData();
        data.Add(1);//防止空
        foreach (var ai in AI_List)
        {
            if(ai.Value == null)
            {
                Debug.LogFormat("ai_name:{0}", ai.Key);
            }
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

    // AIRPC将所有AI的RPC进行合包操作
    public void AIRPC(string AIName, string methodName, params object[] args)
    {
        if (!GameMgr.instance)//GameMgr.instance没被初始化，则此时是离线状态
            return;
        
        lock (bufferlock)
        {
            ++protoCnt;
            proto.AddString(AIName);
            proto.AddString(methodName);
            proto.AddAIObjects(args);
            // 快满了直接发送，保留1K空间
            if(proto.bytes.Length > 3072 )
            {
                ProtocolBytes AIRPC_proto = new ProtocolBytes();
                AIRPC_proto.AddString("AIRPC");
                AIRPC_proto.AddInt(protoCnt);
                AIRPC_proto.bytes = AIRPC_proto.bytes.Concat(proto.bytes).ToArray();
                NetMgr.srvConn.Send(AIRPC_proto);
                // 更新缓冲区
                proto = new ProtocolBytes();
                protoCnt = 0;
            }
        }
    }

    public void RecvAIRPC(ProtocolBase protoBase)
    {
        //Debug.Log("Reach RecvAIRPC");
        if (is_master_client)
            return;
        
        ProtocolBytes proto = (ProtocolBytes)protoBase;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        // master_client 直接忽略AI的RPC信息
        
        int rpcCnt = proto.GetInt(start, ref start);
        //Debug.Log("rpcCnt = " + rpcCnt);
        for (int i = 0; i < rpcCnt; i++)
        {
            string aiName = proto.GetString(start, ref start);
            string methodName = proto.GetString(start, ref start);
            object[] parameters = proto.GetObjects(start, ref start);   // 应该为(start, ref start)
            //Debug.Log("aiName :" + aiName);
            //Debug.Log("methodName:" + methodName);
            if (AI_List.ContainsKey(aiName))
            {
                AIActionController actionController = AI_Action_List[aiName];
                Type t = actionController.GetType();
                //Debug.Log("Type t:" + t);
                MethodInfo method = t.GetMethod(methodName);
                if (method == null)
                {
                    Debug.LogError("No public method in class " + t);
                }
                method.Invoke(actionController, parameters);
            }
        }
    }
}