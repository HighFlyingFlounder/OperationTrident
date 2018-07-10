using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OperationTrident.Util;

public class EnterNextScene : MonoBehaviour {
    private bool arrived = false;

    private Camera m_FadeOutGuiCamera;

    public string nextScene = "PlayerTest2";

    public void SetGuiCamera(Camera cam)
    {
        m_FadeOutGuiCamera = cam;
    }

	// Use this for initialization
	private void Start () {
        NetMgr.srvConn.msgDist.AddListener("Result", RecvResult);
        m_FadeOutGuiCamera = null;
    }

    private void Update()
    {
        FadeInOutUtil.UpdateState();
    }

    private void OnGUI()
    {
        FadeInOutUtil.RenderGUI();
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
            //淡出
            float fadeOutDuration = 3.0f;
            FadeInOutUtil.SetFadingState(fadeOutDuration,m_FadeOutGuiCamera, Color.black, FadeInOutUtil.FADING_STATE.FADING_OUT);
            StartCoroutine(FadeOutCoroutine(fadeOutDuration));//等多一下确定淡出完成？
        }
    }

    private IEnumerator FadeOutCoroutine(float sec)
    {
        yield return new WaitForSeconds(sec);
        GameMgr.instance.nextScene = nextScene;
        SceneManager.LoadScene("Loading", LoadSceneMode.Single);
        //SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }

}
