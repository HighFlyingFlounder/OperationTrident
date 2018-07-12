using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [CreateAssetMenu(menuName = "AI/FSM Data")]
    public class AIFSMData : ScriptableObject
    {
#if UNITY_EDITOR
        public bool showStateTransitionGraph = true;
        public int addStateIndex = 0;
        public List<bool> showStateTransitionConditions = new List<bool>();
        public int initStateIndex = 0;
        public bool showInitParams = true;
        public string assetPath = null;
#endif

        // 用于设置状态转移图
        [System.Serializable]
        public struct StateTransitionGraphNode
        {
            [System.Serializable]
            public struct StateTransitionRule
            {
                public string condition;
                public string nextState;
            };

            public string currentState;
            public List<StateTransitionRule> stateTransitionRules;
        }

        public List<StateTransitionGraphNode> stateTransitionGraphNodes = new List<StateTransitionGraphNode>();
        public string initStateName = null;

        Dictionary<string, Dictionary<string, string>> _stateTransitionGraph;

        public Dictionary<string, Dictionary<string, string>> StateTransitionGraph
        {
            get
            {
                return _stateTransitionGraph;
            }
        }

        void OnEnable()
        {
            // 将序列化的结构数组转换为Dictionary，方便之后进行索引
            if (stateTransitionGraphNodes != null)
            {
                _stateTransitionGraph = new Dictionary<string, Dictionary<string, string>>();

                foreach (var node in stateTransitionGraphNodes)
                {
                    if (node.stateTransitionRules != null)
                    {
                        Dictionary<string, string> rules = new Dictionary<string, string>();
                        foreach (var rule in node.stateTransitionRules)
                        {
                            rules.Add(rule.condition, rule.nextState);
                        }

                        _stateTransitionGraph.Add(node.currentState, rules);
                    }
                }
            }
        }
        
    }
}