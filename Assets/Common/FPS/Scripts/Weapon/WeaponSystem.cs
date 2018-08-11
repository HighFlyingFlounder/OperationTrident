using UnityEngine;
using System.Collections;
using OperationTrident.Common.UI;

namespace OperationTrident.FPS.Weapons {
    public class WeaponSystem : MonoBehaviour, NetSyncInterface, IWeaponInfo {
        public bool IsLocalObject = true;

        //保存武器对象的数组
        public GameObject[] WeaponGameObjects;
        //默认武器索引
        public int StartingWeaponIndex = 0;

        //管理的武器数量
        private int m_WeaponNumber;
        //当前正在使用的武器索引
        private int m_WeaponIndex;

        private Weapon[] m_Weapons;

        private NetSyncController m_NetSyncController;

        public int CurrentAmmo
        {
            get
            {
                return m_Weapons[m_WeaponIndex].m_CurrentAmmo;
            }
        }

        public int TotalAmmo
        {
            get
            {
                return m_Weapons[m_WeaponIndex].m_CurrentTotalAmmo;
            }
        }

        public bool isInfinite
        {
            get
            {
                return !m_Weapons[m_WeaponIndex].LimitedTotalAmmo;
            }
        }

        void Start() {
            //初始化武器系统
            m_WeaponNumber = WeaponGameObjects.Length;

            InitWeapons();

            //确保激活默认武器
            m_WeaponIndex = StartingWeaponIndex;
            SetActiveWeapon(m_WeaponIndex);
        }

        // Update is called once per frame
        void Update() {
            //不是LocalPlayer，不执行任何操作
            if (!IsLocalObject) {
                return;
            }

            //使用鼠标滚轮切换武器
            if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                //使用RPC函数
                NextWeapon();

            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                //使用RPC函数
                PreviousWeapon();
            }
        }

        public void SetActiveWeapon(int index) {
            //在激活前确保武器存在
            if (index >= m_WeaponNumber || index < 0) {
                Debug.LogWarning("Tried to switch to a weapon that does not exist.  Make sure you have all the correct weapons in your weapons array.");
                return;
            }

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

        

        private void InitWeapons() {
            //m_WeaponsTotalAmmunition = new int[m_WeaponNumber];
            m_Weapons = new Weapon[m_WeaponNumber];

            //确保其他武器都处于禁用状态
            for (int i = 0; i < m_WeaponNumber; i++) {
                WeaponGameObjects[i].SetActive(true);

                m_Weapons[i] = WeaponGameObjects[i].GetComponent<Weapon>();
                m_Weapons[i].enabled = true;
            }
        }

        #region RPC函数
        //切换下一把武器
        public void NextWeapon() {
            //调用RPC函数
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "NextWeapon");
            }

            m_WeaponIndex++;
            if (m_WeaponIndex > m_WeaponNumber - 1)
                m_WeaponIndex = 0;

            SetActiveWeapon(m_WeaponIndex);
        }

        //切换上一把武器
        public void PreviousWeapon() {
            //调用RPC函数
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "PreviousWeapon");
            }

            m_WeaponIndex--;
            if (m_WeaponIndex < 0)
                m_WeaponIndex = m_WeaponNumber - 1;

            SetActiveWeapon(m_WeaponIndex);
        }

        public void RecvData(SyncData data) {
            throw new System.NotImplementedException();
        }

        public SyncData SendData() {
            return new SyncData();
        }

        public void Init(NetSyncController controller) {
            m_NetSyncController = controller;
        }
        #endregion
    }
}