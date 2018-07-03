using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationTrident.StartScene
{
    public class RoomSceneController : MonoBehaviour
    {
        public struct RoomInfo
        {
            public int id;
            public int number;
            public int state;

            public RoomInfo(int id,int number,int state)
            {
                this.id = id;
                this.number = number;
                this.state = state;
            }
        }

        private List<RoomInfo> m_roomList = new List<RoomInfo>();

        [SerializeField]
        private List<GameObject> roomsPanels;

        [SerializeField]
        private Text idText;

        [SerializeField]
        private Scrollbar scrollbar;

        [SerializeField]
        private Canvas roomListCanvas;

        [SerializeField]
        private Canvas teamCanvas;

        public void Init()
        {
            //监听
            NetMgr.srvConn.msgDist.AddListener("GetAchieve", RecvGetAchieve);
            NetMgr.srvConn.msgDist.AddListener("GetRoomList", RecvGetRoomList);

            //发送查询
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetRoomList");
            NetMgr.srvConn.Send(protocol);

            protocol = new ProtocolBytes();
            protocol.AddString("GetAchieve");
            NetMgr.srvConn.Send(protocol);
        }

        // Use this for initialization
        void Start()
        {
            



        }

        // Update is called once per frame
        void Update()
        {

        }

        //收到GetAchieve协议
        public void RecvGetAchieve(ProtocolBase protocol)
        {
            //解析协议
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int win = proto.GetInt(start, ref start);
            int lost = proto.GetInt(start, ref start);
            //处理
            idText.text = "您好，" + GameMgr.instance.id;
            //winText.text = win.ToString();
            //lostText.text = lost.ToString();
        }

        public void RecvGetRoomList(ProtocolBase protocol)
        {
            m_roomList.Clear();
            //解析协议
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int count = proto.GetInt(start, ref start);
            for (int i = 0; i < count; i++)
            {
                int num = proto.GetInt(start, ref start);
                int status = proto.GetInt(start, ref start);
                m_roomList.Add(new RoomInfo(i, num, status));
            }
            scrollbar.numberOfSteps = m_roomList.Count;
            scrollbar.value = 0;
            SetRoomList();

        }

        public void SetRoomList()
        {
            //房间列表当前页的起始room id
            int roomStartId = (int)(scrollbar.value * m_roomList.Count);
            int maxStartId = m_roomList.Count - roomsPanels.Count;
            if (maxStartId < 0) maxStartId = 0;
            if (roomStartId > maxStartId)roomStartId = maxStartId;
            Debug.Log(m_roomList.Count);


            for (int i = 0; i < roomsPanels.Count; i++)
            {
                roomsPanels[i].SetActive(false);
            }
            for (int i = 0; i < Mathf.Min(m_roomList.Count,roomsPanels.Count); i++)
            {
                roomsPanels[i].SetActive(true);
                roomsPanels[i].transform.Find("roomid").GetComponent<Text>().text =
                    "房间号:" + m_roomList[roomStartId + i].id;
                roomsPanels[i].transform.Find("numbers").GetComponent<Text>().text =
                    "人数:" + m_roomList[roomStartId + i].number;
                roomsPanels[i].transform.Find("state").GetComponent<Text>().text =
                    "状态:" + (m_roomList[roomStartId + i].state==1?"准备中":"战斗中");
            }
        }

        //刷新按钮
        public void OnRefreshClick()
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetRoomList");
            NetMgr.srvConn.Send(protocol);
        }

        // 新建房间
        public void OnNewClick()
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("CreateRoom");
            NetMgr.srvConn.Send(protocol, OnNewBack);
        }

        //新建按钮返回
        public void OnNewBack(ProtocolBase protocol)
        {
            //解析参数
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            //处理
            if (ret == 0)
            {
               // PanelMgr.instance.OpenPanel<TipPanel>("", "创建成功!");
                //PanelMgr.instance.OpenPanel<RoomPanel>("");
                GameMgr.instance.isMasterClient = true;
                roomListCanvas.enabled = false;
                teamCanvas.enabled = true;
            }
            else
            {
                //PanelMgr.instance.OpenPanel<TipPanel>("", "创建房间失败！");
            }
        }
    }
}
