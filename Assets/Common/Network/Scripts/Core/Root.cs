using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
        if(PanelMgr.instance)//旧版本的UI系统
            PanelMgr.instance.OpenPanel<LoginPanel>("");
        SceneManager.LoadScene("GameHall", LoadSceneMode.Single);
    }

    void Update()
    {
        NetMgr.Update();
    }
}
