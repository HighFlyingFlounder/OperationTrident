using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class GameHallUIController : UIBase
    {
        [Header("Text")]
        [SerializeField]
        Text playerNameText;

        [Header("Scroll View Content")]
        [SerializeField]
        GameObject content;

        [Header("Button")]
        [SerializeField]
        Button returnButton;
        [SerializeField]
        Button createRoomButton;
        [SerializeField]
        Button reloadButton;

        [Header("UI Prefab")]
        [SerializeField]
        GameObject roomInfoUIPrefab;
        [SerializeField]
        GameObject roomUIPrefab;

        [Header("Others")]
        [SerializeField]
        float autoUpdateRoomListTime;
        float dt;

        List<UIRoomInfo.RoomInfo> roomInfoList = new List<UIRoomInfo.RoomInfo>();

        void Start()
        {
            playerNameText.text = GameMgr.instance.id;

            returnButton.onClick.AddListener(delegate { GameHallUIManager.Instance.CloseCurrent(); });
            createRoomButton.onClick.AddListener(delegate { CreateRoom(); });
            reloadButton.onClick.AddListener(delegate { GetRoomList(); });

            GetRoomList();
            dt = autoUpdateRoomListTime;
        }

        new void Update()
        {
            dt -= Time.deltaTime;
            if (dt < 0)
            {
                dt = autoUpdateRoomListTime;
                GetRoomList();
            }
        }

        void GetRoomList()
        {
            //发送查询
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetRoomList");
            NetMgr.srvConn.Send(protocol, GetRoomListBack);
        }

        void GetRoomListBack(ProtocolBase protocol)
        {
            //解析协议
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int count = proto.GetInt(start, ref start);
            roomInfoList.Clear();
            for (int i = 0; i < count; i++)
            {
                int num = proto.GetInt(start, ref start);
                int status = proto.GetInt(start, ref start);

                roomInfoList.Add(new UIRoomInfo.RoomInfo(i + 1, status, num));
            }

            Utility.DeleteAllChildren(content);
            foreach (var roomInfo in roomInfoList)
            {
                AddRoomToRoomList(roomInfo);
            }
        }

        void CreateRoom()
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("CreateRoom");
            NetMgr.srvConn.Send(protocol, CreateRoomBack);
        }

        void CreateRoomBack(ProtocolBase protocol)
        {
            //解析参数
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            GameMgr.instance.roomID = roomInfoList.Count + 1;
            //处理
            if (ret == 0)
            {
                GetRoomList();
                GameHallUIManager.Instance.Open(roomUIPrefab);
            }
            else
            {
                Debug.Log("创建失败");
            }
        }

        void EnterRoom(int roomID)
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("EnterRoom");

            protocol.AddInt(roomID);
            NetMgr.srvConn.Send(protocol, EnterRoomBack);
        }

        void EnterRoomBack(ProtocolBase protocol)
        {
            //解析参数
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            GameMgr.instance.roomID = roomInfoList.Count;
            //处理
            if (ret == 0)
            {
                GameHallUIManager.Instance.Open(roomUIPrefab);
            }
            else
            {
                Debug.Log("进入房间失败");
            }
        }

        void AddRoomToRoomList(UIRoomInfo.RoomInfo roomInfo)
        {
            GameObject go = Instantiate(roomInfoUIPrefab, content.transform);
            go.GetComponent<UIRoomInfo>().SetRoomInfo(roomInfo);
            go.GetComponent<Button>().onClick.AddListener(delegate { EnterRoom(roomInfo.roomID - 1); });
        }
    }
}