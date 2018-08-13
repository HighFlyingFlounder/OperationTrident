using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.UI
{
    public class PlayerInfoController : MonoBehaviour
    {
        [SerializeField]
        GameObject weaponSystem = null;

        PlayerInfoUIController UIController;
        IPlayerInfo playerInfo;
        IWeaponInfo weaponInfo;

        void Start()
        {
            if(this.gameObject.name != GameMgr.instance.id)
            {
                this.enabled = false;
                return;
            }
            playerInfo = this.gameObject.GetComponent<IPlayerInfo>();
            if(weaponSystem != null)
                weaponInfo = weaponSystem.GetComponent<IWeaponInfo>();

            UIController = GamingUIManager.Instance.GetPlayerInfoUIController();
            UIController.SetTotalHP(playerInfo.TotalHP);

            if (weaponSystem != null)
                UIController.ShowAmmoInfo();
            else
                UIController.HideAmmoInfo();

            GamingUIManager.Instance.ShowPlayerInfoUI();
        }

        void Update()
        {
            UIController.SetCurrentHP(playerInfo.CurrentHP);
            if(weaponSystem != null)
            {
                UIController.SetTotalAmmo(weaponInfo.isInfinite, weaponInfo.TotalAmmo);
                UIController.SetCurrentAmmo(weaponInfo.CurrentAmmo);
            }
        }

        private void OnDestroy()
        {
            GamingUIManager.Instance.HidePlayerInfoUI();
        }
        
    }
}