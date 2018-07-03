using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.FPS.Player {
    public class FollowPlayerModel : MonoBehaviour {
        public GameObject PlayerModel;

        private Transform m_Root;
        private Vector3 m_OriginalLocalPosition;

        // Use this for initialization
        void Start() {
            m_Root = PlayerModel.transform.Find("Root");
            if(m_Root == null) {
                Debug.LogError("Please make sure the PlayerModel has a child named Root");
            }

            m_OriginalLocalPosition = m_Root.InverseTransformPoint(this.transform.position);
        }

        private void Update() {
            this.transform.position = m_Root.TransformPoint(m_OriginalLocalPosition);
        }
    }
}
