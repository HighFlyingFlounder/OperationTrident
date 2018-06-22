using UnityEngine;
using System.Collections;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public string id = "PlayerID";

    public string startScene = "SpaceBattle";

    //服务器信息
    public string host = "127.0.0.1";
    public int port = 1234;

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }
}
