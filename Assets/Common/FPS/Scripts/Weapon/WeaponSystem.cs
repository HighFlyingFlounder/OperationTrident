using UnityEngine;
using System.Collections;

namespace OperationTrident.Weapons {
    public class WeaponSystem : MonoBehaviour {
        //保存武器对象的数组
        public GameObject[] Weapons;
        //默认武器索引
        public int StartingWeaponIndex = 0;

        //当前正在使用的武器索引
        private int m_WeaponIndex;

        private int[] m_WeaponAmmunition;

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
            m_WeaponAmmunition = new int[Weapons.Length];

            //确保其他武器都处于禁用状态
            for (int i = 0; i < Weapons.Length; i++) {
                //初始化当前弹药量，默认都是最大容量
                m_WeaponAmmunition[i] = Weapons[i].GetComponent<Weapon>().AmmoCapacity;

                Debug.Log(m_WeaponAmmunition[i]);
            }
        }

        private void UpdateWeaponsAmmunition(int ammo) {
            m_WeaponAmmunition[m_WeaponIndex] = ammo;

            Debug.Log(m_WeaponIndex + " " + m_WeaponAmmunition[m_WeaponIndex]);
        }
    }
}