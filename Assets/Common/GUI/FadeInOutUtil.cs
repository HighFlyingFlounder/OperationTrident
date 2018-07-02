using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Util
{
    //请把FadeInOutUtil挂在游戏对象上，因为它要在OnGUI里面画一个贴图
    public class FadeInOutUtil : MonoBehaviour
    {
        //纯黑贴图
        private Texture2D m_PureTexture;

        //全屏的矩形
        private Rect m_FullScreenRect;

        //现在处于哪个状态
        private enum FADING_STATE
        {
            FADING_OUT,
            FADING_IN,
            STOPPED
        };

        private FADING_STATE m_FadingState=FADING_STATE.STOPPED;
        private Color m_FadeColor;
        private float m_Duration=0.0f;
        private float m_CurrentFadingTime=0.0f;//当前的fade in/out已经多久了

        //Cam用来获取渲染窗口的的pixel rect
        //FadeColor是淡入/淡出的基本颜色，只有rgb有用
        public void FadeIn(float duration,Color fadeColor)
        {
            this.enabled = true;
            m_CurrentFadingTime = 0.0f;
            m_Duration = duration;
            m_FadeColor = fadeColor;
            m_FadingState = FADING_STATE.FADING_IN;
        }

        public void FadeOut(float duration, Color fadeColor)
        {
            this.enabled = true;
            m_CurrentFadingTime = 0.0f;
            m_Duration = duration;
            m_FadeColor = fadeColor;
            m_FadingState = FADING_STATE.FADING_OUT;
        }

        /***********************************************
         *                     生命周期函数
         * *********************************************/
        // Use this for initialization
        void Start()
        {
            m_FullScreenRect = new Rect(new Vector2(0, 0), new Vector2(Screen.width, Screen.height));
            m_FadeColor = Color.black;
            m_PureTexture = new Texture2D(1, 1);
            m_PureTexture.SetPixel(0, 0, Color.black);
            m_PureTexture.Apply();
        }

        private void Update()
        {
            m_CurrentFadingTime += Time.deltaTime;
            //时间够了就disable并且设置状态为STOPPED
            if(m_CurrentFadingTime>=m_Duration)
            {
                m_FadingState = FADING_STATE.STOPPED;
                this.enabled = false;
            }
        }

        //画一个纯色全屏贴图
        void OnGUI()
        {
            Color c = m_FadeColor;
            switch (m_FadingState)
            {
                case FADING_STATE.FADING_IN:
                    {
                        float timeFactor = m_CurrentFadingTime / m_Duration;
                        c.a = 1.0f - timeFactor* timeFactor;
                        m_PureTexture.SetPixel(0, 0, c);
                        m_PureTexture.Apply();
                        GUI.DrawTexture(Camera.main.pixelRect, m_PureTexture, ScaleMode.StretchToFill, true);
                        break;
                    }

                case FADING_STATE.FADING_OUT:
                    {
                        float timeFactor = m_CurrentFadingTime / m_Duration;
                        c.a = timeFactor* timeFactor;
                        m_PureTexture.SetPixel(0, 0, c);
                        m_PureTexture.Apply();
                        GUI.DrawTexture(Camera.main.pixelRect, m_PureTexture, ScaleMode.StretchToFill, true);
                        break;
                    }

                case FADING_STATE.STOPPED:
                    return;
            }
        }
    }
}