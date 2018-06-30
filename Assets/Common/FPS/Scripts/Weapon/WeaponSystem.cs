using UnityEngine;
using System.Collections;

namespace OperationTrident.FPS.Weapons {
    public class WeaponSystem : MonoBehaviour {
        //保存武器对象的数组
        public GameObject[] WeaponGameObjects;
        //默认武器索引
        public int StartingWeaponIndex = 0;

        private int m_WeaponNumber;
        //当前正在使用的武器索引
        private int m_WeaponIndex;
        //保存所有武器的弹药量
        private int[] m_WeaponsTotalAmmunition;
        private Weapon[] m_Weapons;

        void Start() {
            m_WeaponNumber = WeaponGameObjects.Length;

            InitWeapons();

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
            if (index >= m_WeaponNumber || index < 0) {
                Debug.LogWarning("Tried to switch to a weapon that does not exist.  Make sure you have all the correct weapons in your weapons array.");
                return;
            }

            //向父对象传递消息
            //SendMessageUpwards("OnEasyWeaponsSwitch", SendMessageOptions.DontRequireReceiver);

            //更新当前使用的武器索引
            m_WeaponIndex = index;

            //如果是激光武器，在切换前先停止发射激光
            m_Weapons[index].StopBeam();

            //确保其他武器都处于禁用状态
            for (int i = 0; i < m_WeaponNumber; i++) {
                m_Weapons[i].enabled = false;
            }

            //启用当前的武器
            m_Weapons[index].enabled = true;
        }

        public void NextWeapon() {
            m_WeaponIndex++;
            if (m_WeaponIndex > m_WeaponNumber - 1)
                m_WeaponIndex = 0;

            SetActiveWeapon(m_WeaponIndex);
        }

        public void PreviousWeapon() {
            m_WeaponIndex--;
            if (m_WeaponIndex < 0)
                m_WeaponIndex = m_WeaponNumber - 1;

            SetActiveWeapon(m_WeaponIndex);
        }

        private void InitWeapons() {
            m_WeaponsTotalAmmunition = new int[m_WeaponNumber];
            m_Weapons = new Weapon[m_WeaponNumber];

            int ammo, totalAmmo;

            //确保其他武器都处于禁用状态
            for (int i = 0; i < m_WeaponNumber; i++) {
                WeaponGameObjects[i].SetActive(true);

                m_Weapons[i] = WeaponGameObjects[i].GetComponent<Weapon>();
                if (m_Weapons[i].InfiniteAmmo) {
                    //-1代表无限子弹
                    m_WeaponsTotalAmmunition[i] = -1;
                    return;
                }

                ammo = m_Weapons[i].AmmoCapacity;
                totalAmmo = m_Weapons[i].TotalAmmunition;

                //初始化当前总弹药量
                m_WeaponsTotalAmmunition[i] = totalAmmo >= ammo ? totalAmmo : ammo;
            }
        }


        private void UpdateWeaponsTotalAmmunition(int ammo) {
            m_WeaponsTotalAmmunition[m_WeaponIndex] = ammo;

            Debug.Log(m_WeaponIndex + " " + m_WeaponsTotalAmmunition[m_WeaponIndex]);
        }
        
    }
}