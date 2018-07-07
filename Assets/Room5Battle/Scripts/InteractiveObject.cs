using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Util;

namespace OperationTrident.Room5
{
    //InteractiveObject的 Init/Update/GUI渲染 需要手动调用，只是一个类似于工具类的东西
    //Initialize，然后在外面的Update()调用UpdateState，OnGUI()调用RenderGUI
    //要把这东西挂到对应物体上；交互使用RayCast的CompareTag来做的
    public class InteractiveObject:MonoBehaviour
    {
        //画进度条的纯色texture
        private Texture2D m_TextureGrey;

        //用于碰撞的Tag
        private string m_ObjectTag;

        //交互按键
        private KeyCode m_Key;

        //按键持续时间（大于0可以画进度条，等于0就瞬间获取）
        private float m_RequiredKeyPressPersistTime;

        //GUI渲染camera
        //private Room5GetCamera m_GetCamUtil;
        //private Camera m_Cam;

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
        public void Initialize(string objectTag, KeyCode interactKey, float minimumInteractTime,string lookingPromptText,string interactingPromptText)
        {
            m_ObjectTag = objectTag;
            m_Key = interactKey;
            m_RequiredKeyPressPersistTime = minimumInteractTime;
            m_PlayerLookingPromptText = lookingPromptText;
            m_PlayerInteractingPromptText = interactingPromptText;

            m_TextureGrey = new Texture2D(1, 1);
            m_TextureGrey.SetPixel(0, 0, new Color(0.9f, 0.9f, 0.9f));
            m_TextureGrey.Apply();
        }

        //不是MonoBehaviour，需要被调用（为了不迷惑，就不叫Update()了）
        public void UpdateState()
        {
            //摄像机中心发出的射线
            m_IsLookingAtObject = mFunc_IsLookingAtObject();

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
                    GUIUtil.DisplaySubtitleInGivenGrammar(m_PlayerInteractingPromptText, Room5.GetCameraUtil.GetCurrentCamera(), 0.5f);

                    //进度条
                    float barWidth = 200.0f;
                    float halfW = Room5.GetCameraUtil.GetCurrentCamera().pixelWidth / 2;
                    float halfH = Room5.GetCameraUtil.GetCurrentCamera().pixelHeight / 2;
                    Rect rect2 = new Rect();
                    rect2.xMin = halfW - barWidth / 2;
                    rect2.xMax = rect2.xMin + barWidth * (m_CurrentKeyPressedTime / m_RequiredKeyPressPersistTime);
                    rect2.yMin = halfH + 30.0f;
                    rect2.yMax = halfH + 40.0f;
                    GUI.DrawTexture(rect2, m_TextureGrey);
                }
                else
                {
                    GUIUtil.DisplaySubtitleInGivenGrammar(m_PlayerLookingPromptText, Room5.GetCameraUtil.GetCurrentCamera(), 0.5f);
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
        private bool mFunc_IsLookingAtObject()
        {
            if (Room5.GetCameraUtil.GetCurrentCamera())
            {
                Camera cam = Room5.GetCameraUtil.GetCurrentCamera();
                //摄像机中心发出的射线
                Vector3 centerCoordPixel = new Vector3(cam.pixelWidth / 2.0f, cam.pixelHeight / 2.0f);
                Ray viewRay = cam.ScreenPointToRay(centerCoordPixel);
                RaycastHit hitInfo;
                const float rayCastDepth = 7.0f;
                if (Physics.Raycast(viewRay, out hitInfo, rayCastDepth))
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

    }
}
