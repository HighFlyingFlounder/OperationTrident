using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    
    public abstract class AIState : MonoBehaviour
    {
        // 当前状态下满足的转移条件，若为null，则不转移状态
        protected string _satisfy = null;
        
        // 判断是不是第一次初始化
        protected bool _firstInit = true;

        /// <summary>
        /// 初始化状态
        /// </summary>
        public virtual void Init()
        {
            _satisfy = null;
        }

        /// <summary>
        /// 用于只需初始化一次的数据
        /// </summary>
        public virtual void InitOnce()
        {
            _firstInit = false;
        }

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