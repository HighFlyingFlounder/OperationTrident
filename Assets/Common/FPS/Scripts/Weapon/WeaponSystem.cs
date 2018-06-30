﻿using UnityEngine;
using System.Collections;

namespace OperationTrident.Weapons {
    public class WeaponSystem : MonoBehaviour {
        //保存武器对象的数组
        public GameObject[] Weapons;
        //默认武器索引
        public int StartingWeaponIndex = 0;

        //当前正在使用的武器索引
        private int m_WeaponIndex;

        private int[] m_WeaponsTotalAmmunition;

        // Use this for initialization
        void Start() {
            InitWeaponsAmmunition();


            //确保激活默认武器
            m_WeaponIndex = StartingWeaponIndex;
            SetActiveWeapon(m_WeaponIndex);
        }

        // Update is called once per frame
        void Update() {
            //使用鼠标滚轮切换武器
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                NextWeapon();
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                PreviousWeapon();
        }

        public void SetActiveWeapon(int index) {
            //在激活前确保武器存在
            if (index >= Weapons.Length || index < 0) {
                Debug.LogWarning("Tried to switch to a weapon that does not exist.  Make sure you have all the correct weapons in your weapons array.");
                return;
            }

            //向父对象传递消息
            SendMessageUpwards("OnEasyWeaponsSwitch", SendMessageOptions.DontRequireReceiver);

            //更新当前使用的武器索引
            m_WeaponIndex = index;

            //如果是激光武器，在切换前先停止发射激光
            Weapons[index].GetComponent<Weapon>().StopBeam();

            //确保其他武器都处于禁用状态
            for (int i = 0; i < Weapons.Length; i++) {
                Weapons[i].SetActive(false);
            }

            //启用当前的武器
            Weapons[index].SetActive(true);
        }

        public void NextWeapon() {
            m_WeaponIndex++;
            if (m_WeaponIndex > Weapons.Length - 1)
                m_WeaponIndex = 0;
            SetActiveWeapon(m_WeaponIndex);
        }

        public void PreviousWeapon() {
            m_WeaponIndex--;
            if (m_WeaponIndex < 0)
                m_WeaponIndex = Weapons.Length - 1;
            SetActiveWeapon(m_WeaponIndex);
        }

        private void InitWeaponsAmmunition() {
            m_WeaponsTotalAmmunition = new int[Weapons.Length];
            int ammo, totalAmmo;

            //确保其他武器都处于禁用状态
            for (int i = 0; i < Weapons.Length; i++) {
                Weapon weapon = Weapons[i].GetComponent<Weapon>();
                if (weapon.InfiniteAmmo) {
                    //-1代表无限子弹
                    m_WeaponsTotalAmmunition[i] = -1;
                    return;
                }

                ammo = weapon.AmmoCapacity;
                totalAmmo = weapon.TotalAmmunition;

                //初始化当前总弹药量
                m_WeaponsTotalAmmunition[i] = totalAmmo >= ammo ? totalAmmo : ammo;

                Debug.Log(m_WeaponsTotalAmmunition[i]);
            }
        }

        private void UpdateWeaponsTotalAmmunition(int ammo) {
            m_WeaponsTotalAmmunition[m_WeaponIndex] = ammo;

            Debug.Log(m_WeaponIndex + " " + m_WeaponsTotalAmmunition[m_WeaponIndex]);
        }
    }
}