using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.FPS.Player {
    public class AnimationController : MonoBehaviour {

        public GameObject PlayerModel;

        private Animator m_Animator;

        private AnimatorControllerParameter[] m_AnimatorParameters;

        private void Awake() {
            m_Animator = PlayerModel.GetComponent<Animator>();
        }

        private void SetWalkAnimationParamters(Vector2 input) {
            m_Animator.SetFloat("turn", input.x);
            m_Animator.SetFloat("forward", input.y);
        }

        private void SetRunAnimationParamters(bool running) {
            m_Animator.SetBool("isRunning", running);
        }

        private void SetCrouchAnimationParamters(bool isCrouching) {
            m_Animator.SetBool("isCrouching", isCrouching);
        }

        private void SetJumpAnimationParamters() {
            m_Animator.SetTrigger("jump");
        }
    }
}
