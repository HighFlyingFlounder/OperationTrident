using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.UI
{
    public class GamingUIManager : UIManager
    {
        static GamingUIManager instance;

        public static GamingUIManager Instance
        {
            get
            {
                return instance;
            }
        }

        protected void Awake()
        {
            if (instance == null)
            {
                instance = this as GamingUIManager;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        [Header("UI Prefab")]
        [SerializeField]
        GameObject playerInfoUIPrefab;
        GameObject playerInfoUI = null;

        [SerializeField]
        GameObject dieUIPrefab;
        GameObject dieUI = null;

        void Start()
        {
            InitUIOnce(playerInfoUIPrefab, ref playerInfoUI);
            InitUIOnce(dieUIPrefab, ref dieUI);
        }


        public PlayerInfoUIController GetPlayerInfoUIController()
        {
            return playerInfoUI.GetComponent<PlayerInfoUIController>();
        }

        public void ShowPlayerInfoUI()
        {
            playerInfoUI.SetActive(true);
        }

        public void HidePlayerInfoUI()
        {
            if(playerInfoUI != null)
                playerInfoUI.SetActive(false);
        }

        public void ShowDieUI()
        {
            dieUI.SetActive(true);
            dieUI.GetComponent<Animation>().Play();
        }

        public void HideDieUI()
        {
            dieUI.SetActive(false);
        }
    }
}