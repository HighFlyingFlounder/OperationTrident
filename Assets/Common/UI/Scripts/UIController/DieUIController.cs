using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class DieUIController : UIBase
    {
        [Header("Button")]
        [SerializeField]
        Button returnButton;

        void Start()
        {
            returnButton.onClick.AddListener(delegate { 
                GameMgr.ReturnRoom(); 
                GamingUIManager.Instance.HideDieUI();
            });
        }
    }
    
}