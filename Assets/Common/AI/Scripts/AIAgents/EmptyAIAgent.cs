
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class EmptyAIAgent : AIAgent
    {
        [SerializeField]
        TurretAIAgentInitParams _initParams;
        
        public override void SetInitParams(AIAgentInitParams initParams)
        {
            _initParams = (TurretAIAgentInitParams)initParams;
        }
    }

    [System.Serializable]
    public class EmptyAIAgentInitParams : AIAgentInitParams
    {
    }
}