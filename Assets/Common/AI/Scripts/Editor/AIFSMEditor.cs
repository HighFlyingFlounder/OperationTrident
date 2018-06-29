using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace OperationTrident.Common.AI
{
    [CustomEditor(typeof(AIFSM), true)]
    public class AIFSMEditor : Editor
    {
        // 两个按钮
        static GUIContent addButtonContent = new GUIContent("+", "添加状态到当前状态机");
        static GUIContent deleteButtonContent = new GUIContent("-", "删除状态");

        // 获取当前正在编辑的FSM
        AIFSM _currentFSM;

        // 当前FSM用到的状态
        List<string> _stateInFSM;

        // 获取状态注册器
        AIStateRegister _stateRegister;

        void OnEnable()
        {
            _currentFSM = target as AIFSM;
            _stateRegister = Utility.GetAIStateRegister();
        }

        public override void OnInspectorGUI()
        {
            // NULL用于取消某个状态转移条件
            _stateInFSM = new List<string> { "NULL" };
            foreach (var node in _currentFSM.stateTransitionGraphNodes)
            {
                _stateInFSM.Add(node.currentState);
            }
            // 从AIStateRegister获取所有已经注册的状态
            List<string> registeredStates = _stateRegister.GetStates();
            // 删除AIStateRegister中没有的状态
            DeleteStateFromFSM(registeredStates);

            // 折叠FSM标签，默认打开
            _currentFSM.showStateTransitionGraph = EditorGUILayout.Foldout(_currentFSM.showStateTransitionGraph, _currentFSM.GetType().Name.ToString(), true);
            // 获取当前FSM中已有的状态，将NULL移除
            List<string> states = _stateInFSM.GetRange(1, _stateInFSM.Count - 1);
            if (_currentFSM.showStateTransitionGraph)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("States", GUILayout.Width(50f));
                // 从所有已经注册的状态里选择状态添加到当前FSM
                _currentFSM.addStateIndex = EditorGUILayout.Popup(_currentFSM.addStateIndex, registeredStates.ToArray());
                // 当动态从AIStateRegister中删除状态时，索引可能超出范围
                _currentFSM.addStateIndex = _currentFSM.addStateIndex >= registeredStates.Count ? 0 : _currentFSM.addStateIndex;
                // 点击添加按钮时添加状态
                if (GUILayout.Button(addButtonContent, EditorStyles.miniButton, GUILayout.Width(20f))
                     && _currentFSM.addStateIndex < registeredStates.Count)
                {
                    AddStateToFSM(registeredStates[_currentFSM.addStateIndex]);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                EditorGUI.indentLevel++; // 缩进

                // 显示状态机中_stateTransitionGraph的每个单独节点
                for (int i = 0; i < _currentFSM.stateTransitionGraphNodes.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    // 是否折叠状态，默认打开
                    _currentFSM.showStateTransitionConditions[i] = EditorGUILayout.Foldout(
                        _currentFSM.showStateTransitionConditions[i],
                        _currentFSM.stateTransitionGraphNodes[i].currentState,
                        true
                    );
                    // 点击删除状态按钮时，删除当前状态
                    if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButton, GUILayout.Width(20f)))
                    {
                        DeleteStateFromFSM(i);
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();

                    UpdateConditionInState(i);

                    if (_currentFSM.showStateTransitionConditions[i])
                    {
                        EditorGUI.indentLevel++; // 缩进

                        // 显示该状态的所有转移条件，用于添加满足该条件后跳转的下一个状态
                        for (int j = 0; j < _currentFSM.stateTransitionGraphNodes[i].stateTransitionRules.Count; j++)
                        {
                            string condition = _currentFSM.stateTransitionGraphNodes[i].stateTransitionRules[j].condition;
                            string nextState = _currentFSM.stateTransitionGraphNodes[i].stateTransitionRules[j].nextState;
                            EditorGUILayout.BeginHorizontal();
                            // 当前状态可以返回的条件
                            EditorGUILayout.LabelField(condition);
                            int index = _stateInFSM.IndexOf(nextState);
                            // 当找不到该状态时，置为NULL
                            index = index == -1 ? 0 : index;

                            // 设置满足条件后的下一个状态
                            index = EditorGUILayout.Popup(index, _stateInFSM.ToArray());
                            AIFSM.StateTransitionGraphNode.StateTransitionRule rule;
                            rule.condition = condition;
                            rule.nextState = index == 0 ? null : _stateInFSM[index];
                            _currentFSM.stateTransitionGraphNodes[i].stateTransitionRules[j] = rule;

                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel--; // 取消缩进
                    }
                }
                EditorGUI.indentLevel--; // 取消缩进
            }

            EditorGUILayout.Space();

            // 设置初始化状态
            _currentFSM.initStateIndex = EditorGUILayout.Popup("Initial State", _currentFSM.initStateIndex, states.ToArray());
            _currentFSM.initStateIndex = _currentFSM.initStateIndex < states.Count ? _currentFSM.initStateIndex : 0;
            // 当前状态机中没有状态时，设置为null
            string lastinitStateName = _currentFSM.initStateName;
            _currentFSM.initStateName = states.Count > 0 ? states[_currentFSM.initStateIndex] : null;

            // 当编辑器中某个字段发生改变时，保存更改后的数据
            if(GUI.changed)
                EditorUtility.SetDirty(_currentFSM);
        }

        void AddStateToFSM(string stateName)
        {
            // 判断当前状态机是否有该状态
            if (_stateInFSM.IndexOf(stateName) != -1)
                return;

            // 获取该状态能返回的所有条件
            FieldInfo[] fields = _stateRegister.GetStateType(stateName).GetNestedType("Conditions").GetFields();

            // 初始化状态节点
            AIFSM.StateTransitionGraphNode node;
            node.currentState = stateName;
            node.stateTransitionRules = new List<AIFSM.StateTransitionGraphNode.StateTransitionRule>();

            for (int i = 0; i < fields.Length; i++)
            {
                AIFSM.StateTransitionGraphNode.StateTransitionRule rule;
                rule.condition = (string)fields[i].GetValue(null);
                rule.nextState = null;
                node.stateTransitionRules.Add(rule);
            }

            //在当前状态机注册状态
            _currentFSM.stateTransitionGraphNodes.Add(node);

            // 初始化Editor中与该状态相关的变量
            _currentFSM.showStateTransitionConditions.Add(true);
            _stateInFSM.Add(stateName);
        }

        void DeleteStateFromFSM(List<string> registeredStates)
        {
            List<string> currentStatesInFSM = _stateInFSM.GetRange(1, _stateInFSM.Count - 1);

            // 删除状态机里有，但状态注册机里没有的状态
            for (int i = 0; i < currentStatesInFSM.Count; i++)
            {
                if (registeredStates.IndexOf(currentStatesInFSM[i]) == -1)
                {
                    DeleteStateFromFSM(i);
                }
            }
        }

        void DeleteStateFromFSM(int stateIndex)
        {
            // 判断当前状态机是否有该状态
            if (stateIndex >= _currentFSM.stateTransitionGraphNodes.Count || stateIndex < 0)
                return;

            // 从状态机删除状态
            _currentFSM.stateTransitionGraphNodes.RemoveAt(stateIndex);

            // 将Editor中与该状态相关的变量删除
            _stateInFSM.RemoveAt(stateIndex + 1);
            _currentFSM.showStateTransitionConditions.RemoveAt(stateIndex);
        }

        void UpdateConditionInState(int stateIndex)
        {
            AIFSM.StateTransitionGraphNode node = _currentFSM.stateTransitionGraphNodes[stateIndex];

            FieldInfo[] fields = _stateRegister.GetStateType(node.currentState).GetNestedType("Conditions").GetFields();
            List<string> conditions = new List<string>();
            foreach (var info in fields)
            {
                conditions.Add((string)info.GetValue(null));
            }

            List<AIFSM.StateTransitionGraphNode.StateTransitionRule> rules = node.stateTransitionRules;

            for (int i = 0; i < rules.Count; i++)
            {
                int index = conditions.IndexOf(rules[i].condition);
                if (index != -1)
                    conditions.RemoveAt(index);
                else
                    _currentFSM.stateTransitionGraphNodes[stateIndex].stateTransitionRules.RemoveAt(i);
            }

            foreach (var condition in conditions)
            {
                AIFSM.StateTransitionGraphNode.StateTransitionRule rule;
                rule.condition = condition;
                rule.nextState = null;
                _currentFSM.stateTransitionGraphNodes[stateIndex].stateTransitionRules.Add(rule);
            }
        }
    }
}