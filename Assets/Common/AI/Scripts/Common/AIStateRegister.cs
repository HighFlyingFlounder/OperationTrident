using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace OperationTrident.Common.AI
{
    [CreateAssetMenu]
    public class AIStateRegister : ScriptableObject
    {
        [SerializeField]
        MonoScript[] AIStateScripts;
        Dictionary<string, MonoScript> AIStateDictionary;
        public static string assetPath = "Assets/Common/AI/Scripts/States/AIStateRegister.asset";

        void OnEnable()
        {
            BuildDictionary();
        }

        void OnValidate()
        {
            BuildDictionary();
        }

        private void Reset()
        {
            AIStateDictionary = new Dictionary<string, MonoScript>();
        }

        public List<string> GetStates()
        {
            return new List<string>(AIStateDictionary.Keys);
        }

        public Type GetStateType(string stateName)
        {
            if (AIStateDictionary.ContainsKey(stateName))
                return AIStateDictionary[stateName].GetClass();
            return null;
        }

        void BuildDictionary()
        {
            AIStateDictionary = new Dictionary<string, MonoScript>();

            if (AIStateScripts != null)
            {
                foreach (var script in AIStateScripts)
                {
                    if (script == null)
                        continue;

                    Type stateType = Utility.GetType(script.name);
                    if (stateType == null)
                    {
                        Debug.Log(script.name + "中类名与文件名不符");
                        continue;
                    }
                    
                    if (!stateType.IsSubclassOf(Utility.GetType("AIState")))
                    {
                        Debug.Log(script.name + "不是一个AIState");
                        continue;
                    }

                    FieldInfo stateNameField = stateType.GetField("STATE_NAME");
                    if (stateNameField == null)
                    {
                        Debug.Log(script.name + "没有状态名");
                        continue;
                    }

                    string stateName = (string)(stateNameField.GetValue(null));
                    if (AIStateDictionary.ContainsKey(stateName))
                    {
                        Debug.Log(stateName + "已存在");
                        continue;
                    }
                    AIStateDictionary.Add(stateName, script);
                }
            }
        }
    }
}