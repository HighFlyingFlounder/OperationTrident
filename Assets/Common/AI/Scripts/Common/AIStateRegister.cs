using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Reflection;

namespace OperationTrident.Common.AI
{
    [CreateAssetMenu(menuName = "AI/State Register")]
    public class AIStateRegister : ScriptableObject
    {
        [SerializeField]
        UnityEngine.Object[] AIStateScripts;
        Dictionary<string, Type> AIStateDictionary;

        void OnEnable()
        {
            BuildDictionary();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            BuildDictionary();
        }

        private void Reset()
        {
            AIStateDictionary = new Dictionary<string, Type>();
        }
#endif

        public List<string> GetStates()
        {
            return new List<string>(AIStateDictionary.Keys);
        }

        public Type GetStateType(string stateName)
        {
            if (AIStateDictionary.ContainsKey(stateName))
                return AIStateDictionary[stateName];
            return null;
        }

        void BuildDictionary()
        {
            AIStateDictionary = new Dictionary<string, Type>();

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
                    AIStateDictionary.Add(stateName, stateType);
                }
            }
        }
    }
}