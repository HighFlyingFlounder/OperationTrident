using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class AIStateParam
    {
        private Hashtable _massData = new Hashtable();

        /// <summary>
        /// 将需要在状态之间传递的数据放进Hashtable中
        /// </summary>
        /// <param name="key">用于在Hashtable中索引数据，需要唯一</param>
        /// <param name="value">需要传递的具体数据</param>
        /// <returns>
        ///     true: 说明key是第一次添加
        ///     false: 说明Hashtable中已有key，此时原本的value被覆盖
        /// </returns> 
        public bool SetMassData(string key, object value)
        {
            if (_massData.Contains(key))
            {
                _massData[key] = value;
                return false;
            }

            _massData.Add(key, value);
            return true;
        }

        /// <summary>
        /// 在Hashtable中根据key来获取数据
        /// </summary>
        /// <param name="key">在Hashtable中的唯一索引</param>
        /// <returns>
        ///     object: 说明Hashtable中存在key对应的数据
        ///     null: 说明Hashtable中不存在key
        /// </returns> 
        public object GetMassData(string key)
        {
            if (_massData.ContainsKey(key))
            {
                return _massData[key];
            }
            else
            {
                return null;
            }
        }
    }

    public class AIStateParamParserBase
    {
        protected AIStateParam _param;
        public AIStateParamParserBase(AIStateParam param)
        {
            _param = param;
        }
    }
}
