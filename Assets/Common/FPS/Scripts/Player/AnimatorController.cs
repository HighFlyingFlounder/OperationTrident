using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.CrossInput;

namespace OperationTrident.Player {
    public class AnimatorController : MonoBehaviour {

        private Animator m_Animator;
        private InputManager m_InputManager;

        private Vector2 m_Input;

        private void Awake() {
            m_Animator = GetComponent<Animator>();
            m_InputManager = GetComponent<InputManager>();
        }

        // Update is called once per frame
        void Update() {
            m_Input = new Vector2(m_InputManager.Horizontal(), m_InputManager.Vertical());
            m_Animator.SetFloat("speed", m_Input.magnitude);

            m_Animator.SetBool("run", m_InputManager.PressSpeedUpButton());
        }
    }
}
