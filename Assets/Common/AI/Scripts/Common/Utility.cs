using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OperationTrident.Common.AI
{
    public static class Utility
    {
        public static Type GetType(string typeName)
        {
            return Type.GetType("OperationTrident.Common.AI." + typeName);
        }

        public static AIStateRegister GetAIStateRegister()
        {
            return AssetDatabase.LoadAssetAtPath(AIStateRegister.assetPath, typeof(AIStateRegister)) as AIStateRegister;
        }

        public static string GetAssetPath(ScriptableObject obj)
        {
            return AssetDatabase.GetAssetPath(obj);
        }

#if UNITY_EDITOR
        public static bool CanDrawEditor()
        {
            return Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint;
        }
#endif
    }
}
