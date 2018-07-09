using UnityEngine;
using System.Collections;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public string id = "OffLine";

    public string nextScene = "StoryBoard2";//用于全局的状态，传入到AsynLoadingScene

    //[HideInInspector]
    public bool isMasterClient = false;//暂时用isOwner（房主）来定master_client
    //服务器信息
    public string host = "116.56.136.22";
    public int port = 8000;

    // Use this for initialization
    void Awake()
    {
        instance = this;
    }
}
