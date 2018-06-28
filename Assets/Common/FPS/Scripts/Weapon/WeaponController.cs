using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.CrossInput;

namespace OperationTrident.Weapon {
    enum Weapon {
        None,
        Gun,
        //EMP,
        Grenade
    }
    public class WeaponController : MonoBehaviour {
        public GameObject Gun;
        //public GameObject EMP;
        public GameObject Grenade;

        private GunIKController m_GunIKController;
        private GrenadeIKController m_GrenadeIKController;
        private InputManager m_InputManager;

        private Weapon m_CurrentWeapon;
        private Coroutine m_CurrentCoroutine;
        private bool m_SwitchState;
        private void Awake() {
            m_GunIKController = Gun.GetComponent<GunIKController>();
            m_GrenadeIKController = Grenade.GetComponent<GrenadeIKController>();
            m_InputManager = GetComponent<InputManager>();
        }

        // Use this for initialization
        void Start() {
            //默认拿枪
            m_CurrentWeapon = Weapon.Gun;
            //初始没有协程
            m_CurrentCoroutine = null;

            m_SwitchState = false;
        }

        // Update is called once per frame
        void Update() {
            switch (m_CurrentWeapon) {
                case Weapon.None:
                    if (m_InputManager.SwitchButtonDown()) {
                        if (m_CurrentCoroutine != null) {
                            StopCoroutine(m_CurrentCoroutine);
                        }
                        m_CurrentCoroutine = StartCoroutine(m_GunIKController.PickGun());
                        m_CurrentWeapon = Weapon.Gun;
                    }
                    break;

                case Weapon.Gun:
                    if (m_InputManager.SwitchButtonDown()) {
                        if (m_CurrentCoroutine != null) {
                            StopCoroutine(m_CurrentCoroutine);
                        }
                        //拿枪
                        m_CurrentCoroutine = StartCoroutine(m_GunIKController.PutGun());
                        //按下siwtch按键，开启更换武器状态
                        m_SwitchState = true;
                    } else {
                        //收枪完毕，拿手雷
                        if (m_SwitchState && !m_GunIKController.RuningCoroutine) {
                            m_CurrentCoroutine =
                                StartCoroutine(m_GrenadeIKController.PickGrenade());
                            //进入下一个状态
                            m_CurrentWeapon = Weapon.Grenade;

                            //武器更换完毕，关闭武器更换状态
                            m_SwitchState = false;
                        }
                    }
                    break;

                case Weapon.Grenade:
                    if (m_InputManager.SwitchButtonDown()) {
                        if (m_CurrentCoroutine != null) {
                            StopCoroutine(m_CurrentCoroutine);
                        }

                        m_CurrentCoroutine = StartCoroutine(m_GrenadeIKController.PutGrenade());
                        //按下siwtch按键，开启更换武器状态
                        m_SwitchState = true;
                    } else {
                        //拿手雷完毕
                        if (m_SwitchState && !m_GrenadeIKController.RuningCoroutine) {
                            //进入下一个状态
                            m_CurrentWeapon = Weapon.None;
                            //武器更换完毕，关闭武器更换状态
                            m_SwitchState = false;
                        }
                    }
                    break;
                    
            }
        }
    }
}
