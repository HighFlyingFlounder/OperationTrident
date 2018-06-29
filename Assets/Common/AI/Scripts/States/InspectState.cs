using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class InspectState : AIState
    {
        public static readonly string STATE_NAME = "Inspect";

        public class Conditions
        {
            public static readonly string SIGHT_PLAYER = "Sight Player";
            public static readonly string FINISH_INSPECTION = "Finish Inspection";
        }

		public override void Init(AIStateParam param)
        {
            base.Init(param);
            transform.GetComponent<Animator>().SetFloat("Speed", -1f);
			StartCoroutine(FindPlayer());
        }

        public override string Execute()
        {
			return _satisfy;
        }

        IEnumerator FindPlayer()
        {
            yield return new WaitForSeconds(8f);
			_satisfy = Conditions.FINISH_INSPECTION;
        }
    }
}
