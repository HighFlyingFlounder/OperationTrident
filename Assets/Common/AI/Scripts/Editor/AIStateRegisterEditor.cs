using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [CustomEditor(typeof(AIStateRegister))]
    public class AIStateRegisterEditor : Editor
    {
        AIStateRegister _register;
        bool showInfo = true;

        void OnEnable()
        {
            _register = target as AIStateRegister;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            List<string> states = _register.GetStates();
            showInfo = EditorGUILayout.Foldout(showInfo, "Registered State [" + states.Count + "]:", true);
            if (showInfo && Utility.CanDrawEditor())
            {
                EditorGUI.indentLevel++; // 缩进
                foreach (var state in states)
                {
                    EditorGUILayout.LabelField(state);
                }
                EditorGUI.indentLevel--; // 取消缩进
            }
        }
    }
}
