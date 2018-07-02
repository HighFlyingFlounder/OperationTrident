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
        }

        [System.Serializable]
        public class InitParams : InitParamsBase
        {
            public Transform patrolLocations;
        }

        List<Vector3> _patrolLocations;
        bool _firstInit = true;
        int _nextPatrolLocationIndex = 0;
        NavMeshAgent _navAgent = null;
        bool _isMoving = false;
        float _remainDistance = Mathf.Infinity;
        Animator _animator;

        public override void Init(AIStateParam param)
        {
            base.Init(param);
            if (_firstInit)
            {
                _firstInit = false;
                _paramParser = new WanderStateParamParser(param);
                _patrolLocations = (_paramParser as WanderStateParamParser).PatrolLocations;
                _navAgent = transform.GetComponent<NavMeshAgent>();
                _animator = transform.GetComponent<Animator>();
            }
            _isMoving = false;
        }

        public override string Execute()
        {
            if (!_isMoving)
            {
                _navAgent.SetDestination(_patrolLocations[_nextPatrolLocationIndex]);
                _isMoving = true;
                _nextPatrolLocationIndex = (_nextPatrolLocationIndex + 1) % _patrolLocations.Count;
            }

            if (!_navAgent.pathPending)
            {
                _remainDistance = _navAgent.remainingDistance;
                if (_remainDistance != Mathf.Infinity && _remainDistance - _navAgent.stoppingDistance <= float.Epsilon
                && _navAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    _satisfy = Conditions.ARRIVE_AT_LOCATION;
                }
            }
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
