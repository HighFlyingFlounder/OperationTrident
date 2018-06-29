using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    // [DisallowMultipleComponent]
    [CreateAssetMenu]
    public class AIFSM : ScriptableObject
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
        string _currStateName = null;
        AIState _currState = null;
        AIStateRegister _stateRegister = null;
        GameObject _gameObject = null;

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
            _stateRegister = Utility.GetAIStateRegister();
        }

        public void Init(GameObject gameObject, AIState.InitParamsBase initParams)
        {
            _gameObject = gameObject;
            // 在FSM所在GameObject添加状态脚本
            if (_stateTransitionGraph != null)
            {
                foreach (var node in _stateTransitionGraph)
                {
                    Type state = _stateRegister.GetStateType(node.Key);
                    if (state != null && !_gameObject.GetComponent(state))
                    {
                        _gameObject.AddComponent(state);
                    }
                }
            }

            // 设置当前状态，并初始化
            Type initStateType = _stateRegister.GetStateType(initStateName);
            if (initStateType != null)
            {
                _currState = (_gameObject.GetComponent(initStateType) as AIState);
                _currStateName = initStateName;

                AIStateParam stateParams = new AIStateParam();
                FieldInfo[] fieldInfos = initParams.GetType().GetFields();
                foreach (var info in fieldInfos)
                {
                    stateParams.SetMassData(info.Name, info.GetValue(initParams));
                }
                _currState.Init(stateParams);
            }
        }

        public void Update()
        {
            if (_currState == null)
                return;

            // 执行当前状态的更新
            string condition = _currState.Execute();
            if (condition != null)
            {
                AIState nextState = GetNextState(_currStateName, condition);
                if (nextState != null)
                {
                    // 进行状态转移
                    nextState.Init(_currState.Transition());
                    _currState = nextState;
                    _currStateName = _stateTransitionGraph[_currStateName][condition];
                }
            }
        }

        AIState GetNextState(string currentState, string condition)
        {
            string nextState = _stateTransitionGraph[currentState][condition];
            Type nextStateScriptType = _stateRegister.GetStateType(nextState);
            return nextStateScriptType == null ? null : _gameObject.GetComponent(nextStateScriptType) as AIState;
        }
    }
}
