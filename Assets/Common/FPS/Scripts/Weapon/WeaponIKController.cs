using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.FPS.Weapons {
    [RequireComponent(typeof(Animator))]
    public class WeaponIKController : MonoBehaviour {
        public Transform LootAtTarget;
        public Transform RightHandTarget;
        public Transform LeftHandTarget;

        public GameObject PlayerModel;

        private Animator m_Animator;

        // Use this for initialization
        void Start() {
            m_Animator = PlayerModel.GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex) {
            if (LookAtObj != null) {
                m_Animator.SetLookAtWeight(1);
                m_Animator.SetLookAtPosition(LookAtObj.position);
            }

            if (RightHandObj != null) {
                m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                m_Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandObj.position);
                m_Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandObj.rotation);
            }

            if (LeftHandObj != null) {
                m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandObj.position);
                m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandObj.rotation);
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
