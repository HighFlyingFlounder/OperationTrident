﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiBattle : MonoBehaviour
{
    //单例
    public static MultiBattle instance;
    //玩家预设
    public GameObject[] PlayerPrefabs;
    //游戏中给所有的角色
    public Dictionary<string, UserController> list = new Dictionary<string, UserController>();
    public Dictionary<string, Hinder> rock_list = new Dictionary<string, Hinder>();

    private GameObject[]rocks;

    // Use this for initialization
    void Start()
    {
        //单例模式
        instance = this;
        //获得所有陨石的句柄
        rocks = GameObject.FindGameObjectsWithTag("Hinder");
        foreach (GameObject rock in rocks)
            rock_list.Add(rock.name, rock.GetComponent<Hinder>());
    }

    //清理场景
     public void ClearBattle()
    {
        list.Clear();
        GameObject[] flyers = GameObject.FindGameObjectsWithTag("flyer");
        for (int i = 0; i < flyers.Length; i++)
            Destroy(flyers[i]);
    }

    //开始战斗
    public void StartBattle(ProtocolBytes proto)
    {
        //解析协议
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        if (protoName != "Fight")
            return;
        //坦克总数
        int count = proto.GetInt(start, ref start);
        //清理场景
        ClearBattle();
        //每一个玩家
        for (int i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            int swopID = proto.GetInt(start, ref start);
            GeneratePlayer(id, swopID);
        }
        foreach (GameObject rock in rocks)
            rock.SetActive(true);
        NetMgr.srvConn.msgDist.AddListener("UpdateUnitInfo", RecvUpdateUnitInfo);
        NetMgr.srvConn.msgDist.AddListener("HitRock", RecvHitRock);
        NetMgr.srvConn.msgDist.AddListener("Result", RecvResult);
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
        //预设
        // if (tankPrefabs.Length < 2)
        // {
        //     Debug.LogError("坦克预设数量不够");
        //     return;
        // }
        //产生玩家角色 0暂时代表只有一种玩家prefab
        GameObject playerObj = (GameObject)Instantiate(PlayerPrefabs[0]);
        playerObj.name = id;
        playerObj.transform.position = swopTrans.position;
        playerObj.transform.rotation = swopTrans.rotation;
        //列表处理

        UserController bt = new UserController();
        bt = playerObj.GetComponent<UserController>();

        //加到List中的是各个玩家的UserController脚本

        list.Add(id, bt);
        //玩家处理

        if (id == GameMgr.instance.id)
        {
            bt.ctrlType = UserController.CtrlType.player;
            //CameraFollow cf = Camera.main.gameObject.GetComponent<CameraFollow>();
            //GameObject target = bt.gameObject;
            //cf.SetTarget(target);
        }
        else
        {
            bt.ctrlType = UserController.CtrlType.net;
            playerObj.transform.Find("Camera").gameObject.GetComponent<Camera>().enabled = false;
            playerObj.transform.Find("Camera/sand_effect").gameObject.SetActive(false);
            //bt.InitNetCtrl();  //初始化网络同步
        }
    }


    public void RecvUpdateUnitInfo(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        Vector3 nPos;
        Vector3 nRot;
        nPos.x = proto.GetFloat(start, ref start);
        nPos.y = proto.GetFloat(start, ref start);
        nPos.z = proto.GetFloat(start, ref start);
        nRot.x = proto.GetFloat(start, ref start);
        nRot.y = proto.GetFloat(start, ref start);
        nRot.z = proto.GetFloat(start, ref start);

        int isRun = proto.GetInt(start, ref start);
        int isPushed = proto.GetInt(start, ref start);
        //处理
        Debug.Log("RecvUpdateUnitInfo " + id);
        if (!list.ContainsKey(id))
        {
            Debug.Log("RecvUpdateUnitInfo bt == null ");
            return;
        }
        UserController bt = list[id];
        if (id == GameMgr.instance.id)
            return;

        bt.NetUpdateUnitInfo(nPos, nRot, isRun, isPushed);
        // bt.tank.NetForecastInfo(nPos, nRot);
        // bt.tank.NetTurretTarget(turretY, gunX); //稍后实现
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
        list[GameMgr.instance.id].gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //取消监听
        NetMgr.srvConn.msgDist.DelListener("UpdateUnitInfo", RecvUpdateUnitInfo);
        NetMgr.srvConn.msgDist.DelListener("HitRock", RecvHitRock);
        NetMgr.srvConn.msgDist.AddListener("Result", RecvResult);
    }
}


