using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.FPS.Common;

namespace OperationTrident.FPS.Player {
    public class HeadBobbing : MonoBehaviour {
        [SerializeField] private Bobbing m_Bobbing = new Bobbing();
        //用来保存协程
        private Coroutine m_Coroutine;
        private Vector3 m_OriginalPosition;

        // Use this for initialization
        void Start() {
            m_Bobbing.Init(this.transform);

            m_OriginalPosition = this.transform.localPosition;
        }

        private void ChangeMovementSpeed(float speed) {
            m_Bobbing.ChangeSpeed(speed);
        }

        private void StartWalking() {
            m_Coroutine = StartCoroutine(m_Bobbing.DoBobbing());
        }

        private void StopWalking() {
            StopCoroutine(m_Coroutine);

            transform.localPosition = m_OriginalPosition;
        }
    }

}