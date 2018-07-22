using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationTrident.Common.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class TrackingAIAgent : AIAgent
    {
        [SerializeField]
        TrackingAIAgentInitParams _initParams;

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
                return _initParams.horizontalFOV;
            }
            set
            {
                _initParams.horizontalFOV = Mathf.Clamp(value, 0, 180);
            }
        }
        public override float CameraVerticalFOV
        {
            get
            {
                return _initParams.verticalFOV;
            }
            set
            {
                _initParams.verticalFOV = Mathf.Clamp(value, 0, 180);
            }
        }
        public override float CameraSightDistance
        {
            get
            {
                return _initParams.sightDistance;
            }
            set
            {
                _initParams.sightDistance = Mathf.Clamp(value, 0, 500);
            }
        }

        public override float AttackPrecisionAngle
        {
            get
            {
                return _initParams.precisionAngle;
            }
            set
            {
                _initParams.precisionAngle = Mathf.Clamp(value, 0, 10);
            }
        }

        public override float AttackPrecisionRadius
        {
            get
            {
                return _initParams.precisionRadius;
            }
            set
            {
                _initParams.precisionRadius = Mathf.Clamp(value, 0, 5);
            }
        }

        private void Awake()
        {
            Camera.InitCamera(CameraHorizontalFOV, CameraVerticalFOV, CameraSightDistance);
        }

        private new void Update()
        {
            if (!ReactiveTarget.IsAlive)
            {
                PathfindingAgent.enabled = false;
            }

            base.Update();
        }

        public override void SetInitParams(AIAgentInitParams initParams)
        {
            _initParams = (TrackingAIAgentInitParams)initParams;
        }

    }

	[System.Serializable]
    public class TrackingAIAgentInitParams : AIAgentInitParams
    {
        [Tooltip("设置水平FOV角度")]
        [Range(0, 180)]
        public float horizontalFOV = 120f;

        [Tooltip("设置垂直FOV角度")]
        [Range(0, 180)]
        public float verticalFOV = 60f;

        [Tooltip("设置视线距离")]
        [Range(0, 500)]
        public float sightDistance = 100f;

        [Tooltip("设置射击精度范围角度")]
        [Range(0, 10)]
        public float precisionAngle = 5f;

        [Tooltip("设置射击精度范围半径")]
        [Range(0, 5)]
        public float precisionRadius = 1f;
    }
}