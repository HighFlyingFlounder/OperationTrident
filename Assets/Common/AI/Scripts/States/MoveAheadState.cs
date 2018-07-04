using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class MoveAheadState : AIState
    {
		public static readonly string STATE_NAME = "Move Ahead";

        public class Conditions
        {
            public static readonly string ARRIVE_AT_LOCATION = "Arrive at location";
            public static readonly string SIGHT_PLAYER = "Sight Player";
        }

        Animator _animator;
		float _remainDistance;

        public override void Init()
        {
			// 设置寻路目标点
            _agent.PathfindingAgent.SetDestination(_agent.TargetPosition);
            _agent.PathfindingAgent.isStopped = false;
            _animator = GetComponent<Animator>();
			_animator.SetFloat("Speed", 1);
        }

        public override string Execute()
        {
			Transform target = Utility.DetectPlayers(_agent.Camera);
            if(target != null)
            {
                _agent.Target = target;
                return Conditions.SIGHT_PLAYER;
            }

			if (!_agent.PathfindingAgent.pathPending)
            {
                _remainDistance = _agent.PathfindingAgent.remainingDistance;
                if (_remainDistance != Mathf.Infinity && _remainDistance - _agent.PathfindingAgent.stoppingDistance <= float.Epsilon
                && _agent.PathfindingAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    return Conditions.ARRIVE_AT_LOCATION;
                }
            }
			return null;
        }

		public override void Exit()
		{
            _agent.PathfindingAgent.isStopped = true;
			_animator.SetFloat("Speed", -1);
		}


    }
}
