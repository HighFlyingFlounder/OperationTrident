using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class ServerSettingUIController : UIBase
    {
        [Header("Input Field")]
        [SerializeField]
        InputField ipInputField_1;
        [SerializeField]
        InputField ipInputField_2;
        [SerializeField]
        InputField ipInputField_3;
        [SerializeField]
        InputField ipInputField_4;
        [SerializeField]
        InputField portInputField;

        [Header("Button")]
        [SerializeField]
        Button resetButton;
        [SerializeField]
        Button saveButton;
        [SerializeField]
        Button returnButton;

        void Start()
        {
            tabSelectFields.Add(ipInputField_1.gameObject);
            tabSelectFields.Add(ipInputField_2.gameObject);
            tabSelectFields.Add(ipInputField_3.gameObject);
            tabSelectFields.Add(ipInputField_4.gameObject);
            tabSelectFields.Add(portInputField.gameObject);
            tabSelectFields.Add(resetButton.gameObject);
            tabSelectFields.Add(saveButton.gameObject);
            tabSelectFields.Add(returnButton.gameObject);

            firstSelectField = ipInputField_1.gameObject;

            ipInputField_1.onValueChanged.AddListener(delegate { Utility.ValidateNum(ipInputField_1, 255); });
            ipInputField_2.onValueChanged.AddListener(delegate { Utility.ValidateNum(ipInputField_2, 255); });
            ipInputField_3.onValueChanged.AddListener(delegate { Utility.ValidateNum(ipInputField_3, 255); });
            ipInputField_4.onValueChanged.AddListener(delegate { Utility.ValidateNum(ipInputField_4, 255); });
            portInputField.onValueChanged.AddListener(delegate { Utility.ValidateNum(portInputField, 65535); });

            resetButton.onClick.AddListener(delegate { ResetIPandPort(); });
            saveButton.onClick.AddListener(delegate { SaveIPandPort(); });
            returnButton.onClick.AddListener(delegate { GameHallUIManager.Instance.CloseCurrent(); });
        }

        protected override void FirstInit()
        {
            SelectFirstField();
            GetIPandPort();
        }

        void GetIPandPort()
        {
            string[] ip = GameMgr.instance.host.Split('.');
            ipInputField_1.text = ip[0];
            ipInputField_2.text = ip[1];
            ipInputField_3.text = ip[2];
            ipInputField_4.text = ip[3];
            portInputField.text = GameMgr.instance.port.ToString();
        }
        void ResetIPandPort()
        {
            GameMgr.instance.host = "120.77.34.23";
            GameMgr.instance.port = 8000;
            GetIPandPort();
            Debug.Log("重置ip地址和端口成功");
            GameHallUIManager.Instance.CloseCurrent();
        }

        void SaveIPandPort()
        {
            GameMgr.instance.host = ipInputField_1.text + "." + ipInputField_2.text + "." + ipInputField_3.text + "." + ipInputField_4.text;
            GameMgr.instance.port = int.Parse(portInputField.text);
            Debug.Log("ip地址和端口修改成功");
            GameHallUIManager.Instance.CloseCurrent();
        }
    }
}