using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitlePanel : PanelBase
{

    private Button startBtn;
    private Button infoBtn;

    #region 生命周期
    public override void Init(params object[] args)
    {
        base.Init(args);
        skinPath = "TitlePanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing()
    {
        base.OnShowing();
        Transform skinTrans = skin.transform;
        startBtn = skinTrans.Find("StartBtn").GetComponent<Button>();
        infoBtn = skinTrans.Find("InfoBtn").GetComponent<Button>();

        startBtn.onClick.AddListener(OnStartClick);
        infoBtn.onClick.AddListener(OnInfoClick);
    }
    #endregion


    public void OnStartClick()
    {
        //设置
        PanelMgr.instance.OpenPanel<OptionPanel>("");
    }

    public void OnInfoClick()
    {
        PanelMgr.instance.OpenPanel<InfoPanel>("");
    }
}