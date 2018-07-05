using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class TurretAIAgent : AIAgent
    {

        [Header("初始化参数")]
        [Tooltip("设置检测范围最小半径")]
        [SerializeField]
        [Range(10, 30)]
        float _detectRangeMin;

        [Tooltip("设置检测范围最大半径")]
        [SerializeField]
        [Range(50, 100)]
        float _detectRangeMax;

        public override float DetectRangeMin
        {
            get
            {
                return _detectRangeMin;
            }
        }
        public override float DetectRangeMax
        {
            get
            {
                return _detectRangeMax;
            }
        }
    }
}