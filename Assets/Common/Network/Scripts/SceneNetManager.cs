using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.CrossInput;
using OperationTrident.Player;

public class SceneNetManager : MonoBehaviour
{
    public static SceneNetManager instance;
    //玩家预设
    public GameObject[] PlayerPrefabs;
    //游戏中给所有的角色
    public Dictionary<string, GameObject> list;

    void Awake()
    {
        instance = this;
        if (!GameMgr.instance)//GameMgr.instance没被初始化，则此时是离线状态
            return;
        //协议
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("FinishLoading");
        NetMgr.srvConn.Send(protocol);
        //监听
        NetMgr.srvConn.msgDist.AddListener("FinishLoading", RecvLoading);
    }
    // Use this for initialization
    void Start()
    {
        if (!GameMgr.instance)//GameMgr.instance没被初始化，则此时是离线状态
            return;
        if (GameObject.FindGameObjectWithTag("Player"))
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        StartGame();
    }
    //开始一场游戏的准备工作
    void StartGame()
    {
        list = new Dictionary<string, GameObject>();
        //协议
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("StartFight");
        NetMgr.srvConn.Send(protocol);
        //监听
        NetMgr.srvConn.msgDist.AddListener("StartFight", RecvStartFight);
    }

    //清理场景
    public void ClearBattle()
    {
        //自行删除掉list中的所有玩家
        foreach (var obj in list)
            Destroy(obj.Value);
        list.Clear();
    }

    //开始战斗
    public void StartBattle(ProtocolBytes proto)
    {
        //解析协议
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        //清理场景
        ClearBattle();
        //玩家总数
        int count = proto.GetInt(start, ref start);
        //每一个玩家
        for (int i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            Debug.Log("id = " + id);
            int swopID = proto.GetInt(start, ref start);
            GeneratePlayer(id, swopID);
        }
    }

    //产生玩家
    public void GeneratePlayer(string id, int swopID)
    {
        //获取出生点
        Transform sp = GameObject.Find("SwopPoints").transform;
        Transform swopTrans;
        swopTrans = sp.GetChild(swopID - 1);
        if (swopTrans == null)
        {
            Debug.LogError("GeneratePlayer出生点错误！");
            return;
        }
        //产生玩家角色 0暂时代表只有一种玩家prefab
        GameObject playerObj = (GameObject)Instantiate(PlayerPrefabs[0]);
        playerObj.name = id;
        playerObj.transform.position = swopTrans.position;
        playerObj.transform.rotation = swopTrans.rotation;

        list.Add(id, playerObj);
        //玩家通用处理
        NetSyncTransform netsyn = playerObj.GetComponent<NetSyncTransform>();
        if (id == GameMgr.instance.id)
        {
            netsyn.ctrlType = NetSyncTransform.CtrlType.player;//CtrlType默认为none，none不发送消息，模拟单人模式
            playerObj.GetComponent<InputManager>().IsLocalPlayer = true;
        }
        else
        {

            netsyn.ctrlType = NetSyncTransform.CtrlType.net;
            playerObj.GetComponent<InputManager>().IsLocalPlayer = false;
            playerObj.GetComponent<PlayerController>().enabled = false;
            playerObj.transform.Find("Camera").gameObject.GetComponent<Camera>().enabled = false;
            playerObj.transform.Find("Camera").gameObject.GetComponent<AudioListener>().enabled = false;
        }
        //玩家特殊处理，例如禁用掉某些脚本或者添加新的脚本
        HandlePlayer(id,playerObj);
    }
    /// <summary> 
    /// 生成玩家时对玩家进行处理        
    /// </summary> 
    /// <param name="id">玩家的id，用id == GameMgr.instance.id来判断是否是本地玩家</param>
    /// <param name="playerObj">玩家对象</param>    
    /// <returns></returns> 
    public virtual void HandlePlayer(string id, GameObject playerObj)
    {

    }

    public void RecvStartFight(ProtocolBase protocol)
    {
        StartBattle((ProtocolBytes)protocol);
        //若要游戏内的玩家不用退出至游戏大厅而是重新开始此关卡，则不应该在此取消监听
        NetMgr.srvConn.msgDist.DelListener("StartFight", RecvStartFight);
    }

    public void RecvLoading(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        //解析协议
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string player_id = proto.GetString(start, ref start);
        Debug.Log(player_id + " finish Loading");
    }
}