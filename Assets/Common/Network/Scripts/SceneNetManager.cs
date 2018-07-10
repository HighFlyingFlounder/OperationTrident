using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneNetManager : MonoBehaviour
{
    public static ProtocolBytes fight_protocol;
    public static SceneNetManager instance;
    //本地玩家实体预设
    public GameObject[] LocalPlayerPrefabs;
    //其他玩家实体预设
    public GameObject[] NetPlayerPrefabs;
    //游戏中给所有的角色
    public Dictionary<string, GameObject> list;

    public virtual void Awake()
    {
        instance = this;
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
        StartBattle(fight_protocol);
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
        //产生玩家角色
        GameObject playerObj;
        if (id == GameMgr.instance.id)//本地玩家
        {
            playerObj = Instantiate(LocalPlayerPrefabs[0]);
        }
        else
        {
            playerObj = Instantiate(NetPlayerPrefabs[0]);
        }
        //playerObj所有子节点都得改名
        NetSyncController[] net_sync_controllers = playerObj.GetComponentsInChildren<NetSyncController>();
        foreach (var temp in net_sync_controllers)
        {
            temp.name = id + temp.name;
            temp.setSyncID(temp.name);//确保sync_id正确
        }
        playerObj.name = id;
        playerObj.GetComponent<NetSyncController>().setSyncID(playerObj.name);//确保sync_id正确

        playerObj.transform.position = swopTrans.position;
        playerObj.transform.rotation = swopTrans.rotation;

        list.Add(id, playerObj);
        //玩家通用处理
        NetSyncTransform netsyn = playerObj.GetComponent<NetSyncTransform>();
        if (id == GameMgr.instance.id)
        {
            netsyn.ctrlType = NetSyncTransform.CtrlType.player;//CtrlType默认为none，none不发送消息，模拟单人模式
        }
        else
        {

            netsyn.ctrlType = NetSyncTransform.CtrlType.net;
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


}