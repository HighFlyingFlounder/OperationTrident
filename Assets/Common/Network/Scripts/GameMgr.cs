using UnityEngine;
using System.Collections;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public string id = "OffLine";

    public string startScene = "SpaceBattle";

    [HideInInspector]
    public bool isMasterClient = false;//暂时用isOwner（房主）来定master_client
    //服务器信息
    public string host = "127.0.0.1";
    public int port = 1234;

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }
}
