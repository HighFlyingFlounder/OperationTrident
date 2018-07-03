using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationTrident.Common.AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public class PatrolState : AIState
    {
        public static readonly string STATE_NAME = "Patrol";

        public class Conditions
        {
            public static readonly string ARRIVE_AT_LOCATION = "Arrive at location";
            public static readonly string SIGHT_PLAYER = "Sight Player";
        }

        // 巡逻点列表，AI会在这些地点中循环移动
        Vector3[] _patrolLocations;
        // 记录下一个巡逻点
        int _nextPatrolLocationIndex = 0;
        // 寻路代理
        NavMeshAgent _navAgent = null;
        // 用于判断是否正在移动
        bool _isMoving = false;
        // 到下一个巡逻点的距离
        float _remainDistance = Mathf.Infinity;
        AICamera _camera = null;
        // 用于控制动画（之后会换成控制器）
        Animator _animator;

        public override void Init()
        {
            base.Init();
            _isMoving = false;

            InitOnce();
        }

        public override void InitOnce()
        {
            if (_firstInit)
            {
                base.InitOnce();
                _patrolLocations = GetComponent<AIAgent>().PatrolLocations;
                if(_patrolLocations == null)
                    Debug.Log("没有设置巡逻路径");
                _navAgent = transform.GetComponent<NavMeshAgent>();
                _animator = transform.GetComponent<Animator>();
                _camera = transform.GetComponentInChildren<AICamera>();
            }
        }

        public override string Execute()
        {
            if (!_isMoving)
            {
                // 设置寻路目标点
                _navAgent.SetDestination(_patrolLocations[_nextPatrolLocationIndex]);
                _isMoving = true;
            }

            if (!_navAgent.pathPending)
            {
                _remainDistance = _navAgent.remainingDistance;
                if (_remainDistance != Mathf.Infinity && _remainDistance - _navAgent.stoppingDistance <= float.Epsilon
                && _navAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    // 当到达一个巡逻点时，设置下一个巡逻点，并返回已满足ARRIVE_AT_LOCATION条件
                    _nextPatrolLocationIndex = (_nextPatrolLocationIndex + 1) % _patrolLocations.Length;
                    _satisfy = Conditions.ARRIVE_AT_LOCATION;
                }
            }

            _camera.UpdateCamera();

            Transform[] players = Utility.GetPlayersPosition();
            foreach (var player in players)
            {
                if (_camera.DetectTarget(player))
                {
                    GetComponent<AIAgent>().TargetPosition = Utility.GetPlayerShootedTarget(player);
                    _satisfy = Conditions.SIGHT_PLAYER;
                    return _satisfy;
                }
            }

            // 移动时的动画
            _animator.SetFloat("Speed", _navAgent.speed);

            return _satisfy;
        }
    }
}
