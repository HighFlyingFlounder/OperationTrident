using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class AIFSM
    {
        string _currStateName = null;
        AIState _currState = null;
        AIStateRegister _stateRegister = null;
        GameObject _gameObject = null;
        AIFSMData _fsm = null;

        public AIFSMData FSMData
        {
            set
            {
                _fsm = value;
            }
        }

        public void Init(GameObject gameObject)
        {
            _gameObject = gameObject;
            _stateRegister = Utility.GetAIStateRegister();
            
            // 在FSM所在GameObject添加状态脚本
            if (_fsm.StateTransitionGraph != null)
            {
                foreach (var node in _fsm.StateTransitionGraph)
                {
                    Type state = _stateRegister.GetStateType(node.Key);
                    if (state != null && !_gameObject.GetComponent(state))
                    {
                        _gameObject.AddComponent(state);
                    }
                }
            }

            // 设置当前状态，并初始化
            Type initStateType = _stateRegister.GetStateType(_fsm.initStateName);
            if (initStateType != null)
            {
                _currState = (_gameObject.GetComponent(initStateType) as AIState);
                _currStateName = _fsm.initStateName;
                _currState.Init();
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
                    nextState.Init();
                    _currState = nextState;
                    _currStateName = _fsm.StateTransitionGraph[_currStateName][condition];
                }
            }
        }

        AIState GetNextState(string currentState, string condition)
        {
            string nextState = _fsm.StateTransitionGraph[currentState][condition];
            Type nextStateScriptType = _stateRegister.GetStateType(nextState);
            return nextStateScriptType == null ? null : _gameObject.GetComponent(nextStateScriptType) as AIState;
        }
    }
}
