using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.FPS.Player {
    public class FollowPlayerModel : MonoBehaviour {
        public Transform PlayerModelTarget;

        private Vector3 m_OriginalLocalPosition;

        // Use this for initialization
        void Start() {
            if(PlayerModelTarget == null) {
                Debug.LogError("Please make sure to set the PlayerModelTarget...");
            }

            m_OriginalLocalPosition = PlayerModelTarget.InverseTransformPoint(this.transform.position);
        }

        private void Update() {
            this.transform.position = PlayerModelTarget.TransformPoint(m_OriginalLocalPosition);
        }
    }
}
