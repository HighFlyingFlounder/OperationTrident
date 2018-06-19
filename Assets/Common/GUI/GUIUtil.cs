using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OperationTrident.Util
{
    public static class GUIUtil
    {
        public readonly static Vector3 blackVec = new Vector3(0.0f, 0.0f, 0.0f);// 完全黑
        public readonly static Vector3 greyVec = new Vector3(0.5f, 0.5f, 0.5f);// 中间灰
        public readonly static Vector3 whiteVec = new Vector3(1.0f, 1.0f, 1.0f);// 完全白
        public readonly static Vector3 blueVec = new Vector3(0.0f, 0.0f, 1.0f);// 完全蓝
        public readonly static Vector3 redVec = new Vector3(1.0f, 0.0f, 0.0f);// 完全红
        public readonly static Vector3 greenVec = new Vector3(0.0f, 1.0f, 0.0f);// 完全绿
        public readonly static Vector3 seaBlueVec = new Vector3(0.0f, 0.5f, 1.0f);// 深一点的蓝
        public readonly static Vector3 skyBlueVec = new Vector3(0.0f, 1.0f, 1.0f);// 浅一点的蓝
        public readonly static Vector3 deepPurpleVec = new Vector3(0.5f, 0.0f, 1.0f);// 深一点的紫
        public readonly static Vector3 brightPurpleVec = new Vector3(1.0f, 0.0f, 1.0f);// 亮一点的紫
        public readonly static Vector3 pinkVec = new Vector3(1.0f, 0.0f, 0.5f);// 粉色
        public readonly static Vector3 orangeVec = new Vector3(1.0f, 0.5f, 0.0f);// 橙色
        public readonly static Vector3 yellowVec = new Vector3(1.0f, 1.0f, 0.0f);// 黄色
        public readonly static Vector3 brightGreenVec = new Vector3(0.5f, 1.0f, 0.0f);// 亮一点的绿色
        public readonly static Vector3 deepGreenVec = new Vector3(0.0f, 1.0f, 0.5f);// 深一点的绿色

        public readonly static Color blackColor = GetColorFromVector3(blackVec);
        public readonly static Color greyColor = GetColorFromVector3(greyVec);
        public readonly static Color whiteColor = GetColorFromVector3(whiteVec);
        public readonly static Color blueColor = GetColorFromVector3(blueVec);
        public readonly static Color redColor = GetColorFromVector3(redVec);
        public readonly static Color greenColor = GetColorFromVector3(greenVec);
        public readonly static Color seaBlueColor = GetColorFromVector3(seaBlueVec);
        public readonly static Color skyBlueColor = GetColorFromVector3(skyBlueVec);
        public readonly static Color deepPurpleColor = GetColorFromVector3(deepPurpleVec);
        public readonly static Color brightPurpleColor = GetColorFromVector3(brightPurpleVec);
        public readonly static Color pinkColor = GetColorFromVector3(pinkVec);
        public readonly static Color orangeColor = GetColorFromVector3(orangeVec);
        public readonly static Color yellowColor = GetColorFromVector3(yellowVec);
        public readonly static Color brightGreenColor = GetColorFromVector3(brightGreenVec);
        public readonly static Color deepGreenColor = GetColorFromVector3(deepGreenVec);

        public readonly static Color subtitleNormalColor = GetPureColor(200.0f / 256.0f);
        public readonly static Color missionContentNormalCor = GetColorFromString("66 33 99");

        // 默认的字体大小
        private const int defaultFontSize = 12;

        // 默认的字体对齐
        private const TextAlignment defaultAlignment = TextAlignment.Center;
        private const TextAnchor defaultAnchor = TextAnchor.MiddleCenter;

        // 字幕在屏幕的哪里：按比例算
        private const float defaultSubtitleRatioHeight = 4.0f / 5.0f;

        // 微软雅黑
        public readonly static Font microsoftYaHei = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", defaultFontSize);

        public static int DefaultFontSize
        {
            get
            {
                return defaultFontSize;
            }
        }

        public static TextAlignment DefaultAlignment
        {
            get
            {
                return defaultAlignment;
            }
        }

        public static TextAnchor DefaultAnchor
        {
            get
            {
                return defaultAnchor;
            }
        }

        public static float DefaultSubtitleRatioHeight
        {
            get
            {
                return defaultSubtitleRatioHeight;
            }
        }

        // 传入参数获得一种纯色  越接近0越黑，越接近1越白
        public static Color GetPureColor(float factor)
        {
            return GetColorFromVector3(new Vector3(factor, factor, factor));
        }

        // 混合两个颜色
        public static Vector3 MixTwoColor(Vector3 colorA, Vector3 colorB)
        {
            return new Vector3((colorA.x + colorB.x) / 2, (colorA.y + colorB.y) / 2, (colorA.z + colorB.z) / 2);
        }

        // 从一个字符串里面构造颜色向量 格式"48 3F 1F"
        public static Vector3 GetColorVec3FromString(string a)
        {
            try
            {
                int[] rgb = GetIntArrayFromString(a);
                return new Vector3(rgb[0] / 256.0f, rgb[1] / 256.0f, rgb[2] / 256.0f);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // 从一个字符串里面构造颜色向量
        public static Color GetColorFromString(string a)
        {
            return GetColorFromVector3(GetColorVec3FromString(a));
        }

        // 从字符串中构造一个int数组
        private static int[] GetIntArrayFromString(string a)
        {
            string[] hexValuesSplit = a.Split(' ');
            int[] toReturn = new int[hexValuesSplit.Length];
            for (int i = 0; i < hexValuesSplit.Length; i++)
            {
                // 把一个字符串转成int值
                toReturn[i] = Convert.ToInt32(hexValuesSplit[i], 16);
            }
            return toReturn;
        }

        // 变得不透明，传入一个参数，单位还是1/256
        public static Vector4 transparentLessColor(Vector4 color,float factor)
        {
            return new Vector4(color.x, color.y, color.z, Math.Min(color.w + factor / 256.0f, 1.0f));
        }

        // 变得不透明，传入一个参数，单位还是1/256
        public static Vector4 transparentLessColor(Color color, float factor)
        {
            return new Vector4(color.r, color.g, color.b, Math.Min(color.a + factor / 256.0f, 1.0f));
        }

        // 变得透明，传入一个参数，单位还是1/256
        public static Vector4 transparentMoreColor(Vector4 color, float factor)
        {
            return new Vector4(color.x, color.y, color.z, Math.Max(color.w - factor / 256.0f, 0.0f));
        }

        // 变得透明，传入一个参数，单位还是1/256
        public static Vector4 transparentMoreColor(Color color, float factor)
        {
            return new Vector4(color.r, color.g, color.b, Math.Max(color.a - factor / 256.0f, 0.0f));
        }

        // 淡化颜色，传入一个淡化的参数（意味着会变得更加明亮！）factor的每个单位代表着1/256
        public static Vector3 FadeAColor(Vector3 color, float factor)
        {
            return new Vector3(Math.Min(color.x + factor/256.0f, 1.0f)
                , Math.Min(color.y + factor / 256.0f, 1.0f)
                , Math.Min(color.z + factor / 256.0f, 1.0f));
        }

        // 淡化颜色，传入一个淡化的参数（意味着会变得更加明亮！）factor的每个单位代表着1/256
        public static Color FadeAColor(Color color,float factor)
        {
            return new Color(Math.Min(color.r + factor / 256.0f, 1.0f)
                , Math.Min(color.g + factor / 256.0f, 1.0f)
                , Math.Min(color.b + factor / 256.0f, 1.0f));
        }

        // 增强颜色，传入一个增强的参数（意味着会变得更加黑暗！）factor的每个单位代表着1/256
        public static Vector3 DeepAColor(Vector3 color, float factor)
        {
            return new Vector3(Math.Max(color.x - factor / 256.0f, 0.0f)
                , Math.Max(color.y - factor / 256.0f, 0.0f)
                , Math.Max(color.z - factor / 256.0f, 0.0f));
        }

        // 增强颜色，传入一个增强的参数（意味着会变得更加黑暗！）factor的每个单位代表着1/256
        public static Vector3 DeepAColor(Color color, float factor)
        {
            return new Vector3(Math.Max(color.r - factor / 256.0f, 0.0f)
                , Math.Max(color.g - factor / 256.0f, 0.0f)
                , Math.Max(color.b - factor / 256.0f, 0.0f));
        }

        // 从Vector3构造一个颜色
        public static Color GetColorFromVector3(Vector3 vec)
        {
            return new Color(vec.x, vec.y, vec.z);
        }

        // 从Vector4构造一个颜色
        public static Color GetColorFromVector4(Vector4 vec)
        {
            return new Color(vec.x, vec.y, vec.z, vec.w);
        }

        // 从颜色中获得Vector3
        public static Vector3 GetVector3FromColor(Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }

        // 从颜色中获得Vector4
        public static Vector4 GetVector4FromColor(Color color)
        {
            return new Vector4(color.r, color.g, color.b, color.a);
        }

        // 得到默认的字体样式（全默认,颜色黑色）
        public static GUIStyle GetDefaultTextStyle()
        {
            return GetDefaultTextStyle(blackColor);
        }

        // 得到默认的字体样式（颜色自己指定）
        public static GUIStyle GetDefaultTextStyle(Vector3 color)
        {
            return GetDefaultTextStyle(GetColorFromVector3(color), defaultFontSize);
        }

        // 得到默认的字体样式（颜色自己指定）
        public static GUIStyle GetDefaultTextStyle(Color color)
        {
            return GetDefaultTextStyle(color, defaultFontSize);
        }

        // 得到指定大小的默认字体样式
        public static GUIStyle GetDefaultTextStyle(Color color,int fontSize)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.fontStyle = FontStyle.Normal;
            style.font = microsoftYaHei;
            style.alignment = defaultAnchor;
            style.fontSize = fontSize;
            return style;
        }

        // 从GUI的位置转到屏幕坐标
        public static Vector2 GUIPositionToScreenPoint(Vector2 guiPoint)
        {
            return GUIUtility.GUIToScreenPoint(guiPoint);
        }

        // 从指定点以指定比例缩放GUI
        public static void ScaleGUIAroundPivot(Vector2 scale,Vector2 pivot)
        {
            GUIUtility.ScaleAroundPivot(scale, pivot);
        }

        // 从屏幕坐标到GUI点
        public static Vector2 ScreenPointToGUIPositon(Vector2 screenPoint)
        {
            return GUIUtility.ScreenToGUIPoint(screenPoint);
        }

        // 从屏幕矩形转到GUI的矩形
        public static Rect ScreenRectToGUIRect(Rect screenRect)
        {
            return GUIUtility.ScreenToGUIRect(screenRect);
        }

        // 根据具体的字的大小修正一下GUI的位置，最后的bool值决定是否解决颠倒效应，如果要解决颠倒效应，必须传入相机的pixelHeight
        public static Rect GetFixedRectDueToFontSize(Vector2 guiPosition, int fontSize = defaultFontSize, bool fixedTopDown = false, int cameraPixelHeight=0)
        {
            // 就算你设置了要解决颠倒效应，你也要传一个相机的高给我啊
            if ((!fixedTopDown) || cameraPixelHeight == 0)
                return new Rect(guiPosition.x - fontSize / 4,
                    guiPosition.y - fontSize / 4,
                    fontSize, 
                    fontSize);
            else
                return new Rect(guiPosition.x - fontSize / 4, 
                    cameraPixelHeight - (guiPosition.y - fontSize / 4), 
                    fontSize,
                    fontSize);
        }
        
        // 获得世界坐标，传入一个相机，然后直接获得修正过大小后的一个Rect (主要是方便任务系统？也可能用在别的地方)
        public static Rect GetFixedRectDirectlyFromWorldPosition(Vector3 worldPosition,Camera camera,int fontSize = defaultFontSize)
        {
            Vector3 guiPosition = camera.WorldToScreenPoint(worldPosition);
            return GetFixedRectDueToFontSize(
                new Vector2(guiPosition.x, guiPosition.y),
                fontSize,
                true,
                camera.pixelHeight);
        }


        // 注意下面这些Display开头的函数都要放在OnGUI()里面调用！@！@！￥！@￥！@￥！@%……！@￥……@#%&￥……*


        // 直接调字幕，显示在默认的位置。注意一定要在OnGUI()中调用该函数！！！
        public static void DisplaySubtitleInDefaultPosition(string subtitle,Camera camera)
        {
            DisplaySubtitleInDefaultPosition(subtitle, camera, DefaultFontSize);
        }

        // 以指定大小的字体显示默认位置的字幕
        public static void DisplaySubtitleInDefaultPosition(string subtitle,Camera camera,int fontSize)
        {
            GUIStyle style = GetDefaultTextStyle(subtitleNormalColor, fontSize);
            GUI.Label(
                new Rect(
                    new Vector2(0.0f, camera.pixelHeight * defaultSubtitleRatioHeight),
                    new Vector2(camera.pixelWidth, DefaultFontSize)
                    ), subtitle, style);
        }

        // 在指定位置显示内容
        public static void DisplayContentInGivenPosition(string subtitle,Rect positionRect)
        {
            DisplayContentInGivenPosition(subtitle, positionRect, DefaultFontSize);
        }

        // 在指定位置显示内容
        public static void DisplayContentInGivenPosition(string subtitle,Rect positionRect,int fontSize)
        {
            GUIStyle style = GetDefaultTextStyle(subtitleNormalColor, fontSize);
            GUI.Label(positionRect, subtitle, style);
        }

        // 在指定位置显示内容，并有指定样式
        public static void DisplayContentInGivenPosition(string subtile,Rect positionRect,GUIStyle style)
        {
            GUI.Label(positionRect, subtile, style);
        }

        // 默认的显示任务目标
        public static void DisplayMissionTargetDefault(string missionContent,Camera camera, bool inLeft = true)
        {
            if (inLeft)
            {
                DisplayContentInGivenPosition(missionContent,
                        new Rect(1.0f, camera.pixelHeight / 20.0f, DefaultFontSize * missionContent.Length, DefaultFontSize),
                        GetDefaultTextStyle(missionContentNormalCor));
            }
            else
            {
                DisplayContentInGivenPosition(missionContent,
                        new Rect(camera.pixelWidth-1.0f- DefaultFontSize * missionContent.Length, camera.pixelHeight / 20.0f, DefaultFontSize * missionContent.Length, DefaultFontSize),
                        GetDefaultTextStyle(missionContentNormalCor));
            }
        }

        // 显示字幕，用指定的文法！！！！！！！只有一行字幕传进来！加一个字体大小参数,再加一个高度的比例参数，默认是3/4
        public static void DisplaySubtitleInGivenGrammar(string subtitle, Camera camera,
            int fontSize = defaultFontSize, float subtitleRatioHeight = defaultSubtitleRatioHeight)
        {
            List<ColorTempMemory> colors;
            // 先进行文法编译
            string theTrueSubtitle = SubtitleParser.ParseALine(subtitle, out colors);
            // 四种颜色的GUIStyle
            GUIStyle styleYellow = GetDefaultTextStyle(yellowColor, fontSize);
            GUIStyle styleBlue = GetDefaultTextStyle(blueColor, fontSize);
            GUIStyle styleRed = GetDefaultTextStyle(redColor, fontSize);
            GUIStyle styleWhite = GetDefaultTextStyle(whiteColor, fontSize);
            // 先计算出来整行字幕的位置
            float startPositionX = camera.pixelWidth / 2 - theTrueSubtitle.Length * fontSize / 2;
            float positionY = camera.pixelHeight * subtitleRatioHeight;
            foreach (var color in colors)
            {
                int theLength = color.endIndex - color.startIndex + 1;
                float theStartPositionX = startPositionX + color.startIndex * fontSize;
                switch (color.color)
                {
                    case SubtitleParser.YELLOW:
                        GUI.Label(
                            new Rect(
                                new Vector2(theStartPositionX, positionY),
                                new Vector2(theLength * fontSize, fontSize)
                                ), theTrueSubtitle.Substring(color.startIndex, theLength), styleYellow
                            );
                        break;
                    case SubtitleParser.BLUE:
                        GUI.Label(
                            new Rect(
                                new Vector2(theStartPositionX, positionY),
                                new Vector2(theLength * fontSize, fontSize)
                                ), theTrueSubtitle.Substring(color.startIndex, theLength), styleBlue
                            );
                        break;
                    case SubtitleParser.RED:
                        GUI.Label(
                            new Rect(
                                new Vector2(theStartPositionX, positionY),
                                new Vector2(theLength * fontSize, fontSize)
                                ), theTrueSubtitle.Substring(color.startIndex, theLength), styleRed
                            );
                        break;
                    case SubtitleParser.WHITE:
                        GUI.Label(
                            new Rect(
                                new Vector2(theStartPositionX, positionY),
                                new Vector2(theLength * fontSize, fontSize)
                                ), theTrueSubtitle.Substring(color.startIndex, theLength), styleWhite
                            );
                        break;
                }
            }

        }

        // 显示字幕，用指定的文法！！！！！！！只有一行字幕传进来！
        public static void DisplaySubtitleInGivenGrammar(string subtitle,Camera camera)
        {
            DisplaySubtitleInGivenGrammar(subtitle, camera, defaultFontSize);
        }

        // 显示字幕，用指定的文法！！！！！！！只有一行字幕传进来！加一个字体大小参数
        public static void DisplaySubtitleInGivenGrammar(string subtitle, Camera camera, int fontSize)
        {
            DisplaySubtitleInGivenGrammar(subtitle, camera, fontSize, defaultSubtitleRatioHeight);
        }

        // 显示字幕，用指定的文法！！！！！！！只有一行字幕传进来！加一个字幕高度比例参数
        public static void DisplaySubtitleInGivenGrammar(string subtitle, Camera camera, float subtitleRatioHeight)
        {
            DisplaySubtitleInGivenGrammar(subtitle, camera, defaultFontSize, subtitleRatioHeight);
        }




        private static bool isFrameCounterStart = false;
        private static int frameCounter;
        public static bool canBeStopDisplaySubtitleInGivenGrammar = false; // 如果要用这个方法，推荐可以每帧获取一下这个bool值来判断,不要让他每帧都反复判断
        // 显示字幕，用指定的文法！！！！！！！只有一行字幕传进来！再加一个帧数！！这些参数都是可以从static get字段取得的！默认的是前面加default
        [Obsolete]   // 事实上，在每一帧都调用的OnGUI()里面设定停止条件是不现实的，所以已废弃，交给调用者实现停止条件。而且，就算是到达停止条件了，也会反复询问，很吃资源
        public static void DisplaySubtitleInGivenGrammar(string subtitle, Camera camera, int fontSize, float subtitleRatioHeight,int frames)
        {
            if (canBeStopDisplaySubtitleInGivenGrammar) return;
            if (!isFrameCounterStart)
            {
                frameCounter = frames;
                isFrameCounterStart = true;
            }
            else
            {
                if (frameCounter-- >= 0)
                {
                    DisplaySubtitleInGivenGrammar(subtitle, camera, fontSize, subtitleRatioHeight);
                }
                else
                {
                    canBeStopDisplaySubtitleInGivenGrammar = true;
                    return;
                }
            }
        }


    }
}
