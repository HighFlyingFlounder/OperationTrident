using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Util
{
    //请把FadeInOutUtil挂在游戏对象上，因为它要在OnGUI里面画一个贴图
    public static class FadeInOutUtil
    {
        //纯黑贴图
        private static Texture2D m_PureTexture=null;

        //全屏的矩形
        private static Rect m_FullScreenRect=Rect.zero;

        //现在处于哪个状态
        public enum FADING_STATE
        {
            FADING_OUT,
            FADING_IN,
            STOPPED
        };

        private static FADING_STATE m_FadingState=FADING_STATE.STOPPED;
        private static Color m_FadeColor=Color.black;
        private static float m_Duration=0.0f;
        private static float m_CurrentFadingTime=0.0f;//当前的fade in/out已经多久了
        private static Camera m_GuiCamera=null;

        //Cam用来获取渲染窗口的的pixel rect
        //FadeColor是淡入/淡出的基本颜色，只有rgb有用
        //state是当前正在“淡入/淡出/停止"的状态
        public static void SetFadingState(float duration,Camera guiCam,Color fadeColor,FADING_STATE state)
        {
            if(m_PureTexture==null)
            {
                m_PureTexture = new Texture2D(1, 1);
                m_PureTexture.SetPixel(0, 0, Color.black);
                m_PureTexture.Apply();
            }

            m_CurrentFadingTime = 0.0f;
            m_Duration = duration;
            m_GuiCamera = guiCam;
            m_FadeColor = fadeColor;
            m_FadingState = state;
        }

        public static FADING_STATE GetFadingState()
        {
            return m_FadingState;
        }

        /***********************************************
         *                     生命周期函数
         * *********************************************/
         //！！！需要每帧在update里调用！！
        public static void UpdateState()
        {
            m_CurrentFadingTime += Time.deltaTime;
            //时间够了就disable并且设置状态为STOPPED
            if(m_CurrentFadingTime>=m_Duration)
            {
                m_FadingState = FADING_STATE.STOPPED;
            }
        }

        //画一个纯色全屏贴图
        //！！！需要每帧在OnGUI里调用！！

        public static void RenderGUI()
        {
            if (m_GuiCamera == null)
            {
                Debug.Log("warning: FadeInOutUtil GUI camera没绑定！正在变成Camera.current");
                m_GuiCamera = Camera.current;
            }

            Color c = m_FadeColor;
            switch (m_FadingState)
            {
                case FADING_STATE.FADING_IN:
                    {
                        float timeFactor = m_CurrentFadingTime / m_Duration;
                        c.a = 1.0f - timeFactor* timeFactor;
                        m_PureTexture.SetPixel(0, 0, c);
                        m_PureTexture.Apply();
                        GUI.DrawTexture(m_GuiCamera.pixelRect, m_PureTexture, ScaleMode.StretchToFill, true);
                        break;
                    }

                case FADING_STATE.FADING_OUT:
                    {
                        float timeFactor = m_CurrentFadingTime / m_Duration;
                        c.a = timeFactor* timeFactor;
                        m_PureTexture.SetPixel(0, 0, c);
                        m_PureTexture.Apply();
                        GUI.DrawTexture(m_GuiCamera.pixelRect, m_PureTexture, ScaleMode.StretchToFill, true);
                        break;
                    }

                case FADING_STATE.STOPPED:
                    GUI.DrawTexture(m_GuiCamera.pixelRect, m_PureTexture, ScaleMode.StretchToFill, true);
                    return;
            }
        }
    }
}