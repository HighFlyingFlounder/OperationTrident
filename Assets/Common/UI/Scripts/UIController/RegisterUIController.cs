using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class RegisterUIController : UIBase
    {
        [Header("Input Field")]
        [SerializeField]
        InputField usernameInputField;
        [SerializeField]
        InputField passwordInputField;
        [SerializeField]
        InputField confirmationInputField;
        bool isFocused = false;

        [Header("Button")]
        [SerializeField]
        Button registerButton;
        [SerializeField]
        Button returnButton;

        void Start()
        {
            tabSelectFields.Add(usernameInputField.gameObject);
            tabSelectFields.Add(passwordInputField.gameObject);
            tabSelectFields.Add(confirmationInputField.gameObject);
            tabSelectFields.Add(registerButton.gameObject);
            tabSelectFields.Add(returnButton.gameObject);

            firstSelectField = usernameInputField.gameObject;
            SelectFirstField();

            usernameInputField.onValueChanged.AddListener(delegate { Utility.ToUpperCase(usernameInputField); });

            registerButton.onClick.AddListener(delegate { Register(); });
            returnButton.onClick.AddListener(delegate { GameHallUIManager.Instance.CloseCurrent(); });
        }
        new void Update()
        {
            base.Update();
            if (isFocused && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
            {
                Register();
            }
            isFocused = usernameInputField.isFocused || passwordInputField.isFocused || confirmationInputField.isFocused;
        }

        public void Register()
        {
            //用户名密码为空
            if (usernameInputField.text == "" || passwordInputField.text == "")
            {
                Debug.Log("用户名密码不能为空!");
                return;
            }
            //两次密码不同
            if (passwordInputField.text != confirmationInputField.text)
            {
                Debug.Log("两次输入的密码不同！");
                return;
            }
            //连接服务器
            if (NetMgr.srvConn.status != Connection.Status.Connected)
            {
                NetMgr.srvConn.proto = new ProtocolBytes();
                if (!NetMgr.srvConn.Connect(GameMgr.instance.host, GameMgr.instance.port))
                    Debug.Log("连接服务器失败!");
            }
            //发送
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Register");
            protocol.AddString(usernameInputField.text);
            protocol.AddString(passwordInputField.text);
            NetMgr.srvConn.Send(protocol, RegisterBack);
        }

        public void RegisterBack(ProtocolBase protocol)
        {
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            if (ret == 0)
            {
                Debug.Log("注册成功");
                GameHallUIManager.Instance.CloseCurrent();
            }
            else
            {
                Debug.Log("该用户名已被注册");
            }
        }
    }
}