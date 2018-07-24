using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : PanelBase {
    private InputField IPInput;
    private InputField PORTInput;
    private Button UpdateServerBtn;
    private Button ReturnBtn;
    private Text ipPortText;

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
        ipPortText = skinTrans.Find ("ipPortText").GetComponent<Text> ();

        UpdateServerBtn.onClick.AddListener (OnUpdateServerClick);
        ReturnBtn.onClick.AddListener (OnReturnBtn);

        ipPortText.text = "IP:" + GameMgr.instance.host + "\n端口:" + GameMgr.instance.port;
    }
    #endregion

    public void OnUpdateServerClick () {
        PanelMgr.instance.OpenPanel<TipPanel> ("", "修改成功!");
        GameMgr.instance.host = IPInput.text;
        GameMgr.instance.port = int.Parse (PORTInput.text);
        ipPortText.text = "IP:" + GameMgr.instance.host + "\n端口:" + GameMgr.instance.port;
        PanelMgr.instance.ClosePanel ("SettingPanel");
        PanelMgr.instance.OpenPanel<LoginPanel> ("");
    }

    public void OnReturnBtn () {
        PanelMgr.instance.ClosePanel ("SettingPanel");
        PanelMgr.instance.OpenPanel<LoginPanel> ("");
    }
}