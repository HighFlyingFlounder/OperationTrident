using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class MainUIController : UIBase
    {
        [Header("Button")]
        [SerializeField]
        Button startButton;
        [SerializeField]
        Button helpButton;
        [SerializeField]
        Button settingButton;
        [SerializeField]
        Button rewardButton;
        [SerializeField]
        Button logoutButton;

        [Header("UI Prefab")]
        [SerializeField]
        GameObject gameHallUIPrefab;

        void Start()
        {
            tabSelectFields.Add(startButton.gameObject);
            tabSelectFields.Add(helpButton.gameObject);
            tabSelectFields.Add(settingButton.gameObject);
            tabSelectFields.Add(rewardButton.gameObject);
            tabSelectFields.Add(logoutButton.gameObject);

            firstSelectField = startButton.gameObject;

            startButton.onClick.AddListener(delegate { GameHallUIManager.Instance.Open(gameHallUIPrefab); });
            logoutButton.onClick.AddListener(delegate { Logout(); });

            Utility.DisableButton(helpButton);
            Utility.DisableButton(settingButton);
            Utility.DisableButton(rewardButton);
        }

        protected override void FirstInit()
        {
            SelectFirstField();
        }

        void Logout()
        {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Logout");
            NetMgr.srvConn.Send(protocol, LogoutBack);
        }

        void LogoutBack(ProtocolBase protocol)
        {
            GameHallUIManager.Instance.CloseCurrent();
            Debug.Log("注销成功");
            NetMgr.srvConn.Close();
        }
    }
}