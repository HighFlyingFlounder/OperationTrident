using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.CrossInput;
using OperationTrident.Player;

public class PlayerTestManager : MonoBehaviour
{
    //全局NetWorkManager单例
    public static NetWorkManager instance;
    //玩家预设
    public GameObject[] PlayerPrefabs;
    //游戏中给所有的角色
    public Dictionary<string, GameObject> list;
    // Use this for initialization
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }
    //开始一场游戏的准备工作
    void StartGame()
    {
        list = new Dictionary<string, GameObject>();
        //协议
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("FinishLoading");
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
        //玩家处理

        if (id == GameMgr.instance.id)
        {
            playerObj.transform.Find("Camera").gameObject.SetActive(true);
            playerObj.GetComponent<InputManager>().IsLocalPlayer = true;
        }
        else
        {
            NetSyncTransform netsyn = playerObj.GetComponent<NetSyncTransform>();
            netsyn.ctrlType = NetSyncTransform.CtrlType.net;
            playerObj.GetComponent<InputManager>().IsLocalPlayer = false;
            playerObj.GetComponent<PlayerController>().enabled = false;
        }
    }

    public void RecvStartFight(ProtocolBase protocol)
    {
        StartBattle((ProtocolBytes)protocol);
        //若要游戏内的玩家不用退出至游戏大厅而是重新开始此关卡，则不应该在此取消监听
        NetMgr.srvConn.msgDist.DelListener("StartFight", RecvStartFight);
    }
}