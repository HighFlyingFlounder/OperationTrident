using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.UI
{
    public class SpaceBattlePlayerInfo : MonoBehaviour
    {
        PlayerInfoUIController UIController;
        FlyerController Player;

        void Start()
		{
            UIController = GamingUIManager.Instance.GetPlayerInfoUIController();
            UIController.HideAmmoInfo();
            Player = GameObject.Find(GameMgr.instance.id).GetComponent<FlyerController>();
            UIController.SetTotalHP(Player.TotalHp);
            UIController.SetCurrentHP(Player.Hp);
            GamingUIManager.Instance.ShowPlayerInfoUI();
		}

        void Update()
        {
            UIController.SetCurrentHP(Player.Hp);
        }

        public void Hide()
        {
            GamingUIManager.Instance.HidePlayerInfoUI();
        }
    }
}