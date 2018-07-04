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

        AICamera _camera = null;
        NavMeshAgent _navAgent = null;

        float _precisionAngle;
        float _precisionRadius;

        public override void Init()
        {
            base.Init();
            InitOnce();
        }

        public override void InitOnce()
        {
            if (_firstInit)
            {
                base.InitOnce();
                _camera = GetComponent<AIAgent>().Camera;
                _navAgent = GetComponent<AIAgent>().PathfindingAgent;
                _precisionAngle = GetComponent<AIAgent>().AttackPrecisionAngle;
                _precisionRadius = GetComponent<AIAgent>().AttackPrecisionRadius;
            }
        }
        public override string Execute()
        {
            transform.forward = Utility.GetDirectionOnXOZ(transform.position, GetComponent<AIAgent>().Target.position);

            _camera.UpdateCamera();

            Vector3 shootPoint;
            // 先判断是否在射击范围内，若不在，先追击敌人，等敌人进入射击范围后，开始射击
            if(!_camera.GetShootPoint(_precisionAngle, _precisionRadius, GetComponent<AIAgent>().Target.position, out shootPoint))
            {
                _navAgent.SetDestination(GetComponent<AIAgent>().Target.position);
                _navAgent.isStopped = false;
            }
            else
            {
                _navAgent.isStopped = true;
                GetComponent<TestShoot>().Shoot(shootPoint);
                _satisfy = Conditions.FINISH_ONCE_SHOOT;
            }

            // 敌人可能已死亡或躲到障碍后
            if (!_camera.DetectTarget(GetComponent<AIAgent>().Target))
            {
                _navAgent.isStopped = true;
                _satisfy = Conditions.LOST_TARGET;
#if UNITY_EDITOR
                _camera.DrawDefaultAttackPrecisionRange();
#endif
                return _satisfy;
            }

            return _satisfy;
        }
    }
}
