using UnityEngine;
using System.Collections;

public class Root : MonoBehaviour {
    private static bool created = false;
    public static Root instance = null;
    // Use this for initialization
    void Awake()
    {
        if(instance != null)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
        instance = this;
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
    }
    void Start () //切换场景时，该object不会再调用Start了
    {
        Application.runInBackground = true;
        PanelMgr.instance.OpenPanel<LoginPanel>("");
    }

    void Update()
    {
        NetMgr.Update();
    }
}
