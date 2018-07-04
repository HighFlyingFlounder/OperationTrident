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
            transform.forward = Utility.GetDirectionOnXOZ(transform.position, _agent.Target.position);

            _agent.Camera.UpdateCamera();

            Vector3 shootPoint;
            // 先判断是否在射击范围内，若不在，先追击敌人，等敌人进入射击范围后，开始射击
            if(!_agent.Camera.GetShootPoint(_precisionAngle, _precisionRadius, _agent.Target.position, out shootPoint))
            {
                _agent.PathfindingAgent.SetDestination(_agent.Target.position);
                _agent.PathfindingAgent.isStopped = false;
            }
            else
            {
                _agent.PathfindingAgent.isStopped = true;
                GetComponent<TestShoot>().Shoot(shootPoint);
                return Conditions.FINISH_ONCE_SHOOT;
            }

            // 敌人可能已死亡或躲到障碍后
            if (!_agent.Camera.DetectTarget(_agent.Target))
            {
                _agent.PathfindingAgent.isStopped = true;
#if UNITY_EDITOR
                _agent.Camera.DrawDefaultAttackPrecisionRange();
#endif
                return Conditions.LOST_TARGET;
            }

            return null;
        }
    }
}
