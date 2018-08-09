﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OperationTrident.Common.UI
{
    public class PlayerInfoUIController : UIBase
    {
        [Header("UI Content")]
        [SerializeField]
        GameObject HpUI;

        [SerializeField]
        GameObject AmmoUI;

        [Header("Slider")]
        [SerializeField]
        Slider HpSlider;

        [Header("Text")]
        [SerializeField]
        Text totalHPText;
        [SerializeField]
        Text currentHPText;

        int totalHP;
        int currentHP;

        [SerializeField]
        Text totalAmmo;
        [SerializeField]
        Text infiniteAmmo;
        [SerializeField]
        Text currentAmmo;

        public void HideAmmoInfo()
        {
            AmmoUI.SetActive(false);
        }

        public void ShowAmmoInfo()
        {
            AmmoUI.SetActive(true);
        }

        void ShowTotalAmmo(bool isInfinite)
        {
            infiniteAmmo.gameObject.SetActive(isInfinite);
            totalAmmo.gameObject.SetActive(!isInfinite);
        }

        public void SetTotalHP(int hp)
        {
            totalHP = hp;
            totalHPText.text = hp.ToString();
        }

        public void SetCurrentHP(int hp)
        {
            currentHP = hp;
            currentHPText.text = hp.ToString();
            HpSlider.value = (float)currentHP / (float)totalHP;
        }

        public void SetTotalAmmo(bool isInfinite, int ammoNum)
        {
            totalAmmo.text = ammoNum.ToString();
            ShowTotalAmmo(isInfinite);
        }

        public void SetCurrentAmmo(int ammoNum)
        {
            currentAmmo.text = ammoNum.ToString();
        }

        private new void Update(){}
    }
}