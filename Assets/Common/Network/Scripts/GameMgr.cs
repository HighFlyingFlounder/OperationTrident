using UnityEngine;
using System.Collections;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public string id = "Tank";

    //服务器信息
    public static string host = "127.0.0.1";
    public static int port = 1234;

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }
}
