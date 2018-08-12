using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.UI
{
    public class GameHallUIManager : UIManager
    {
        static GameHallUIManager instance;

        public static GameHallUIManager Instance
        {
            get
            {
                return instance;
            }
        }

        protected void Awake()
        {
            if (instance == null)
            {
                instance = this as GameHallUIManager;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        [Header("UI Prefab")]
        [SerializeField]
        GameObject firstInitUIPrefab;

        public void EnterGameHall()
        {
            if (UIStack.Count == 0)
            {
                // 第一次进入
                Open(firstInitUIPrefab);
            }
            else
            {
                // 从游戏返回房间
                ShowLast();

                Debug.Log("send return room");
                ProtocolBytes protocol = new ProtocolBytes();
                protocol.AddString("ReturnRoom");
                NetMgr.srvConn.Send(protocol);
            }
        }
    }
}
