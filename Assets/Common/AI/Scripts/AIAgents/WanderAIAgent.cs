using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace OperationTrident.Common.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class WanderAIAgent : AIAgent
    {
         [SerializeField]
         WanderAIAgentInitParams _initParams;

        public override Vector3[] PatrolLocations
        {
            get
            {
                if (_initParams.patrolLocations == null)
                    return null;

                Transform prelocationTrans = GameObject.Find(_initParams.patrolLocations).transform;
                Vector3[] result = new Vector3[prelocationTrans.childCount];
                for (int i = 0; i < prelocationTrans.childCount; i++)
                {
                    result[i] = prelocationTrans.GetChild(i).position;
                }

                //Vector3[] result = new Vector3[_initParams.patrolLocations.childCount];
                //for (int i = 0; i < _initParams.patrolLocations.childCount; i++)
                //{
                //    result[i] = _initParams.patrolLocations.GetChild(i).position;
                //}

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

        public override int PatrolStartLocationIndex
        {
            get
            {
                return _initParams.patrolStartLocationIndex;
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

        public void SetPatrolLocations(string locationsRoot)
        {
            _initParams.patrolLocations = locationsRoot;
        }

        private void Awake()
        {
            Camera.InitCamera(CameraHorizontalFOV, CameraVerticalFOV, CameraSightDistance);
        }

        private new void Update()
        {
            if(!ReactiveTarget.IsAlive){
                PathfindingAgent.enabled = false;
            }
            
            base.Update();
        }

        public override void SetInitParams(AIAgentInitParams initParams)
        {
            _initParams = (WanderAIAgentInitParams)initParams;
        }
    }

    [System.Serializable]
    public class WanderAIAgentInitParams : AIAgentInitParams
    {
        [Tooltip("设置巡逻路径，传入一个根节点")]
        public string patrolLocations = null;

        [Tooltip("设置巡逻路径起始点")]
        public int patrolStartLocationIndex = 0;

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
