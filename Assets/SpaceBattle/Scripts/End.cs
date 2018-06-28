using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    private bool arrived = false;
    public int total_player = 4;//默认是四个玩家
    private int finished_player = 0;
    private bool isWin = false;
    private void Start()
    {
        finished_player = 0;
        NetMgr.srvConn.msgDist.AddListener("SpaceArriveEnd", RecvSpaceArriveEnd);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.transform.parent.GetComponent<NetSyncTransform>().ctrlType == NetSyncTransform.CtrlType.player)
        {
            if (!arrived)
            {
                finished_player++;
                SendSpaceArriveEnd();
                arrived = true;
            }
        }
    }

    private void Update()
    {
        if (finished_player == total_player && !isWin)
        {
            //更新四个玩家是否到达的信息
        }
    }

    public void RecvSpaceArriveEnd(ProtocolBase protocol)
    {
        //解析协议
        int start = 0;
        ProtocolBytes proto = (ProtocolBytes)protocol;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        finished_player = proto.GetInt(start, ref start);
        //处理
        Debug.Log("RecvSpaceArriveEnd " + id);
    }

    public void SendSpaceArriveEnd()
    {
        ProtocolBytes proto = new ProtocolBytes();
        proto.AddString("SpaceArriveEnd");
        //位置和旋转
        NetMgr.srvConn.Send(proto);
    }

    private void OnDestroy()
    {

    }
}
