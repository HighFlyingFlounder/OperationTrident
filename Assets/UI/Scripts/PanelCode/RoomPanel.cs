using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class RoomPanel : PanelBase
{
    private List<Transform> prefabs = new List<Transform>();
    private Button closeBtn;
    private Button startBtn;

    #region 生命周期
    /// <summary> 初始化 </summary>
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "RoomPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        //组件
        for (int i = 0; i < 6; i++)
        {
            string name = "PlayerPrefab" + i.ToString();
            Transform prefab = skinTrans.Find(name);
            prefabs.Add(prefab);
        }
        closeBtn = skinTrans.Find("CloseBtn").GetComponent<Button>();
        startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
        //按钮事件
        closeBtn.onClick.AddListener(OnCloseClick);
        startBtn.onClick.AddListener(OnStartClick);
        //监听
        NetMgr.srvConn.msgDist.AddListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDist.AddListener("EnterGame", RecvEnterGame);
        //发送查询
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("GetRoomInfo");
        NetMgr.srvConn.Send(protocol);


    }

    public override void OnClosing()
    {

        NetMgr.srvConn.msgDist.DelListener("GetRoomInfo", RecvGetRoomInfo);
        NetMgr.srvConn.msgDist.DelListener("EnterGame", RecvEnterGame);

    }


    public void RecvGetRoomInfo(ProtocolBase protocol)
    {
        //获取总数
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int count = proto.GetInt(start, ref start);
        //每个处理
        int i = 0;
        for (i = 0; i < count; i++)
        {
            string id = proto.GetString(start, ref start);
            //int team = proto.GetInt(start, ref start);
            int win = proto.GetInt(start, ref start);
            int fail = proto.GetInt(start, ref start);
            int isOwner = proto.GetInt(start, ref start);
            //信息处理
            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            string str = "名字：" + id + "\r\n";
            //str += "阵营：" + (team == 1 ? "红" : "蓝") + "\r\n";
            str += "胜利：" + win.ToString() + "   ";
            str += "失败：" + fail.ToString() + "\r\n";
            if (id == GameMgr.instance.id)
                str += "【我自己】";
            if (isOwner == 1)
                str += "【房主】";
            text.text = str;

            // if (team == 1)
            //     trans.GetComponent<Image>().color = Color.red;
            // else
            //     trans.GetComponent<Image>().color = Color.blue;
        }

        for (; i < 6; i++)
        {
            Transform trans = prefabs[i];
            Text text = trans.Find("Text").GetComponent<Text>();
            text.text = "【等待玩家】";
            trans.GetComponent<Image>().color = Color.gray;
        }
    }

    public void OnCloseClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("LeaveRoom");
        NetMgr.srvConn.Send(protocol, OnCloseBack);
    }


    public void OnCloseBack(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret == 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出成功!");
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "退出失败！");
        }
    }


    public void OnStartClick()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("StartGame");
        NetMgr.srvConn.Send(protocol, OnStartBack);
    }

    public void OnStartBack(ProtocolBase protocol)
    {
        //获取数值
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        //处理
        if (ret != 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "开始游戏失败！两队至少都需要一名玩家，只有队长可以开始战斗！");
        }
    }


    public void RecvEnterGame(ProtocolBase protocol)
    {
        SceneManager.LoadScene("SpaceBattle", LoadSceneMode.Single);
        Close();
    }

    #endregion
}