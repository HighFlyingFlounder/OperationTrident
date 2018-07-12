using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class StandAttackState : AIState
    {
        public static readonly string STATE_NAME = "Stand Attack";

        public class Conditions
        {
            public static readonly string LOST_TARGET = "Lost Target";
        }

        public override void Init()
        {
            _agent.ActionController.RPC(_agent.ActionController.LookAtWithTargetName, _agent.Target.parent.name);
            _agent.ActionController.RPC(_agent.ActionController.Shoot);
            // _agent.ActionController.LookAt(_agent.Target.name);
            // _agent.ActionController.Shoot();
        }

        public override string Execute()
        {
            if(!_agent.ActionController.DetectPlayer(_agent.Target))
                return Conditions.LOST_TARGET;
            
            return null;
        }

        public override void Exit()
        {
            // _agent.ActionController.RPC(_agent.ActionController.StopShoot);
            // _agent.ActionController.RPC(_agent.ActionController.StopLookAt);
            _agent.ActionController.StopShoot();
            _agent.ActionController.StopLookAt();
        }
    }
}