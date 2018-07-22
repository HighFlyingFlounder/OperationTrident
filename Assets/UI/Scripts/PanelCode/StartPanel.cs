using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartPanel : PanelBase
{
    private Button SingleBtn;
    private Button MultiplayerBtn;
    private Button ExitBtn;

    #region 生命周期
    //初始化
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "StartPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;

        SingleBtn = skinTrans.Find("SingleBtn").GetComponent<Button>();
        MultiplayerBtn = skinTrans.Find("MultiplayerBtn").GetComponent<Button>();
        ExitBtn = skinTrans.Find("ExitBtn").GetComponent<Button>();

        SingleBtn.onClick.AddListener(OnSingleBtnClick);
        MultiplayerBtn.onClick.AddListener(OnMultiplayerBtnClick);
        ExitBtn.onClick.AddListener(OnExitBtnClick);

    }
    #endregion



    public void OnSingleBtnClick()
    {
        //单人游戏
        //AsyncLoadScene.SingleMode = true;
        //SceneManager.LoadScene("Loading",LoadSceneMode.Single);
        //Close();
        PanelMgr.instance.OpenPanel<TipPanel>("", "单人模式还未完成！");
    }

    public void OnMultiplayerBtnClick()
    {
        PanelMgr.instance.OpenPanel<LoginPanel>("");
        Close();
    }

    public void OnExitBtnClick()
    {
        Application.Quit();
    }
}