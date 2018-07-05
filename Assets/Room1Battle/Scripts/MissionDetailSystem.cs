using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Room1;
using System;

namespace OperationTrident.Util
{
    public class MissionDetailSystem : MonoBehaviour
    {
        public string[] missionDetails =
            {
            //"2018.6.22  星期五    \n雷克雅未克    \n休伯利安行动"
            "2018.6.22  星期五",
            "雷克雅未克",
            "普罗米修斯行动"
        };

        public enum MColor { green, purple, blue, red, pink, yellow};

        public MColor color;

        public float wordTransparentInterval = 0.005f; // 字变得更加透明的周期
        public float wordAppearanceInterval = 0.1f; // 每行字一个个出现的速度
        public float lineSubsequentlyInterval = 1.236f; // 每行字一行行出现的速度
        public int fontSize = 16; // 字体大小

        

        private Color theColor;
        // Use this for initialization
        void Start()
        {
            switch (color)
            {
                case MColor.green:
                    theColor = GUIUtil.greenColor;
                    break;
                case MColor.blue:
                    theColor = GUIUtil.blueColor;
                    break;
                case MColor.pink:
                    theColor = GUIUtil.pinkColor;
                    break;
                case MColor.purple:
                    theColor = GUIUtil.deepPurpleColor;
                    break;
                case MColor.red:
                    theColor = GUIUtil.redColor;
                    break;
                case MColor.yellow:
                    theColor = GUIUtil.yellowColor;
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnGUI()
        {
            try
            {
                GUIUtil.DisplayMissionDetailDefault(
                        missionDetails,
                        Room1.Util.GetCamera(),
                        theColor,
                        wordTransparentInterval: wordTransparentInterval,
                        wordAppearanceInterval: wordAppearanceInterval,
                        lineSubsequentlyInterval: lineSubsequentlyInterval,
                        fontSize: fontSize);
            }
            catch(Exception e)
            {
                GUIUtil.DisplayMissionDetailDefault(
                        missionDetails,
                        Camera.current,
                        theColor,
                        wordTransparentInterval: wordTransparentInterval,
                        wordAppearanceInterval: wordAppearanceInterval,
                        lineSubsequentlyInterval: lineSubsequentlyInterval,
                        fontSize: fontSize);
            }
        }
    }
}