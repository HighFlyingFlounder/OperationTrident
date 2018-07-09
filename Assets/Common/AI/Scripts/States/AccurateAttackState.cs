using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class AccurateAttackState : AIState
    {
        public static readonly string STATE_NAME = "Accurate Attack";

        public class Conditions
        {
            public static readonly string FINISH_ONCE_SHOOT = "Finish Once Shoot";
        }

        public override void Init()
        {
        }

        public override string Execute()
        {
            if (_agent.Target != null)
            {
                _agent.ActionController.LookAt(_agent.Target.position);
                _agent.ActionController.Shoot(_agent.Target.position);
            }
            return Conditions.FINISH_ONCE_SHOOT;
        }

        public override void Exit()
        {
        }


    }
}