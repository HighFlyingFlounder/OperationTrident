using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class IdleState : AIState
    {
		public static readonly string STATE_NAME = "Idle";

        public class Conditions
        {
            public static readonly string UNCONDITIONAL = "Unconditional";
        }

        public override string Execute()
        {
			Debug.Log("Idle");
			_satisfy = Conditions.UNCONDITIONAL;
			return _satisfy;
        }
    }
}
