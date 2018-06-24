using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBattleManager : MonoBehaviour
{
    //单例
    public static NetWorkManager instance;
    //玩家预设
    public GameObject[] PlayerPrefabs;
    //游戏中给所有的角色
    public Dictionary<string, GameObject> list;
    public Dictionary<string, Hinder> rock_list;
    private GameObject[] rocks;
    void Awake()
    {
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
        if (GameObject.FindGameObjectWithTag("flyer"))
            Destroy(GameObject.FindGameObjectWithTag("flyer"));
        StartGame();
    }

    //开始一场游戏的准备工作
    void StartGame()
    {
        list = new Dictionary<string, GameObject>();
        rock_list = new Dictionary<string, Hinder>();
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
        rock_list.Clear();
    }

    //开始战斗
    public void StartBattle(ProtocolBytes proto)
    {
        //解析协议
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        if (protoName != "StartFight")
            return;
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
        NetMgr.srvConn.msgDist.AddListener("HitRock", RecvHitRock);
        NetMgr.srvConn.msgDist.AddListener("Result", RecvResult);
        //获得所有陨石的句柄
        rocks = GameObject.FindGameObjectsWithTag("Hinder");
        foreach (GameObject rock in rocks)
        {
            rock_list.Add(rock.name, rock.GetComponent<Hinder>());
            rock.SetActive(true);
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
            playerObj.GetComponent<NetSyncTransform>().ctrlType = NetSyncTransform.CtrlType.player;
        }
        else
        {
            playerObj.GetComponent<NetSyncTransform>().ctrlType = NetSyncTransform.CtrlType.net;
            playerObj.transform.Find("Camera").gameObject.GetComponent<Camera>().enabled = false;
            playerObj.transform.Find("Camera/sand_effect").gameObject.SetActive(false);
        }
    }

    public void RecvHitRock(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        string rock_name = proto.GetString(start, ref start);

        if (id == GameMgr.instance.id)
            return;
        Hinder ht = rock_list[rock_name];
        ht.Boom();
    }

    public void RecvResult(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        int isWin = proto.GetInt(start, ref start);
        //弹出胜负面板
        PanelMgr.instance.OpenPanel<WinPanel>("", isWin);
        Debug.Log("GameMgr.instance.id: " + GameMgr.instance.id);
        //玩家收到胜利条件后禁用掉list中的玩家
        GameObject flyer;
        Debug.Log("list.TryGetValue : " + list.TryGetValue(GameMgr.instance.id, out flyer));
        if (list.TryGetValue(GameMgr.instance.id, out flyer))
            flyer.SetActive(false);
        //list[GameMgr.instance.id].gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //取消监听
        NetMgr.srvConn.msgDist.DelListener("HitRock", RecvHitRock);
        NetMgr.srvConn.msgDist.DelListener("Result", RecvResult);
        NetMgr.srvConn.msgDist.DelListener("FinishLoading", RecvLoading);
        ClearBattle();
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