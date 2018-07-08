using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class AlertState : AIState
    {
        public static readonly string STATE_NAME = "Alert";

        public class Conditions
        {
            public static readonly string SIGHT_PLAYER = "Sight Player";
            public static readonly string LOST_PLAYER = "Lost Player";
        }

        public override void Init()
        {
            _agent.ActionController.RPC(_agent.ActionController.DetectedTarget, true);
            //_agent.ActionController.DetectedTarget(true);
        }

        public override string Execute()
        {
            _agent.ActionController.RPC(_agent.ActionController.LookAt, _agent.Target.position);
            //_agent.ActionController.LookAt(_agent.Target.position);

            _agent.Camera.UpdateCamera();
            if (_agent.Camera.DetectTarget(_agent.Target))
            {
                return Conditions.SIGHT_PLAYER;
            }
            else
            {
                return Conditions.LOST_PLAYER;
            }
        }

        public override void Exit()
        {
            _agent.ActionController.RPC(_agent.ActionController.DetectedTarget, false);
            //_agent.ActionController.DetectedTarget(false);
        }
    }
}