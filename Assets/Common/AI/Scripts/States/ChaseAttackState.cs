using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class ChaseAttackState : AIState
    {
        public static readonly string STATE_NAME = "Chase Attack";


        public class Conditions
        {
            public static readonly string LOST_TARGET = "Lost Target";
            public static readonly string TARGET_DOWN = "Target Down";
        }

        float _precisionAngle;
        float _precisionRadius;
        bool _isShooting;

        public override void Init()
        {
            if (IsFirstInit)
            {
                _precisionAngle = _agent.AttackPrecisionAngle;
                _precisionRadius = _agent.AttackPrecisionRadius;
            }
            _isShooting = false;
            _agent.ActionController.RPC(_agent.ActionController.LookAtWithTargetName, _agent.Target.parent.name);
        }

        public override string Execute()
        {
            string satisfy = null;
            if (_agent.Target == null)
            {
                return Conditions.TARGET_DOWN;
            }

            _agent.Camera.UpdateCamera();

            Vector3 shootingPoint;
            // 先判断是否在射击范围内，若不在，先追击敌人，等敌人进入射击范围后，开始射击
            if (!_agent.Camera.GetShootPoint(_precisionAngle, _precisionRadius, _agent.Target.position, out shootingPoint))
            {
                _agent.PathfindingAgent.SetDestination(_agent.Target.position);
                _agent.PathfindingAgent.isStopped = false;
                if (_isShooting)
                {
                    _agent.ActionController.RPC(_agent.ActionController.StopShoot);
                    _isShooting = false;
                }
                _agent.ActionController.RPC(_agent.ActionController.Move, true);
            }
            else if (!_agent.PathfindingAgent.isOnOffMeshLink && !_isShooting)
            {
                _isShooting = true;
                _agent.ActionController.RPC(_agent.ActionController.Shoot);
                _agent.PathfindingAgent.isStopped = true;
                _agent.ActionController.RPC(_agent.ActionController.Move, false);
            }

            // 敌人可能已死亡或躲到障碍后
            if (!_agent.Camera.DetectTarget(_agent.Target))
            {
                _agent.TargetPosition = _agent.Target.position;
#if UNITY_EDITOR
                _agent.Camera.DrawDefaultAttackPrecisionRange();
#endif
                return Conditions.LOST_TARGET;
            }

            return satisfy;
        }

        public override void Exit()
        {
            _agent.PathfindingAgent.isStopped = true;
            if (_isShooting)
            {
                _agent.ActionController.RPC(_agent.ActionController.StopShoot);
            }
            _agent.ActionController.RPC(_agent.ActionController.StopLookAt);
            _agent.ActionController.RPC(_agent.ActionController.Move, false);
        }
    }
}
