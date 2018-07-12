using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class AsyncLoadScene : MonoBehaviour
{
    public Slider loadingSlider;

    public Text loadingText;

    private float loadingSpeed = 1;

    private float targetValue;

    private AsyncOperation operation;

    private int player_finishLoading = 0;

    private bool finished_loading = false;

    // Use this for initialization
    void Start()
    {
        loadingSlider.value = 0.0f;

        if (SceneManager.GetActiveScene().name == "Loading")
        {
            //启动协程
            Debug.Log("Loading Scene Coroutine Start!!!");
            StartCoroutine(AsyncLoading());
        }
    }

    IEnumerator AsyncLoading()
    {
        operation = SceneManager.LoadSceneAsync(GameMgr.instance.nextScene);
        //阻止当加载完成自动切换
        operation.allowSceneActivation = false;

        yield return operation;
    }

    // Update is called once per frame
    void Update()
    {
        targetValue = operation.progress;

        if (operation.progress >= 0.9f)
        {
            //operation.progress的值最大为0.9
            targetValue = 1.0f;
        }

        if (targetValue != loadingSlider.value)
        {
            //插值运算
            loadingSlider.value = Mathf.Lerp(loadingSlider.value, targetValue, Time.deltaTime * loadingSpeed);
            if (Mathf.Abs(loadingSlider.value - targetValue) < 0.01f)
            {
                loadingSlider.value = targetValue;
            }
        }

        loadingText.text = ((int)(loadingSlider.value * 100)).ToString() + "%";

        if ((int)(loadingSlider.value * 100) == 100)
        {
            if (!finished_loading)
            {
                SendFinishLoading();
                finished_loading = true;
            }
                
            //允许异步加载完毕后自动切换场景
            //operation.allowSceneActivation = true;
        }
    }

    void SendFinishLoading()
    {
        //监听
        NetMgr.srvConn.msgDist.AddListener("FinishLoading", RecvLoading);
        //协议
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("FinishLoading");
        NetMgr.srvConn.Send(protocol);
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
            operation.allowSceneActivation = true;//允许异步加载完毕后自动切换场景
        }
        
    }



    private void OnDestroy()
    {
        NetMgr.srvConn.msgDist.DelListener("FinishLoading", RecvLoading);
    }
}
