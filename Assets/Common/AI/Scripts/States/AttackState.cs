using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class AttackState : AIState
    {
        public static readonly string STATE_NAME = "Attack";

        public class Conditions
        {
            public static readonly string LOSE_TARGET = "Lose Target";
        }

        public override void Init()
        {
            base.Init();
        }
        public override string Execute()
        {
            // Debug.Log("Attack player in " + (_paramParser as AttackStateParamParser).TargetPosition);

            // Ray ray = new Ray(transform.position, transform.forward);
            // RaycastHit hit;

            // if (Physics.SphereCast(ray, 0.75f, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Map") | 1 << LayerMask.NameToLayer("Players")))
            // {
            //     GameObject hitObj = hit.transform.gameObject;
            //     if (hitObj.GetComponent<PlayerCharacter>() == null)
            //     {
            //         _satisfy = Conditions.LOSE_TARGET;
            //     }
            // }

            return _satisfy;
        }
    }
}
