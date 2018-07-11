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
        Transform _center = null;
        
        public override float DepressionAngle
        {
            get
            {
                return transform.GetComponent<Forge3D.F3DTurret>().ElevationLimit.y;
            }
        }
        public override float DetectRangeMax
        {
            get
            {
                return _initParams.detectRangeMax;
            }
        }
        public override Transform Center 
        {
            get
            {
                if(_center == null)
                {
                    _center = transform.Find("center").transform;
                }
                return _center;
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
        [Tooltip("设置检测范围最大半径")]
        [Range(1, 100)]
        public float detectRangeMax;
    }
}