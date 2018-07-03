using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

namespace room2Battle
{
    /// <summary>
    /// 驱动文件，挂在场景的子场景
    /// </summary>
    public class room2 : MonoBehaviour {
        [SerializeField]
        protected SubsceneController subSceneController;

        void Start()
        {
            subSceneController.addSubscene("enterRoom2", "room2_enter");
            subSceneController.addSubscene("room2_powerroom", "room2_power");
            subSceneController.addSubscene("room2_battle", "room2_battle");
            subSceneController.setInitialSubScene("room2_battle");
            subSceneController.enabled = true;
        }

        void OnGUI()
        {
            if (Camera.main)
            {
                Rect re = new Rect(Camera.main.pixelWidth - 100, 0, 100, 100);
                GUIUtil.DisplayContentInGivenPosition("RTT " + NetMgr.srvConn.RTT + "ms", re);
            }
        }
    }
}
