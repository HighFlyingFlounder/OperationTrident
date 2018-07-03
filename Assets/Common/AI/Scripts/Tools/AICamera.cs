using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [ExecuteInEditMode]
    public class AICamera : MonoBehaviour
    {
        // 设置水平FOV角度
        float _horizontalFOV;
        // 设置垂直FOV角度
        float _verticalFOV;
        // 设置视线距离
        float _sightDistance;

#if UNITY_EDITOR
        // 目标点（测试用）
        [SerializeField]
        Transform _testTarget;
#endif

        Vector3 leftDirection, rightDirection, upDirection, downDirection;
        float halfWidth;

#if UNITY_EDITOR
        void Update()
        {
            AIAgent agent = GetComponentInParent<AIAgent>();
            InitCamera(agent.CameraHorizontalFOV, agent.CameraVerticalFOV, agent.CameraSightDistance);
            UpdateCamera();
            DrawViewableArea();
            // DetectTarget(_testTarget);
        }
#endif

        /// <summary>
        /// 初始化camera参数 
        /// </summary>
        /// <param name="horizontalFOV">水平可视角度</param>
        /// <param name="verticalFOV">垂直可视角度</param>
        /// <param name="sightDistance">视线距离</param>
        public void InitCamera(float horizontalFOV, float verticalFOV, float sightDistance)
        {
            _horizontalFOV = horizontalFOV;
            _verticalFOV = verticalFOV;
            _sightDistance = sightDistance;
        }

        /// <summary>
        /// 在进行检测前先更新摄像机的参数 
        /// </summary>
        public void UpdateCamera()
        {
            Quaternion leftAngle = Quaternion.AngleAxis(-_horizontalFOV / 2, transform.up);
            Quaternion rightAngle = Quaternion.AngleAxis(_horizontalFOV / 2, transform.up);
            Quaternion upAngle = Quaternion.AngleAxis(-_verticalFOV / 2, transform.right);
            Quaternion downAngle = Quaternion.AngleAxis(_verticalFOV / 2, transform.right);

            leftDirection = leftAngle * transform.forward;
            rightDirection = rightAngle * transform.forward;
            upDirection = upAngle * transform.forward;
            downDirection = downAngle * transform.forward;

            halfWidth = _sightDistance * Mathf.Tan(_horizontalFOV * Mathf.Deg2Rad / 2);
        }

        /// <summary>
        /// 检测传入的目标是否在AI的视线内
        /// </summary>
        /// <param name="target">需要检测的目标</param>
        /// <returns>
        /// true: 在视线内  false：不在视线内
        /// </returns>
        public bool DetectTarget(Transform target)
        {
            Vector3 targetDirection = target.transform.position - transform.position;
            float targetDistance = Vector3.Dot(targetDirection, transform.forward);

            // 判断目标是否在camera的远近平面内
            if (targetDistance < 0 || targetDistance > _sightDistance)
                return false;

            // 根据目标点的方向，求出目标方向在水平、竖直方向上的分量，然后判断在不在视角内
            // 判断是否在水平视角内
            Vector3 targetHorizontal = Vector3.Dot(targetDirection, transform.right) * transform.right + transform.forward * targetDistance; // 水平方向
            Vector3 leftCross = Vector3.Cross(targetHorizontal, leftDirection);
            Vector3 rightCross = Vector3.Cross(targetHorizontal, rightDirection);
            if (Vector3.Dot(leftCross, rightCross) > 0)
                return false;

            // 判断是否在垂直视角内
            Vector3 targetVertical = Vector3.Dot(targetDirection, transform.up) * transform.up + transform.forward * targetDistance; // 竖直方向
            Vector3 upCross = Vector3.Cross(targetVertical, upDirection);
            Vector3 downCross = Vector3.Cross(targetVertical, downDirection);
            if (Vector3.Dot(upCross, downCross) > 0)
                return false;

            // 用射线检测是否有障碍在中间
            Ray ray = new Ray(transform.position, targetDirection);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                if (hit.collider.name != target.name)
                    return false;
            }

            return true;
        }

#if UNITY_EDITOR
        void DrawViewableArea()
        {
            Vector3 origin = transform.position;
            float verticalDistance = _sightDistance / Mathf.Cos(_verticalFOV * Mathf.Deg2Rad / 2);
            Vector3 right = halfWidth * transform.right;

            Vector3 topLeft = origin + upDirection * verticalDistance - right;
            Vector3 topRight = origin + upDirection * verticalDistance + right;
            Vector3 bottomLeft = origin + downDirection * verticalDistance - right;
            Vector3 bottomRight = origin + downDirection * verticalDistance + right;

            Debug.DrawLine(origin, topLeft);
            Debug.DrawLine(origin, topRight);
            Debug.DrawLine(origin, bottomLeft);
            Debug.DrawLine(origin, bottomRight);

            Debug.DrawLine(topLeft, topRight);
            Debug.DrawLine(bottomLeft, bottomRight);
            Debug.DrawLine(topLeft, bottomLeft);
            Debug.DrawLine(topRight, bottomRight);
        }
#endif
    }
}