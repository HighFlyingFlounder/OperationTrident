using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.FPS.Weapons {
    public class WeaponIKController : MonoBehaviour {
        public Transform LootAtTarget;
        public Transform RightHandTarget;
        public Transform LeftHandTarget;

        //Player Model对象
        public GameObject PlayerModel;

        private Animator m_Animator;

        // Use this for initialization
        void Start() {
            m_Animator = PlayerModel.GetComponent<Animator>();
            //动态复制Animator
            Animator animator = this.gameObject.AddComponent<Animator>();
            animator.avatar = m_Animator.avatar;
            animator.runtimeAnimatorController = m_Animator.runtimeAnimatorController;
        }

        private void OnAnimatorIK(int layerIndex) {
            if (LootAtTarget != null) {
                m_Animator.SetLookAtWeight(1);
                m_Animator.SetLookAtPosition(LootAtTarget.position);
            }

            if (RightHandTarget != null) {
                m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                m_Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
                m_Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
            }

            if (LeftHandTarget != null) {
                m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
                m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
            }  
        }
    }
}
