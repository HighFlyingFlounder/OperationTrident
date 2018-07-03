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

        private void Start() {
            m_AnimatorParameters = m_Animator.parameters;

            foreach (AnimatorControllerParameter param in m_AnimatorParameters) {
                Debug.Log(param.name);
                Debug.Log(param.defaultBool);
                Debug.Log(param.defaultFloat);
                Debug.Log(param.defaultInt);
                Debug.Log(param.type);
            }
        }

        private void SetWalkAnimationParamters(Vector2 input) {
            m_Animator.SetFloat("speed", input.magnitude);
        }

        private void SetUnderarmAnimationParamters(bool isUnderarming) {
            m_Animator.SetBool("isUnderarming", isUnderarming);
        }
    }
}
