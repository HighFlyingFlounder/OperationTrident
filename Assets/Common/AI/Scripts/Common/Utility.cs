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

        public static void CreateFSMInitParamsAsset(AIState.InitParamsBase initParams, string assetPath)
        {
            AssetDatabase.CreateAsset(initParams, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void UpdateFSMInitParamsAssetPath(string currentPath, string newPath)
        {
            string error = AssetDatabase.RenameAsset(currentPath, newPath);
            if (error != "")
                Debug.Log(error);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static string GetFSMInitParamsAssetPath(string FSMAssetPath)
        {
            return FSMAssetPath.Substring(0, FSMAssetPath.LastIndexOf('.')) + "InitParams.asset";
        }

        public static string GetNewFSMInitParamsAssetName(string FSMAssetPath)
        {
            int startIndex = FSMAssetPath.LastIndexOf('/') + 1;
            int length = FSMAssetPath.LastIndexOf('.') - startIndex;
            return FSMAssetPath.Substring(startIndex, length) + "InitParams";
        }
#endif
    }
}
