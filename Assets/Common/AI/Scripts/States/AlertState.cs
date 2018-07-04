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
        Animator _animator;

        public override void Init()
        {
            if (IsFirstInit)
            {
                _animator = GetComponent<Animator>();
            }
            _animator.SetBool("TargetDetected", true);
        }

        public override string Execute()
        {
            transform.forward = Utility.GetDirectionOnXOZ(transform.position, _agent.Target.position);

            _agent.Camera.UpdateCamera();
            if (_agent.Camera.DetectTarget(_agent.Target))
            {
                return Conditions.SIGHT_PLAYER;
            }
            else
            {
                return Conditions.LOST_PLAYER;
            }

            // return _satisfy;
        }

        public override void Exit()
        {
            _animator.SetBool("TargetDetected", false);
        }
    }
}