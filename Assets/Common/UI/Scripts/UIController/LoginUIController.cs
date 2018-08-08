using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class LoginUIController : UIBase
    {
        [Header("Input Field")]
        [SerializeField]
        InputField usernameInputField;
        [SerializeField]
        InputField passwordInputField;
        bool isFocused = false;

        [Header("Button")]
        [SerializeField]
        Button loginButton;
        [SerializeField]
        Button registerButton;
        [SerializeField]
        Button settingButton;
        [SerializeField]
        Button exitButton;

        [Header("UI Prefab")]
        [SerializeField]
        GameObject mainUIPrefab;
        [SerializeField]
        GameObject settingUIPrefab;
        [SerializeField]
        GameObject registerUIPrefab;

        void Start()
        {
            tabSelectFields.Add(usernameInputField.gameObject);
            tabSelectFields.Add(passwordInputField.gameObject);
            tabSelectFields.Add(loginButton.gameObject);
            tabSelectFields.Add(registerButton.gameObject);
            tabSelectFields.Add(settingButton.gameObject);
            tabSelectFields.Add(exitButton.gameObject);

            firstSelectField = usernameInputField.gameObject;
            SelectFirstField();

            usernameInputField.onValueChanged.AddListener(delegate { Utility.ToUpperCase(usernameInputField); });
            // usernameInputField.onEndEdit.AddListener(delegate { Login(); });
            // passwordInputField.onEndEdit.AddListener(delegate { Login(); });

            loginButton.onClick.AddListener(delegate { Login(); });
            registerButton.onClick.AddListener(delegate { GameHallUIManager.Instance.Open(registerUIPrefab); });
            settingButton.onClick.AddListener(delegate { GameHallUIManager.Instance.Open(settingUIPrefab); });
            exitButton.onClick.AddListener(delegate { Utility.ExitGame(); });
        }

        new void Update()
        {
            base.Update();
            if (isFocused && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
            {
                Login();
            }
            isFocused = usernameInputField.isFocused || passwordInputField.isFocused;
        }

        void Login()
        {
            //用户名密码为空
            if (usernameInputField.text == "" || passwordInputField.text == "")
            {
                Debug.Log("用户名/密码不能为空");
                return;
            }
            //连接服务器
            if (NetMgr.srvConn.status != Connection.Status.Connected)
            {
                NetMgr.srvConn.proto = new ProtocolBytes();
                if (!NetMgr.srvConn.Connect(GameMgr.instance.host, GameMgr.instance.port))
                    Debug.Log("连接服务器失败");
            }
            //发送
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Login");
            protocol.AddString(usernameInputField.text);
            protocol.AddString(passwordInputField.text);

            NetMgr.srvConn.Send(protocol, LoginBack);
        }

        void LoginBack(ProtocolBase protocol)
        {
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            if (ret == 0)
            {
                Debug.Log("登陆成功");
                // 开始游戏
                // TODO 打开开始游戏场景
                GameMgr.instance.id = usernameInputField.text;
                GameHallUIManager.Instance.Open(mainUIPrefab);
            }
            else
            {
                Debug.Log("用户名/密码错误");
            }
        }
    }
}