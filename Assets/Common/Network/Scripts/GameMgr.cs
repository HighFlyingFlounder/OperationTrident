using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    [HideInInspector]
    public string id = "OffLine";

    [HideInInspector]
    public string nextScene;//用于全局的状态，传入到AsynLoadingScene
    public string defaultStartScene = "OpeningVideo"; //用于设置开始游戏后第一个场景是哪个

    [HideInInspector]
    public int roomID; //房间号

    [HideInInspector]
    public int player_num = 0;//该局房间总人数
    
    [HideInInspector]
    public bool isMasterClient = false;//暂时用isOwner（房主）来定master_client
    //服务器信息
    public string host = "116.56.136.22";
    public int port = 8000;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        nextScene = defaultStartScene;
    }

    static public void ReturnRoom()
    {
        instance.nextScene = instance.defaultStartScene;
        SceneManager.LoadScene("GameHall_New", LoadSceneMode.Single);
    }
}
