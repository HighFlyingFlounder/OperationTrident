using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class RoomUIController : UIBase
    {
        [Header("Text")]
        [SerializeField]
        Text roomIDText;
        [SerializeField]
        Text[] memberInfoTextList;

        [Header("Button")]
        [SerializeField]
        Button exitRoomButton;
        [SerializeField]
        Button startGameButton;

        bool isInRoom;

        void Start()
        {
            roomIDText.text += GameMgr.instance.roomID;
            isInRoom = true;
            NetMgr.srvConn.msgDist.AddListener("GetRoomInfo", RecvGetRoomInfo);
            NetMgr.srvConn.msgDist.AddListener("EnterGame", RecvEnterGame);

            exitRoomButton.onClick.AddListener(delegate { ExitRoom(); });
            startGameButton.onClick.AddListener(delegate { StartGame(); });

            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetRoomInfo");
            NetMgr.srvConn.Send(protocol);
        }

        void OnDestroy()
        {
            NetMgr.srvConn.msgDist.DelListener("GetRoomInfo", RecvGetRoomInfo);
            NetMgr.srvConn.msgDist.DelListener("EnterGame", RecvEnterGame);
            if (isInRoom)
                ExitRoom();
        }

        void RecvGetRoomInfo(ProtocolBase protocol)
        {
            //获取总数
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int count = proto.GetInt(start, ref start);

            int i;
            for (i = 0; i < count; i++)
            {
                string playerName = proto.GetString(start, ref start);
                int win = proto.GetInt(start, ref start);
                int fail = proto.GetInt(start, ref start);
                bool isOwner = proto.GetInt(start, ref start) == 1;
                bool isMe = playerName == GameMgr.instance.id;

                if (isMe)
                    GameMgr.instance.isMasterClient = isOwner;

                memberInfoTextList[i].GetComponent<UIMemberInfo>().SetMemberInfo(playerName, isOwner, isMe);
            }

            for (; i < memberInfoTextList.Length; i++)
            {
                memberInfoTextList[i].GetComponent<UIMemberInfo>().ResetMemberInfo();
            }

            if (GameMgr.instance.isMasterClient)
                Utility.EnableButton(startGameButton);
            else
                Utility.DisableButton(startGameButton);
        }

        void RecvEnterGame(ProtocolBase protocol)
        {
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int player_num = proto.GetInt(start, ref start);

            GameMgr.instance.player_num = player_num;//该局房间总人数
            StartFight();//获得战场信息
        }

        void StartFight()
        {
            //协议
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("StartFight");
            NetMgr.srvConn.Send(protocol);
            //监听
            NetMgr.srvConn.msgDist.AddListener("StartFight", RecvStartFight);
        }

        void RecvStartFight(ProtocolBase protocol)
        {
            SceneNetManager.fight_protocol = (ProtocolBytes)protocol;
            //若要游戏内的玩家不用退出至游戏大厅而是重新开始此关卡，则不应该在此取消监听
            NetMgr.srvConn.msgDist.DelListener("StartFight", RecvStartFight);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
        }

        void ExitRoom()
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("LeaveRoom");
            NetMgr.srvConn.Send(protocol, ExitRoomBack);
        }

        void ExitRoomBack(ProtocolBase protocol)
        {
            //获取数值
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            //处理
            if (ret == 0)
            {
                GameHallUIManager.Instance.CloseCurrent();
                isInRoom = false;
            }
        }

        void StartGame()
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("StartGame");
            NetMgr.srvConn.Send(protocol, StartGameBack);
        }

        void StartGameBack(ProtocolBase protocol)
        {
        }
    }
}