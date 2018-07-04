using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationTrident.Common.AI
{
    public class WanderAIAgent : AIAgent
    {
        [Header("初始化参数")]
        [Tooltip("设置巡逻路径，传入一个根节点")]
        [SerializeField]
        Transform _patrolLocations = null;

        [Tooltip("设置水平FOV角度")]
        [SerializeField]
        [Range(0, 180)]
        float _horizontalFOV = 120f;

        [Tooltip("设置垂直FOV角度")]
        [SerializeField]
        [Range(0, 180)]
        float _verticalFOV = 60f;

        [Tooltip("设置视线距离")]
        [SerializeField]
        [Range(0, 500)]
        float _sightDistance = 100f;

        [Tooltip("设置射击精度范围角度")]
        [SerializeField]
        [Range(0, 10)]
        float _precisionAngle = 5f;

        [Tooltip("设置射击精度范围半径")]
        [SerializeField]
        [Range(0, 5)]
        float _precisionRadius = 1f;

        public override Vector3[] PatrolLocations
        {
            get
            {
                if (_patrolLocations == null)
                    return null;

                Vector3[] result = new Vector3[_patrolLocations.childCount];
                for (int i = 0; i < _patrolLocations.childCount; i++)
                {
                    result[i] = _patrolLocations.GetChild(i).position;
                }
                return result;
            }
        }

        public override NavMeshAgent PathfindingAgent
        {
            get
            {
                return transform.GetComponent<NavMeshAgent>();
            }
        }

        public override AICamera Camera
        {
            get
            {
                return transform.GetComponentInChildren<AICamera>();
            }
        }

        public override float CameraHorizontalFOV
        {
            get
            {
                return _horizontalFOV;
            }
            set
            {
                _horizontalFOV = Mathf.Clamp(value, 0, 180);
            }
        }
        public override float CameraVerticalFOV
        {
            get
            {
                return _verticalFOV;
            }
            set
            {
                _verticalFOV = Mathf.Clamp(value, 0, 180);
            }
        }
        public override float CameraSightDistance
        {
            get
            {
                return _sightDistance;
            }
            set
            {
                _sightDistance = Mathf.Clamp(value, 0, 500);
            }
        }

        public override float AttackPrecisionAngle
        {
            get
            {
                return _precisionAngle;
            }
            set
            {
                _precisionAngle = Mathf.Clamp(value, 0, 10);
            }
        }

        public override float AttackPrecisionRadius
        {
            get
            {
                return _precisionRadius;
            }
            set
            {
                _precisionRadius = Mathf.Clamp(value, 0, 5);
            }
        }

        public void SetPatrolLocations(Transform locationsRoot)
        {
            _patrolLocations = locationsRoot;
        }

        private void Awake()
        {
            Camera.InitCamera(CameraHorizontalFOV, CameraVerticalFOV, CameraSightDistance);
        }
    }
}
