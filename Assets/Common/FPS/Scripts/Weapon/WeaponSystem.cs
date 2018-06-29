using UnityEngine;
using System.Collections;

namespace OperationTrident.Weapons {
    public class WeaponSystem : MonoBehaviour {
        //保存武器对象的数组
        public GameObject[] weapons;
        //默认武器索引
        public int startingWeaponIndex = 0;

        //当前正在使用的武器索引
        private int weaponIndex;


        // Use this for initialization
        void Start() {
            //确保激活默认武器
            weaponIndex = startingWeaponIndex;
            SetActiveWeapon(weaponIndex);
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
            if (index >= weapons.Length || index < 0) {
                Debug.LogWarning("Tried to switch to a weapon that does not exist.  Make sure you have all the correct weapons in your weapons array.");
                return;
            }

            //向父对象传递消息
            SendMessageUpwards("OnEasyWeaponsSwitch", SendMessageOptions.DontRequireReceiver);

            //更新当前使用的武器索引
            weaponIndex = index;

            //如果是激光武器，在切换前先停止发射激光
            weapons[index].GetComponent<Weapon>().StopBeam();

            //确保其他武器都处于禁用状态
            for (int i = 0; i < weapons.Length; i++) {
                weapons[i].SetActive(false);
            }

            //启用当前的武器
            weapons[index].SetActive(true);
        }

        public void NextWeapon() {
            weaponIndex++;
            if (weaponIndex > weapons.Length - 1)
                weaponIndex = 0;
            SetActiveWeapon(weaponIndex);
        }

        public void PreviousWeapon() {
            weaponIndex--;
            if (weaponIndex < 0)
                weaponIndex = weapons.Length - 1;
            SetActiveWeapon(weaponIndex);
        }
    }
}