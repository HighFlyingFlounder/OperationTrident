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

        public struct MemberInfo
        {
            public string id;
            public int isOwner;
            public bool isMe;
            public MemberInfo(string id,int isOwner,bool isMe)
            {
                this.id = id;
                this.isOwner = isOwner;
                this.isMe = isMe;
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

        private List<MemberInfo> m_memberList = new List<MemberInfo>();

        [SerializeField]
        private List<GameObject> membersImages;

        [SerializeField]
        private GameObject starPrebab;

        [SerializeField]
        private Text welcomeText;

        public void InitRoomListScene()
        {
            Debug.Log("Init Room List Scene");
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

        void OnDisable()
        {
            Debug.Log("RoomList Disable");
            NetMgr.srvConn.msgDist.DelListener("GetAchieve", RecvGetAchieve);
            NetMgr.srvConn.msgDist.DelListener("GetRoomList", RecvGetRoomList);
            NetMgr.srvConn.msgDist.DelListener("GetRoomInfo", RecvGetRoomInfo);
            NetMgr.srvConn.msgDist.DelListener("EnterGame", RecvEnterGame);
        }

        private void OnDestroy()
        {
            Debug.Log("RoomList Destroy");
        }

        public void InitTeamScene()
        {
            NetMgr.srvConn.msgDist.AddListener("GetRoomInfo", RecvGetRoomInfo);
            NetMgr.srvConn.msgDist.AddListener("EnterGame", RecvEnterGame);
            //发送查询
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetRoomInfo");
            NetMgr.srvConn.Send(protocol);
        }

        public void RecvGetRoomInfo(ProtocolBase protocol)
        {
            m_memberList.Clear();
            //获取总数
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int count = proto.GetInt(start, ref start);
            Debug.Log("Count: " + count);
            //每个处理
            int i = 0;
            for (i = 0; i < count; i++)
            {
                Debug.Log("获取每个成员信息" + i);
                string id = proto.GetString(start, ref start);
                //int team = proto.GetInt(start, ref start);
                int win = proto.GetInt(start, ref start);
                int fail = proto.GetInt(start, ref start);
                int isOwner = proto.GetInt(start, ref start);
                Debug.Log("是房主吗?" + isOwner);
                if (isOwner == 1)
                {
                    NetSyncController.isMasterClient = true;
                }
                m_memberList.Add(new MemberInfo(id, isOwner, id == GameMgr.instance.id));
            }
            SetTeamMemberList();
        }

        private void SetTeamMemberList()
        {
            Debug.Log("设置房间信息");
            const int maxMemeberCount = 6;
            for(int i = 0; i < maxMemeberCount; i++)
            {
                membersImages[i].transform.Find("Text").GetComponent<Text>().text = string.Empty;
            }
            for(int i=0;i< Mathf.Min(m_memberList.Count,maxMemeberCount);++i)
            {
                membersImages[i].SetActive(true);
                membersImages[i].transform.Find("Text").GetComponent<Text>().text = m_memberList[i].id;
                if (m_memberList[i].isOwner == 1)
                {
                    Debug.Log("准备显示星星");
                    GameObject gameObject = Instantiate(starPrebab);
                    gameObject.transform.parent = membersImages[i].transform;
                    gameObject.GetComponent<RectTransform>().
                gameObject.GetComponent<RectTransform>().position = new Vector3(
                    membersImages[i].transform.position.x - 20.0f - membersImages[i].GetComponent<RectTransform>().rect.width/2,
                    membersImages[i].transform.position.y,
                    membersImages[i].transform.position.z
                    );
                }
                if (m_memberList[i].isMe)
                {
                    membersImages[i].GetComponent<Image>().color = new Color(0.7f,0.5f,1.0f,0.3f);
                    welcomeText.text = "您好"+m_memberList[i].id+"\n\n请召集您的队友";
                }
            }
        }

        public void RecvEnterGame(ProtocolBase protocol)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(GameMgr.instance.startScene, 
                UnityEngine.SceneManagement.LoadSceneMode.Single);
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
                //PanelMgr.instance.OpenPanel<TipPanel>("", "开始游戏失败！两队至少都需要一名玩家，只有队长可以开始战斗！");
                Debug.Log("只有队长能开始比赛啊丢");
            }
        }

        // Use this for initialization
        void Start()
        {
            roomListCanvas.enabled = false;
            teamCanvas.enabled = false;
            if(StartSceneEvent.startSceneState != StartSceneEvent.StartSceneState.Login)
            {
                roomListCanvas.enabled = false;
                teamCanvas.enabled = true;
                InitTeamScene();
            }
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
            Debug.Log("wtf");
            scrollbar.value = 0;
            SetRoomList();

        }

        public void OnScrollBarValueChanged()
        {
            SetRoomList();
        }

        private void SetRoomList()
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
                string to = (i+roomStartId).ToString();
                roomsPanels[i].transform.Find("joinin").GetComponent<Button>().onClick.RemoveAllListeners();
                roomsPanels[i].transform.Find("joinin").GetComponent<Button>().onClick.AddListener(
                    delegate() {
                        Debug.Log("invoke");
                        OnJoinBtnClick(to);
                        //roomsPanels[i].transform.Find("joinin").GetComponent<Button>().onClick.RemoveAllListeners();
                });
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
            Debug.Log("On New Click");
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("CreateRoom");
            NetMgr.srvConn.Send(protocol, OnNewBack);
        }

        //新建按钮返回
        public void OnNewBack(ProtocolBase protocol)
        {
            Debug.Log("On New Back");
            //解析参数
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            //处理
            if (ret == 0)
            {
                Debug.Log("创建成功");
                // PanelMgr.instance.OpenPanel<TipPanel>("", "创建成功!");
                //PanelMgr.instance.OpenPanel<RoomPanel>("");
                GameMgr.instance.isMasterClient = true;
                roomListCanvas.enabled = false;
                teamCanvas.enabled = true;
                InitTeamScene();
            }
            else
            {
                Debug.Log("fail to create");
                //PanelMgr.instance.OpenPanel<TipPanel>("", "创建房间失败！");
            }
        }

        //加入按钮
        public void OnJoinBtnClick(string name)
        {
            Debug.Log(name);
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("EnterRoom");

            protocol.AddInt(int.Parse(name));
            NetMgr.srvConn.Send(protocol, OnJoinBtnBack);
            Debug.Log("请求进入房间 " + name);
        }

        //加入按钮返回
        public void OnJoinBtnBack(ProtocolBase protocol)
        {
            //解析参数
            ProtocolBytes proto = (ProtocolBytes)protocol;
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            //处理
            if (ret == 0)
            {
                roomListCanvas.enabled = false;
                teamCanvas.enabled = true;
                Debug.Log("1110");
                InitTeamScene();
            }
            else
            {
                Debug.Log("进入房间失败");
            }
        }
    }
}
