using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.FPS.Common {
    public class LockCursor : MonoBehaviour {
        private bool m_CursorIsLocked;

        private void Start() {
            m_CursorIsLocked = true;
        }

        // Update is called once per frame
        void Update() {
            UpdateCursorLock();
        }

        //检测按键状态
        private void UpdateCursorLock() {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                m_CursorIsLocked = false;
            } else if (Input.GetMouseButtonUp(0)) {
                m_CursorIsLocked = true;
            }

            if (m_CursorIsLocked) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else if (!m_CursorIsLocked) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

}