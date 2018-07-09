using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.FPS.Common {
    public class GetCamera : MonoBehaviour {
        public GameObject MainCamera;
        public GameObject[] MirrorCameras;

        private Camera m_MainCamera;
        private Camera[] m_MirrorCameras;

        void Awake() {
            //默认为主摄像机
            m_MainCamera = MainCamera.GetComponent<Camera>();

            m_MirrorCameras = new Camera[MirrorCameras.Length];
            for(int i = 0; i < MirrorCameras.Length; i++) {
                m_MirrorCameras[i] = MirrorCameras[i].GetComponent<Camera>();
            }
        }

        public Camera GetCurrentUsedCamera() {
            for (int i = 0; i < MirrorCameras.Length; i++) {
                if (MirrorCameras[i].activeSelf) {
                    return m_MirrorCameras[i];
                }
            }

            return m_MainCamera;
        }

        public GameObject GetCurrentUsedCameraGameObject() {
            for (int i = 0; i < MirrorCameras.Length; i++) {
                if (MirrorCameras[i].activeSelf) {
                    return MirrorCameras[i];
                }
            }

            return MainCamera;
        }

        public Camera GetMainCamera() {
            return m_MainCamera;
        }

        public GameObject GetMainCameraGameObject() {
            return MainCamera;
        }
    }
}
