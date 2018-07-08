using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class TurretAIAgent : AIAgent
    {
        [SerializeField]
        TurretAIAgentInitParams _initParams;
        

        public override float DetectRangeMin
        {
            get
            {
                return _initParams.detectRangeMin;
            }
        }
        public override float DetectRangeMax
        {
            get
            {
                return _initParams.detectRangeMax;
            }
        }

        public override void SetInitParams(AIAgentInitParams initParams)
        {
            _initParams = (TurretAIAgentInitParams)initParams;
        }
    }

    [System.Serializable]
    public class TurretAIAgentInitParams : AIAgentInitParams
    {
        [Tooltip("设置检测范围最小半径")]
        [Range(10, 30)]
        public float detectRangeMin;

        [Tooltip("设置检测范围最大半径")]
        [Range(50, 100)]
        public float detectRangeMax;
    }
}