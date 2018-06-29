using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class AIStateRegisterWindow : EditorWindow
    {
        Editor editor;

        [MenuItem("Window/AI State Register")]
        static void Init()
        {
            AIStateRegisterWindow window = (AIStateRegisterWindow)EditorWindow.GetWindow(typeof(AIStateRegisterWindow), false, "State Register", true);
            window.autoRepaintOnSceneChange = true;
            window.Show();
        }

        private void OnEnable()
        {
            editor = Editor.CreateEditor(Utility.GetAIStateRegister());
        }
        private void OnGUI()
        {
            editor.OnInspectorGUI();
        }
    }
}
