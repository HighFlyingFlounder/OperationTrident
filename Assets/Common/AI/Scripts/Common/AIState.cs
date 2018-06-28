using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    
    public abstract class AIState : MonoBehaviour
    {
        [System.Serializable]
        public class InitParamsBase : ScriptableObject {}

        // 当前状态下满足的转移条件，若为null，则不转移状态
        protected string _satisfy = null;
        // 设置当前状态可以获得的参数，用于状态之间的参数传递
        protected AIStateParam _params = null;
        // 解析由另一个状态传入的参数的解析器
        protected AIStateParamParserBase _paramParser = null;

        /// <summary>
        ///     初始化状态，其中_paramParser需要在子类设置特定的解析器
        /// </summary>
        /// <param name="param">由另一个状态传入的参数</param>
        public virtual void Init(AIStateParam param)
        {
            _satisfy = null;
            _params = new AIStateParam();
            _paramParser = null;
        }

        /// <summary>
        ///     运行当前状态
        /// </summary>
        /// <returns>返回状态转移已满足的条件，若为null，则不满足状态条件</returns>
        public abstract string Execute();

        /// <summary>
        ///     获取当前状态已设置好的参数
        /// </summary>
        public AIStateParam Transition()
        {
            return _params;
        }
    }
}