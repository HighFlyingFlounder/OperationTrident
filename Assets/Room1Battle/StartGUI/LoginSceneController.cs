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
            loginCanvas.enabled = true;
            titleCanvas.enabled = false;
            roomCanvas.enabled = false;
            titleTextCanvas.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {

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

            loginCanvas.enabled = false;
            titleCanvas.enabled = true;
            roomCanvas.enabled = false;
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
        }
    }
}