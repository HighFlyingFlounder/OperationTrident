using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterNextScene : MonoBehaviour {
    private bool arrived = false;
    public string nextScene = "PlayerTest2";
	// Use this for initialization
	void Start () {
        NetMgr.srvConn.msgDist.AddListener("Result", RecvResult);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (!arrived)
            {
                if(other.gameObject.name == GameMgr.instance.id)//是本地玩家到达
                {
                    SendSpaceArriveEnd();
                    arrived = true;
                }
            }
        }
    }

    public void SendSpaceArriveEnd()
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("SpaceArriveEnd");
        //位置和旋转
        NetMgr.srvConn.Send(proto);
    }

    public void RecvResult(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        int isWin = proto.GetInt(start, ref start);
        //弹出胜负面板
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //取消监听
        NetMgr.srvConn.msgDist.DelListener("Result", RecvResult);

        if (isWin == 0)//失败
        {
            Debug.Log("Room1 Fail");
            OperationTrident.EventSystem.Messenger.Broadcast(OperationTrident.Room1.DieHandler.PLAYER_DIE);
        }
        else//胜利
        {
            //某关卡胜利是直接进入下一个场景，故不会进入这里
            Debug.Log("Room1 胜利！");
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
    }
}
