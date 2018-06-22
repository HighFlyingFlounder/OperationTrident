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
        if(!arrived)
          SendSpaceArriveEnd();
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
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);

        //取消监听
        NetMgr.srvConn.msgDist.DelListener("Result", RecvResult);
    }
}
