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

        [System.Serializable]
        public class InitParams : InitParamsBase
        {
            public Transform patrolLocations;
        }

        // 巡逻点列表，AI会在这些地点中循环移动
        List<Vector3> _patrolLocations;
        // 记录下一个巡逻点
        int _nextPatrolLocationIndex = 0;
        // 寻路代理
        NavMeshAgent _navAgent = null;
        // 用于判断是否正在移动
        bool _isMoving = false;
        // 到下一个巡逻点的距离
        float _remainDistance = Mathf.Infinity;
        // 用于控制动画（之后会换成控制器）
        Animator _animator;

        public override void Init(AIStateParam param)
        {
            base.Init(param);
            _paramParser = new WanderStateParamParser(param);
            _isMoving = false;

            InitOnce();
        }

        public override void InitOnce()
        {
            if (_firstInit)
            {
                base.InitOnce();
                _patrolLocations = (_paramParser as WanderStateParamParser).PatrolLocations;
                _navAgent = transform.GetComponent<NavMeshAgent>();
                _animator = transform.GetComponent<Animator>();
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
                    _nextPatrolLocationIndex = (_nextPatrolLocationIndex + 1) % _patrolLocations.Count;
                    _satisfy = Conditions.ARRIVE_AT_LOCATION;
                }
            }

            // 移动时的动画
            _animator.SetFloat("Speed", _navAgent.speed);

            return _satisfy;
        }
    }

    public class WanderStateParamParser : AIStateParamParserBase
    {
        public WanderStateParamParser(AIStateParam param) : base(param) { }

        public List<Vector3> PatrolLocations
        {
            get
            {
                object locations = _param.GetMassData("patrolLocations");
                if (locations != null)
                {
                    Transform[] tempPatrolLocations = (locations as Transform).GetComponentsInChildren<Transform>();
                    List<Vector3> result = new List<Vector3>();
                    for (int i = 1; i < tempPatrolLocations.Length; i++)
                    {
                        result.Add(tempPatrolLocations[i].position);
                    }
                    return result;
                }
                else
                {
                    Debug.Log("Can not get 'patrolLocations' in 'WanderStateParamParser'.");
                    throw new System.NotImplementedException();
                }
            }
        }
    }
}
