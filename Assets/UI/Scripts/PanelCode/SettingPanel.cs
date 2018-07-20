using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : PanelBase {
    private InputField IPInput;
    private InputField PORTInput;
    private Button UpdateServerBtn;
    private Button ReturnBtn;

    #region 生命周期
    //初始化
    public override void Init (params object[] args) {
        base.Init (args);
        skinPath = "SettingPanel";
        layer = PanelLayer.Panel;
    }

    public override void OnShowing () {
        base.OnShowing ();
        Transform skinTrans = skin.transform;

        IPInput = skinTrans.Find ("IPInput").GetComponent<InputField> ();
        PORTInput = skinTrans.Find ("PORTInput").GetComponent<InputField> ();
        UpdateServerBtn = skinTrans.Find ("UpdateServerBtn").GetComponent<Button> ();
        ReturnBtn = skinTrans.Find ("ReturnBtn").GetComponent<Button> ();

        UpdateServerBtn.onClick.AddListener (OnUpdateServerClick);
        ReturnBtn.onClick.AddListener (OnReturnBtn);
    }
    #endregion

    public void OnUpdateServerClick () {
        PanelMgr.instance.OpenPanel<TipPanel> ("", "修改成功!");
        GameMgr.instance.host = IPInput.text;
        GameMgr.instance.port = int.Parse (PORTInput.text);
        PanelMgr.instance.ClosePanel ("SettingPanel");
        PanelMgr.instance.OpenPanel<LoginPanel> ("");
    }

    public void OnReturnBtn () {
        PanelMgr.instance.ClosePanel ("SettingPanel");
        PanelMgr.instance.OpenPanel<LoginPanel> ("");
    }
}