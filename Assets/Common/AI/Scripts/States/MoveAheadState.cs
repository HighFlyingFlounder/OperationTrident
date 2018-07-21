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

        float _remainDistance;

        public override void Init()
        {
            // 设置寻路目标点
            _agent.PathfindingAgent.SetDestination(_agent.TargetPosition);
            _agent.PathfindingAgent.isStopped = false;
            _agent.ActionController.RPC(_agent.ActionController.Move, true);
        }

        public override string Execute()
        {
            // 当AI不在Link上时才检测玩家，不然会在半空中发现玩家改变状态
            if (!_agent.PathfindingAgent.isOnOffMeshLink)
            {
                _agent.Target = Utility.DetectAllPlayersWithCamera(_agent.Camera);
                if (_agent.Target != null)
                {
                    return Conditions.SIGHT_PLAYER;
                }
            }

            if (!_agent.PathfindingAgent.pathPending)
            {
                _remainDistance = _agent.PathfindingAgent.remainingDistance;
                if (_remainDistance != Mathf.Infinity && _remainDistance - _agent.PathfindingAgent.stoppingDistance <= float.Epsilon
                && _agent.PathfindingAgent.pathStatus == NavMeshPathStatus.PathComplete || _agent.PathfindingAgent.velocity == Vector3.zero)
                {
                    return Conditions.ARRIVE_AT_LOCATION;
                }
            }
            return null;
        }

        public override void Exit()
        {
            _agent.PathfindingAgent.isStopped = true;
            _agent.ActionController.RPC(_agent.ActionController.Move, false);
        }
    }
}
