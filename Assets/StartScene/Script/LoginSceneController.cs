using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationTrident.StartScene
{
    public class LoginSceneController : MonoBehaviour
    {
        [SerializeField]
        private InputField idInput;

        [SerializeField]
        private InputField pwInput;

        [SerializeField]
        private Canvas regCanvas;

        [SerializeField]
        private Canvas loginCanvas;

        [SerializeField]
        private Canvas titleCanvas;

        [SerializeField]
        private Canvas roomCanvas;

        [SerializeField]
        private Canvas titleTextCanvas;

        // Use this for initialization
        void Start()
        {
            if (StartSceneEvent.startSceneState == StartSceneEvent.StartSceneState.Login)
            {
                LoginInit();
            }
            else
            {
                loginCanvas.enabled = false;
                titleCanvas.enabled = false;
                roomCanvas.enabled = false;
                titleTextCanvas.enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LoginInit()
        {
            loginCanvas.enabled = true;
            titleCanvas.enabled = false;
            roomCanvas.enabled = false;
            titleTextCanvas.enabled = true;
            regCanvas.enabled = false;
        }

        public void RegInit()
        {
            loginCanvas.enabled = false;
            titleCanvas.enabled = false;
            roomCanvas.enabled = false;
            titleTextCanvas.enabled = true;
            regCanvas.enabled = true;
        }

        public void OnLoginClick()
        {
            //用户名密码为空
            if (idInput.text == "" || pwInput.text == "")
            {
                Debug.Log("用户名，密码不能为空");
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
                Debug.Log("登陆成功");
                //开始游戏
                // TODO 打开开始游戏场景

                GameMgr.instance.id = idInput.text;
                loginCanvas.enabled = false;
                titleCanvas.enabled = true;
                roomCanvas.enabled = false;
                StartSceneEvent.startSceneState = StartSceneEvent.StartSceneState.Title;
            }
            else
            {
                Debug.Log("登陆失败");
            }
        }

        public void OnStartGameClick()
        {
            loginCanvas.enabled = false;
            titleCanvas.enabled = false;
            roomCanvas.enabled = true;
            titleTextCanvas.enabled = false;
            StartSceneEvent.startSceneState = StartSceneEvent.StartSceneState.RoomList;
            GetComponent<RoomSceneController>().InitRoomListScene();
        }

        [SerializeField]
        private InputField regIdInput;

        [SerializeField]
        private InputField regPwInput;

        [SerializeField]
        private InputField regReInput;

        public void OnRegClose()
        {
            LoginInit();
        }

        public void OnRegClick()
        {
            //用户名密码为空
            if (regIdInput.text == "" || regPwInput.text == "")
            {
                Debug.Log("用户名密码不能为空!");
                return;
            }
            //两次密码不同
            if (regPwInput.text != regReInput.text)
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
            protocol.AddString(idInput.text);
            protocol.AddString(pwInput.text);
            Debug.Log("发送 " + protocol.GetDesc());
            NetMgr.srvConn.Send(protocol, OnRegBack);
        }

        public void OnRegBack(ProtocolBase protocol)
        {
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            if (ret == 0)
            {
                Debug.Log("注册成功");
                LoginInit();
            }
            else
            {
                Debug.Log("注册失败");
            }
        }
    }
}