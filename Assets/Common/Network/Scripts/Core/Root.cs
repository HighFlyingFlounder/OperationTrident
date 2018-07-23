using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Root : MonoBehaviour
{
    private int player_finishLoading = 0;
    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start() //切换场景时，该object不会再调用Start了
    {
        Application.runInBackground = true;
        if(PanelMgr.instance)//旧版本的UI系统
            PanelMgr.instance.OpenPanel<StartPanel>("");
        SceneManager.LoadScene("GameHall", LoadSceneMode.Single);

        //监听
        NetMgr.srvConn.msgDist.AddListener("FinishLoading", RecvLoading);
    }

    void Update()
    {
        NetMgr.Update();
    }

    public void RecvLoading(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        //解析协议
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string player_id = proto.GetString(start, ref start);
        Debug.Log(player_id + " finish Loading");
        player_finishLoading++;
        Debug.Log(" GameMgr.instance.player_num is " + GameMgr.instance.player_num);
        if (player_finishLoading == GameMgr.instance.player_num)//加载完成的人数等于该局游戏人数总数
        {
            AsyncLoadScene.ToNextScene = true;
            player_finishLoading = 0;
            //operation.allowSceneActivation = true;//允许异步加载完毕后自动切换场景
        }

    }

    private void OnDestroy()
    {
        NetMgr.srvConn.msgDist.DelListener("FinishLoading", RecvLoading);
    }
}
