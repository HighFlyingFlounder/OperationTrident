using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        float _cameraHorizontalFOV = 120f;

        [Tooltip("设置垂直FOV角度")]
        [SerializeField]
        [Range(0, 180)]
        float _cameraVerticalFOV = 60f;

        [Tooltip("设置视线距离")]
        [SerializeField]
        [Range(0, 500)]
        float _cameraSightDistance = 100f;

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

        public override float CameraHorizontalFOV
        {
            get
            {
                return _cameraHorizontalFOV;
            }
            set
            {
                _cameraHorizontalFOV = Mathf.Clamp(value, 0, 180);
            }
        }
        public override float CameraVerticalFOV
        {
            get
            {
                return _cameraVerticalFOV;
            }
            set
            {
                _cameraVerticalFOV = Mathf.Clamp(value, 0, 180);
            }
        }
        public override float CameraSightDistance
        {
            get
            {
                return _cameraSightDistance;
            }
            set
            {
                _cameraSightDistance = Mathf.Clamp(value, 0, 500);
            }
        }

        public void SetPatrolLocations(Transform locationsRoot)
        {
            _patrolLocations = locationsRoot;
        }

        private void Awake()
        {
            
        }
    }
}
