using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    public class InaccurateAttackState : AIState
    {
        public static readonly string STATE_NAME = "Inaccurate Attack";


        public class Conditions
        {
            public static readonly string LOST_TARGET = "Lost Target";
            public static readonly string FINISH_ONCE_SHOOT = "Finish Once Shoot";
        }

        float _precisionAngle;
        float _precisionRadius;

        public override void Init()
        {
            if (IsFirstInit)
            {
                _precisionAngle = _agent.AttackPrecisionAngle;
                _precisionRadius = _agent.AttackPrecisionRadius;
            }
        }

        public override string Execute()
        {
            string satisfy = null;
            if(_agent.Target == null)
            {
                return Conditions.LOST_TARGET;
            }
            _agent.ActionController.LookAt(_agent.Target.position);

            _agent.Camera.UpdateCamera();

            Vector3 shootingPoint;
            // 先判断是否在射击范围内，若不在，先追击敌人，等敌人进入射击范围后，开始射击
            if(!_agent.Camera.GetShootPoint(_precisionAngle, _precisionRadius, _agent.Target.position, out shootingPoint))
            {
                _agent.PathfindingAgent.SetDestination(_agent.Target.position);
                _agent.PathfindingAgent.isStopped = false;
                _agent.ActionController.Move(true);
            }
            else
            {
                _agent.ActionController.Shoot(shootingPoint);
			    _agent.ActionController.Move(false);
                satisfy = Conditions.FINISH_ONCE_SHOOT;
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
            _agent.ActionController.Move(false);
		}
    }
}
