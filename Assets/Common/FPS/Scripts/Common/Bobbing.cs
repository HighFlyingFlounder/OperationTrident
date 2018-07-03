using System;
using System.Collections;
using UnityEngine;

namespace OperationTrident.FPS.Common {
    [Serializable]
    public class Bobbing {
        [Tooltip("水平方向上的抖动系数")]
        public float HorizontalBobbingRange = 0.33f;
        [Tooltip("竖直方向上的抖动")]
        public float VerticalBobbingRange = 0.33f;
        [Tooltip("抖动曲线，横坐标为抖动时间，纵坐标为抖动距离")]
        public AnimationCurve Bobbingcurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                                                            new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                                                            new Keyframe(2f, 0f)); // sin curve for head bob
        [Tooltip("水平抖动值和竖直方向抖动时间之比")]
        public float VerticaltoHorizontalRatio = 1.5f;
        [Tooltip("抖动周期，也就是走一步的时间")]
        public float StepInterval = 1f;

        //摇晃的对象
        private Transform m_BobbingObject;
        //用来保存X方向上的time
        private float m_CyclePositionX;
        //用来保存Y方向上的time
        private float m_CyclePositionY;
        //初始位置
        private Vector3 m_OriginalPosition;
        //计时器
        private float m_CurveDuration;
        //摇晃的速度
        private float m_Speed;

        public void Init(Transform bobbingObject) {
            m_BobbingObject = bobbingObject;

            m_OriginalPosition = m_BobbingObject.localPosition;

            //获得曲线的长度
            m_CurveDuration = Bobbingcurve[Bobbingcurve.length - 1].time;

            m_CyclePositionX = 0f;
            m_CyclePositionY = 0f;
        }

        public void ChangeSpeed(float speed) {
            m_Speed = speed;
        }


        public IEnumerator DoBobbing() {
            float xPos, yPos;

            while (true) {
                xPos = m_OriginalPosition.x + (Bobbingcurve.Evaluate(m_CyclePositionX) * HorizontalBobbingRange);
                yPos = m_OriginalPosition.y + (Bobbingcurve.Evaluate(m_CyclePositionY) * VerticalBobbingRange);

                m_CyclePositionX += (m_Speed * Time.deltaTime) / StepInterval;
                m_CyclePositionY += ((m_Speed * Time.deltaTime) / StepInterval) * VerticaltoHorizontalRatio;

                //循环播放
                if (m_CyclePositionX > m_CurveDuration) {
                    m_CyclePositionX = m_CyclePositionX - m_CurveDuration;
                }
                if (m_CyclePositionY > m_CurveDuration) {
                    m_CyclePositionY = m_CyclePositionY - m_CurveDuration;
                }

                m_BobbingObject.localPosition = new Vector3(xPos, yPos, m_OriginalPosition.z);

                yield return new WaitForFixedUpdate();
            }
        }

    }
}
