using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHallManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (PanelMgr.instance)//旧版本的UI系统
            if(PanelMgr.login_in)
                PanelMgr.instance.OpenPanel<RoomListPanel>("");
            else PanelMgr.instance.OpenPanel<StartPanel>("");

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
