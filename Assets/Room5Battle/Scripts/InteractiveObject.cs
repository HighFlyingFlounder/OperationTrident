using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

namespace OperationTrident.Room5
{
    //InteractiveObject的 Init/Update/GUI渲染 需要手动调用，只是一个类似于工具类的东西
    //Initialize，然后在外面的Update()调用UpdateState，OnGUI()调用RenderGUI
    public class InteractiveObject:MonoBehaviour
    {
        //画进度条的纯色texture
        [SerializeField]
        private Texture m_TextureGrey;

        //用于碰撞的Tag
        private string m_ObjectTag;

        //交互按键
        private KeyCode m_Key;

        //按键持续时间（大于0可以画进度条，等于0就瞬间获取）
        private float m_RequiredKeyPressPersistTime;

        //GUI渲染camera
        private Camera m_Cam;

        //玩家看着物体时的GUI提示
        private string m_PlayerLookingPromptText;

        //玩家交互时的GUI提示
        private string m_PlayerInteractingPromptText;

        //...
        private bool m_IsLookingAtObject = false;
        private bool m_IsPressingKey = false;
        private float m_CurrentKeyPressedTime = 0.0f;

        //互动是否已完成（按下/按完 F 之类的）
        private bool m_IsInteractionDone = false;

        //初始化可交互对象(Interactive Object)的参数：
        //1.可交互对象的Tag（用于raycast）
        //2.GUI的渲染目标camera
        //3.用什么按键来交互
        //4.至少交互多久
        //5.玩家看着时的GUI提示
        //6.玩家在按键交互时的GUI提示
        public void Initialize(string objectTag,Camera guiCam, KeyCode interactKey, float minimumInteractTime,string lookingPromptText,string interactingPromptText)
        {
            m_ObjectTag = objectTag;
            m_Key = interactKey;
            m_RequiredKeyPressPersistTime = minimumInteractTime;
            m_Cam = guiCam;
            m_PlayerLookingPromptText = lookingPromptText;
            m_PlayerInteractingPromptText = interactingPromptText;
        }

        //不是MonoBehaviour，需要被调用（为了不迷惑，就不叫Update()了）
        public void UpdateState()
        {
            //摄像机中心发出的射线
            m_IsLookingAtObject = IsLookingAtObject();

            //如果玩家按下F
            m_IsPressingKey = Input.GetKey(m_Key);

            //是否对着控制台按Key
            if (m_IsPressingKey && m_IsLookingAtObject)
                m_CurrentKeyPressedTime += Time.deltaTime;
            else
                m_CurrentKeyPressedTime = 0;

            //按住F 10s就可以了
            if (m_CurrentKeyPressedTime > m_RequiredKeyPressPersistTime)
            {
                m_IsInteractionDone = true;
            }
        }

        //GUI的渲染(为了不迷惑，就不叫OnGUI了)
        public void RenderGUI()
        {
            if (m_IsLookingAtObject)
            {
                //显示持续按键的进度条
                if (m_IsPressingKey)
                {
                    GUIUtil.DisplaySubtitleInGivenGrammar(m_PlayerInteractingPromptText, Camera.main, 0.5f);

                    //进度条
                    float barWidth = 200.0f;
                    float halfW = Camera.main.pixelWidth / 2;
                    float halfH = Camera.main.pixelHeight / 2;
                    Rect rect2 = new Rect();
                    rect2.xMin = halfW - barWidth / 2;
                    rect2.xMax = rect2.xMin + barWidth * (m_CurrentKeyPressedTime / m_RequiredKeyPressPersistTime);
                    rect2.yMin = halfH + 30.0f;
                    rect2.yMax = halfH + 35.0f;
                    GUI.DrawTexture(rect2, m_TextureGrey);
                }
                else
                {
                    GUIUtil.DisplaySubtitleInGivenGrammar(m_PlayerLookingPromptText, Camera.main, 0.5f);
                }
            }
        }

        //交互是否已完成
        public bool IsInteractionDone()
        {
            this.enabled = false;
            return m_IsInteractionDone;
        }

        /******************************************
         *                          PRIVATE
         * ****************************************/
        //玩家Camera view ray和Tagged了的目标物体的求交
        private bool IsLookingAtObject()
        {
            if (m_Cam)
            {
                //摄像机中心发出的射线
                Vector3 centerCoordPixel = new Vector3(m_Cam.pixelWidth / 2.0f, m_Cam.pixelHeight / 2.0f);
                Ray viewRay = m_Cam.ScreenPointToRay(centerCoordPixel);
                RaycastHit hitInfo;
                if (Physics.Raycast(viewRay, out hitInfo, 4.0f))
                {
                    //如果玩家看着物体
                    if (hitInfo.transform.CompareTag(m_ObjectTag))
                    { return true; }
                    else
                    { return false; }
                }
            }
            return false;
        }

        //=========================================
        //=============  重新设置相机 =============
        //=========================================
        public void SetGUICamera(Camera guiCam)
        {
            m_Cam = guiCam;
        }
    }
}
