using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Util
{
    // 一个供测试用的工具类，可以选择字体
    public class FontSelector : MonoBehaviour
    {
        Vector2 scrollPos;
        string[] fonts;

        void Start()
        {
            fonts = Font.GetOSInstalledFontNames();
        }

        void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach (var font in fonts)
            {
                if (GUILayout.Button(font))
                    GUI.skin.font = Font.CreateDynamicFontFromOSFont(font, 12);
            }
            GUILayout.EndScrollView();
        }
    }
}