﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [ExecuteInEditMode]
    public class AICamera : MonoBehaviour
    {
        // 设置水平FOV角度
        [SerializeField]
        [Range(0, 180)]
        float horizontalFOV = 120f;
        // 设置垂直FOV角度
        [SerializeField]
        [Range(0, 180)]
        float verticalFOV = 60f;
        // 设置视线距离
        [SerializeField]
        [Range(0, 500)]
        float sightDistance = 100f;

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
            UpdateCamera();
            DrawViewableArea();
            // Debug.Log(DetectTarget(_testTarget));
            DetectTarget(_testTarget);
        }
#endif

        /// <summary>
        /// 在进行检测前先更新摄像机的参数 
        /// </summary>
        public void UpdateCamera()
        {
            Quaternion leftAngle = Quaternion.AngleAxis(-horizontalFOV / 2, transform.up);
            Quaternion rightAngle = Quaternion.AngleAxis(horizontalFOV / 2, transform.up);
            Quaternion upAngle = Quaternion.AngleAxis(-verticalFOV / 2, transform.right);
            Quaternion downAngle = Quaternion.AngleAxis(verticalFOV / 2, transform.right);

            leftDirection = leftAngle * transform.forward;
            rightDirection = rightAngle * transform.forward;
            upDirection = upAngle * transform.forward;
            downDirection = downAngle * transform.forward;

            halfWidth = sightDistance * Mathf.Tan(horizontalFOV * Mathf.Deg2Rad / 2);
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
            if (targetDistance < 0 || targetDistance > sightDistance)
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
            float verticalDistance = sightDistance / Mathf.Cos(verticalFOV * Mathf.Deg2Rad / 2);
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