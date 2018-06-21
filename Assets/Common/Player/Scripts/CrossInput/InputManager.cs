using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.CrossInput {
    //[RequireComponent(typeof(NetSyncController))]
    public class InputManager : MonoBehaviour,NetSyncInterface {
        public bool IsLocalPlayer;
        private NetSyncController m_NetSyncController;
        [SerializeField]
        private bool m_HasNetwork = false;

        private Hashtable m_Hashtable;

        //private bool m_MirrorButtonDown;
        //private bool m_AttackButtonDown;
        //private bool m_PressAttackButton;
        //private bool m_AttackButtonUp;
        private bool m_SwitchButtonDown;
        private bool m_PressSpeedUpButton;
        private bool m_JumpButtonDown;

        private bool m_TempBool;

        //private float m_MouseX;
        private float m_MouseY;
        private float m_Horizontal;
        private float m_Vertical;

        private float m_TempFloat;

        private void Awake() {
            m_Hashtable = new Hashtable{
                //{ "MirrorButtonDown", false },
                //{ "AttackButtonDown", false },
                //{ "PressAttackButton", false },
                //{ "AttackButtonUp", false },
                { "SwitchButtonDown", false },
                { "PressSpeedUpButton", false },
                { "JumpButtonDown", false },
                //{ "MouseX", 0f },
                { "MouseY", 0f },
                { "Horizontal", 0f },
                { "Vertical", 0f }
            };

            m_TempBool = false;
            m_TempFloat = 0f;
        }

        public bool MirrorButtonDown() {
            if (IsLocalPlayer) {
                m_TempBool = Input.GetMouseButtonDown(1);
                //if ((bool)m_Hashtable["MirrorButtonDown"] != m_TempBool) {
                //    m_Hashtable["MirrorButtonDown"] = m_TempBool;
                //    m_NetSyncController.SyncVariables();
                //}

                return m_TempBool;
            } else {
                ////接收到true条件时，自动重置为false，不等待下一次网络同步
                //m_TempBool = m_MirrorButtonDown;
                //if (m_MirrorButtonDown) {
                //    m_MirrorButtonDown = false;
                //}
                //return m_TempBool;
                return false;
            }
        }

        public bool AttackButtonDown() {
            if (IsLocalPlayer) {
                m_TempBool = Input.GetMouseButtonDown(0);

                //if ((bool)m_Hashtable["AttackButtonDown"] != m_TempBool) {
                //    m_Hashtable["AttackButtonDown"] = m_TempBool;
                //    m_NetSyncController.SyncVariables();
                //}
                return m_TempBool;
            } else {
                ////接收到true条件时，自动重置为false，不等待下一次网络同步
                //m_TempBool = m_AttackButtonDown;
                //if (m_AttackButtonDown) {
                //    m_AttackButtonDown = false;
                //}
                //return m_TempBool;
                return false;
            }
        }
        public bool PressAttackButton() {
            if (IsLocalPlayer) {
                m_TempBool = Input.GetMouseButton(0);

                //if ((bool)m_Hashtable["PressAttackButton"] != m_TempBool) {
                //    m_Hashtable["PressAttackButton"] = m_TempBool;
                //    m_NetSyncController.SyncVariables();
                //}
                return m_TempBool;
            } else {
                //return m_PressAttackButton;
                return false;
            }
        }
        public bool AttackButtonUp() {
            if (IsLocalPlayer) {
                m_TempBool = Input.GetMouseButtonUp(0);

                //if ((bool)m_Hashtable["AttackButtonUp"] != m_TempBool) {
                //    m_Hashtable["AttackButtonUp"] = m_TempBool;
                //    m_NetSyncController.SyncVariables();
                //}
                return m_TempBool;
            } else {
                ////接收到true条件时，自动重置为false，不等待下一次网络同步
                //m_TempBool = m_AttackButtonUp;
                //if (m_AttackButtonUp) {
                //    m_AttackButtonUp = false;
                //}
                //return m_TempBool;
                return false;
            }
        }

        public bool SwitchButtonDown() {
            if (IsLocalPlayer) {
                m_TempBool = Input.GetKeyDown(KeyCode.H);

                if (m_HasNetwork && (bool)m_Hashtable["SwitchButtonDown"] != m_TempBool) {
                    m_Hashtable["SwitchButtonDown"] = m_TempBool;
                    m_NetSyncController.SyncVariables();
                }
                return m_TempBool;
            } else {
                //接收到true条件时，自动重置为false，不等待下一次网络同步
                m_TempBool = m_SwitchButtonDown;
                if (m_SwitchButtonDown) {
                    m_SwitchButtonDown = false;
                }
                return m_TempBool;
            }
        }

        public bool PressSpeedUpButton() {
            if (IsLocalPlayer) {
                m_TempBool = Input.GetKey(KeyCode.LeftShift);

                if (m_HasNetwork && (bool)m_Hashtable["PressSpeedUpButton"] != m_TempBool) {
                    m_Hashtable["PressSpeedUpButton"] = m_TempBool;
                    m_NetSyncController.SyncVariables();
                }
                return m_TempBool;
            } else {
                return m_PressSpeedUpButton;
            }
        }

        public bool JumpButtonDown() {
            if (IsLocalPlayer) {
                m_TempBool = Input.GetButtonDown("Jump");

                if (m_HasNetwork && (bool)m_Hashtable["JumpButtonDown"] != m_TempBool) {
                    m_Hashtable["JumpButtonDown"] = m_TempBool;
                    m_NetSyncController.SyncVariables();
                }
                return m_TempBool;
            } else {
                //接收到true条件时，自动重置为false，不等待下一次网络同步
                m_TempBool = m_JumpButtonDown;
                if (m_JumpButtonDown) {
                    m_JumpButtonDown = false;
                }
                return m_TempBool;
            }
        }

        public float MouseX() {
            if (IsLocalPlayer) {
                m_TempFloat = Input.GetAxis("Mouse X");

                //if (Mathf.Abs((float)m_Hashtable["MouseX"] - m_TempFloat) > 0) {
                //    m_Hashtable["MouseX"] = m_TempFloat;
                //    m_NetSyncController.SyncVariables();
                //}
                return m_TempFloat;
            } else {
                //return m_MouseX;
                return 0f;
            }
        }
        public float MouseY() {
            if (IsLocalPlayer) {
                m_TempFloat = Input.GetAxis("Mouse Y");

                if (m_HasNetwork && Mathf.Abs((float)m_Hashtable["MouseY"] - m_TempFloat) > 0) {
                    Debug.Log(m_HasNetwork);
                    m_Hashtable["MouseY"] = m_TempFloat;
                    m_NetSyncController.SyncVariables();
                }
                return m_TempFloat;
            } else {
                return m_MouseY;
            }
        }

        public float Horizontal() {
            if (IsLocalPlayer) {
                m_TempFloat = Input.GetAxis("Horizontal");

                if (m_HasNetwork && Mathf.Abs((float)m_Hashtable["Horizontal"] - m_TempFloat) > 0) {
                    m_Hashtable["Horizontal"] = m_TempFloat;
                    m_NetSyncController.SyncVariables();
                }
                return m_TempFloat;
            } else {
                return m_Horizontal;
            }
        }

        public float Vertical() {
            if (IsLocalPlayer) {
                m_TempFloat = Input.GetAxis("Vertical");

                if (m_HasNetwork && Mathf.Abs((float)m_Hashtable["Vertical"] - m_TempFloat) > 0) {
                    m_Hashtable["Vertical"] = m_TempFloat;
                    m_NetSyncController.SyncVariables();
                }
                return m_TempFloat;
            } else {
                return m_Vertical;
            }
        }

        public void SetData(SyncData data) {
            //m_MirrorButtonDown = (bool)data.Get(typeof(bool));
            //m_AttackButtonDown = (bool)data.Get(typeof(bool));
            //m_PressAttackButton = (bool)data.Get(typeof(bool));
            //m_AttackButtonUp = (bool)data.Get(typeof(bool));
            m_SwitchButtonDown = (bool)data.Get(typeof(bool));
            m_PressSpeedUpButton = (bool)data.Get(typeof(bool));
            m_JumpButtonDown = (bool)data.Get(typeof(bool));

            //m_MouseX = (float)data.Get(typeof(float));
            m_MouseY = (float)data.Get(typeof(float));
            m_Horizontal = (float)data.Get(typeof(float));
            m_Vertical = (float)data.Get(typeof(float));
        }

        public SyncData GetData() {
            SyncData data = new SyncData();

            //data.Add(m_Hashtable["MirrorButtonDown"]);
            //data.Add(m_Hashtable["AttackButtonDown"]);
            //data.Add(m_Hashtable["PressAttackButton"]);
            //data.Add(m_Hashtable["AttackButtonUp"]);
            data.Add(m_Hashtable["SwitchButtonDown"]);
            data.Add(m_Hashtable["PressSpeedUpButton"]);
            data.Add(m_Hashtable["JumpButtonDown"]);

            //data.Add(m_Hashtable["MouseX"]);
            data.Add(m_Hashtable["MouseY"]);
            data.Add(m_Hashtable["Horizontal"]);
            data.Add(m_Hashtable["Vertical"]);
            return data;
        }

        public void Init(NetSyncController controller) {
            m_NetSyncController = controller;
        }
    }
}