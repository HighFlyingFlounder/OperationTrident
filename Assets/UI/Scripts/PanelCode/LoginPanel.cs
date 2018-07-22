using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginPanel : PanelBase
{
    private InputField idInput;
    private InputField pwInput;
    private InputField IPInput;
    private InputField PORTInput;
    private Button loginBtn;
    private Button regBtn;
    private Button UpdateServerBtn;
    private Button SettingBtn;
    private Button ReturnBtn;
    private Text ipPortText;

    #region 生命周期
    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "LoginPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        idInput = skinTrans.Find("IDInput").GetComponent<InputField>();
        pwInput = skinTrans.Find("PWInput").GetComponent<InputField>();
        loginBtn = skinTrans.Find("LoginBtn").GetComponent<Button>();
        regBtn = skinTrans.Find("RegBtn").GetComponent<Button>();
        ipPortText = skinTrans.Find ("ipPortText").GetComponent<Text> ();
        SettingBtn = skinTrans.Find("SettingBtn").GetComponent<Button>();
        ReturnBtn = skinTrans.Find("ReturnBtn").GetComponent<Button>();

        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegClick);
        SettingBtn.onClick.AddListener(OnSettingClick);
        ReturnBtn.onClick.AddListener(OnReturnBtnClick);

        ipPortText.text = "IP:" + GameMgr.instance.host + "\n端口:" + GameMgr.instance.port;
    }
    #endregion

    public void OnReturnBtnClick()
    {
        PanelMgr.instance.OpenPanel<StartPanel>("");
        Close();
    }

    public void OnRegClick()
    {
        PanelMgr.instance.OpenPanel<RegPanel>("");
        //Close();
    }

    public void OnLoginClick()
    {
        //用户名密码为空
        if (idInput.text == "" || pwInput.text == "")
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "用户名密码不能为空!");
            return;
        }
        //PanelMgr.login_in = true;
        ////开始游戏
        //PanelMgr.instance.OpenPanel<RoomListPanel>("");
        //GameMgr.instance.id = idInput.text;
        //Close();
        //连接服务器
        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            NetMgr.srvConn.proto = new ProtocolBytes();
            if (!NetMgr.srvConn.Connect(GameMgr.instance.host, GameMgr.instance.port))
                PanelMgr.instance.OpenPanel<TipPanel>("", "连接服务器失败!");
        }
        //发送
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString(idInput.text);
        protocol.AddString(pwInput.text);
        Debug.Log("发送 " + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnLoginBack);
    }

    public void OnLoginBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "登录成功!");
            PanelMgr.login_in = true;
            //开始游戏
            PanelMgr.instance.OpenPanel<RoomListPanel>("");
            GameMgr.instance.id = idInput.text;
            Close();
        }
        else
        {
            PanelMgr.instance.OpenPanel<TipPanel>("", "登录失败，请检查用户名密码!");
        }
    }

    public void OnSettingClick()
    {
        PanelMgr.instance.ClosePanel("LoginPanel");
        PanelMgr.instance.OpenPanel<SettingPanel>("");
    }
}