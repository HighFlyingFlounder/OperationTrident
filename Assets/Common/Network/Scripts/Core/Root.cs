using UnityEngine;
using System.Collections;

public class Root : MonoBehaviour
{
    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start() //切换场景时，该object不会再调用Start了
    {
        Application.runInBackground = true;
        PanelMgr.instance.OpenPanel<LoginPanel>("");
    }

    void Update()
    {
        NetMgr.Update();
    }
}
