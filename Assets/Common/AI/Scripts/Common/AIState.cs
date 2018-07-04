using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{

    public abstract class AIState : MonoBehaviour
    {
        // 判断是不是第一次初始化
        bool _firstInit;

        protected bool IsFirstInit
        {
            get
            {
                if (_firstInit)
                {
                    _firstInit = false;
                    return true;
                }
                return false;
            }
        }

        // 获取AIAgent，用于获取其中的变量
        protected AIAgent _agent;

        protected void Awake()
        {
            _firstInit = true;
            _agent = GetComponent<AIAgent>();
        }

        /// <summary>
        /// 初始化状态
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 运行当前状态
        /// </summary>
        /// <returns>返回状态转移已满足的条件，若为null，则不满足状态条件</returns>
        public abstract string Execute();

        /// <summary>
        /// 退出当前状态
        /// </summary>
        public virtual void Exit()
        {
        }
    }
}