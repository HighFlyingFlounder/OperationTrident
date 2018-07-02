using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class WanderAIAgent : AIAgent
    {
		public PatrolState.InitParams initParams;

        void Awake()
		{
			_initParams = initParams;
		}
    }
}
