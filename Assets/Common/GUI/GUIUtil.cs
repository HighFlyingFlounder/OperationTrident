using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OperationTrident.Util
{
    public class GUIUtil
    {
        //================================================================================
        //==========        一些可以获得的颜色，推荐直接获取Color后缀的         =============
        //==========        颜色Color类和Vector4是直接可以互换的 implicit隐式转换      =============
        //================================================================================


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


        public readonly static Color subtitleNormalColor = GetColorFromString("66 cc ff");
        public readonly static Color missionContentNormalColor = GetColorFromString("ee ee ee");
        public readonly static Color missionPointColor = new Vector4(184.0f / 256.0f, 184.0f / 256.0f, 184.0f / 256.0f, 1.0f);

        public readonly static Color subtitleYellow = new Color(1.0f, 1.0f, 0.7f);
        public readonly static Color subtitleRed = new Color(1.0f, 0.5f, 0.5f);
        public readonly static Color subtitleBlue = new Color(0.2f, 0.6f, 1.0f);
        public readonly static Color subtitleGreen = new Color(0.6f, 1.0f, 0.7f);


        private static float lastTime = Time.time;
        private enum Timer { DCIGPS,DMDD,DMTDS,DMTIMS1,DMTIMS2,DSIGG,DSsIGG,DSsIGGWT};
        private static float[] startTime = { Time.time, Time.time, Time.time, Time.time, Time.time, Time.time, Time.time, Time.time , Time.time , Time.time , Time.time , Time.time , Time.time };
        private static float FrameTime(Timer what)
        {
            float toReturn = Time.time - startTime[(int)what];
            startTime[(int)what] = Time.time;
            return toReturn;
        }
            
        private static void ResetFrame(Timer what)
        {
            startTime[(int)what] = Time.time;
        }
            


        // 默认的字体大小
        private const int defaultFontSize = 18;

        // 默认的字体对齐
        private const TextAlignment defaultAlignment = TextAlignment.Center;
        private const TextAnchor defaultAnchor = TextAnchor.MiddleCenter;

        // 字幕在屏幕的哪里：按比例算
        private const float defaultSubtitleRatioHeight = 4.0f / 5.0f;

        // 任务目标在屏幕左边多少，占屏幕宽的比例
        private const float defaultMissionTargetOffsetLeft = 1.0f / 40.0f;

        // 任务详细信息从屏幕的哪里开始啊，两个Offset值
        private const float defaultMissionDetailOffsetLeft = 1.0f / 20.0f;
        private const float defaultMissionDetailOffsetUp = 2.0f / 3.0f;
        private const float defaultMissionDetailInterval = 0.5f; // 任务细节每行显示的间隔

        //================================================================================
        //==========        一些可以获得的默认的参数         ===============================
        //================================================================================

        // 微软雅黑
        public readonly static Font microsoftYaHei = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", defaultFontSize);
        // 微软Sans Serif
        public readonly static Font microsoftSansSerif = Font.CreateDynamicFontFromOSFont("Microsoft Sans Serif", defaultFontSize);

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

        //================================================================================
        //==========        一些关于颜色的基本操作         ===============================
        //================================================================================

        /// <summary>
        /// 传入参数获得一种纯色  越接近0越黑，越接近1越白
        /// </summary>
        /// <param name="factor" type="float" constraint="0.0f<=factor<=1.0f">
        /// 将颜色的rgb值都设为factor
        /// </param>
        /// <returns type="Color">
        /// 返回一个纯色的颜色向量
        /// </returns>
        public static Color GetPureColor(float factor)
        {
            return GetColorFromVector3(new Vector3(factor, factor, factor));
        }

        /// <summary>
        /// 混合两个颜色
        /// </summary>
        /// <param name="colorA" type="Vector3" constraint="0.0f<=colorA<=1.0f" >
        /// 要混合的颜色A，是一个向量Vector3，代表颜色的rgb
        /// </param>
        /// <param name="colorB" type="Vector3" constraint="0.0f<=colorB<=1.0f">
        /// 要混合的颜色B，是一个向量Vector3，代表颜色的rgb
        /// </param>
        /// <returns type="Vector3">
        /// 返回混合后的颜色向量
        /// </returns>
        public static Vector3 MixTwoColor(Vector3 colorA, Vector3 colorB)
        {
            return new Vector3((colorA.x + colorB.x) / 2, (colorA.y + colorB.y) / 2, (colorA.z + colorB.z) / 2);
        }

        /// <summary>
        /// 混合两个颜色
        /// </summary>
        /// <param name="colorA" type="Color">
        /// 要混合的颜色A，是一个Color
        /// </param>
        /// <param name="colorB" type="Color">
        /// 要混合的颜色B，是一个Color
        /// </param>
        /// <returns type="Color">
        /// 返回混合后的颜色
        /// </returns>
        public static Color MixTwoColor(Color colorA,Color colorB)
        {
            return GetColorFromVector3(
                MixTwoColor
                (GetVector3FromColor(colorA), GetVector3FromColor(colorB)));
        }

        /// <summary>
        /// 从一个字符串里面构造颜色向量 格式"48 3F 1F"
        /// </summary>
        /// <param name="sourceString" type="string" 
        /// constraint="每两个字符用空格隔开,总长为8个字符，两个字符在00-FF之间，代表两位十六进制">
        /// 传进来的字符串
        /// </param>
        /// <returns type="Vector3">
        /// 返回一个颜色向量
        /// </returns>
        public static Vector3 GetColorVec3FromString(string sourceString)
        {
            try
            {
                int[] rgb = GetIntArrayFromString(sourceString);
                return new Vector3(rgb[0] / 256.0f, rgb[1] / 256.0f, rgb[2] / 256.0f);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 从一个字符串里面构造Color类型
        /// </summary>
        /// <param name="sourceString" type="string">
        /// 传进来的字符串
        /// </param>
        /// <returns type="Color">
        /// 返回一个颜色类
        /// </returns>
        public static Color GetColorFromString(string sourceString)
        {
            return GetColorFromVector3(GetColorVec3FromString(sourceString));
        }

        /// <summary>
        /// 从字符串中构造一个int数组，作为private函数，主要用于构造上述的颜色向量
        /// </summary>
        /// <param name="sourceString" type="string">
        /// 传进来的字符串
        /// </param>
        /// <returns type="int[]">
        /// 返回构造好的int数组
        /// </returns>
        private static int[] GetIntArrayFromString(string sourceString)
        {
            string[] hexValuesSplit = sourceString.Split(' ');
            int[] toReturn = new int[hexValuesSplit.Length];
            for (int i = 0; i < hexValuesSplit.Length; i++)
            {
                // 把一个字符串转成int值
                toReturn[i] = Convert.ToInt32(hexValuesSplit[i], 16);
            }
            return toReturn;
        }

        /// <summary>
        /// 将一个颜色变得不透明，传入一个参数，单位是1/256，
        /// 意味着每一个单位的factor会将颜色的rgba的a值增加1/256。
        /// factor太大的话会直接把颜色的a值设为1
        /// </summary>
        /// <param name="color" type="Vector4">
        /// 传进的颜色向量
        /// </param>
        /// <param name="factor" type="float" constraint="factor>=0.0f">
        /// 传进的不透明参数
        /// </param>
        /// <returns type="Vector4">
        /// 返回处理后变得不透明的颜色向量
        /// </returns>
        public static Vector4 TransparentLessColor(Vector4 color, float factor)
        {
            return new Vector4(color.x, color.y, color.z, Math.Min(color.w + factor / 256.0f, 1.0f));
        }

        /// <summary>
        /// 将一个颜色变得不透明，传入一个参数，单位是1/256，
        /// 意味着每一个单位的factor会将颜色的rgba的a值增加1/256。
        /// factor太大的话会直接把颜色的a值设为1
        /// </summary>
        /// <param name="color" type="Color">
        /// 传进来要处理的颜色
        /// </param>
        /// <param name="factor" type="float" constraint="factor>=0.0f">
        /// 传进来的透明参数
        /// </param>
        /// <returns type=Vector4>
        /// 返回一个颜色向量Vector4
        /// </returns>
        [Obsolete]
        public static Vector4 TransparentLessColor(Color color, float factor)
        {
            return new Vector4(color.r, color.g, color.b, Math.Min(color.a + factor / 256.0f, 1.0f));
        }

        /// <summary>
        /// 将一个颜色变得透明，传入一个参数，单位是1/256
        /// 意味着每一个单位的factor会将颜色的rgba的a值减少1/256。
        /// </summary>
        /// <param name="color" type="Vector4">
        /// 传进来要处理的颜色向量
        /// </param>
        /// <param name="factor" type="float" constraint="factor>=0.0f">
        /// 传进来的透明参数
        /// </param>
        /// <returns type="Vector4">
        /// 返回一个处理之后的颜色向量
        /// </returns>
        [Obsolete]
        public static Vector4 TransparentMoreColor(Vector4 color, float factor)
        {
            return new Vector4(color.x, color.y, color.z, Math.Max(color.w - factor / 256.0f, 0.0f));
        }

        /// <summary>
        /// 将一个颜色变得透明，传入一个参数，单位是1/256
        /// 意味着每一个单位的factor会将颜色的rgba的a值减少1/256。
        /// </summary>
        /// <param name="color" type="Color">
        /// 传进来要处理的颜色向量
        /// </param>
        /// <param name="factor" type="float" constraint="factor>=0.0f">
        /// 传进来的透明参数
        /// </param>
        /// <returns type="Color">
        /// 返回一个处理之后的颜色向量
        /// </returns>
        public static Vector4 TransparentMoreColor(Color color, float factor)
        {
            return new Vector4(color.r, color.g, color.b, Math.Max(color.a - factor / 256.0f, 0.0f));
        }

        /// <summary>
        /// 淡化颜色，使颜色变得更白
        /// </summary>
        /// <param name="color" type="Vector3">
        /// 传进来的颜色向量
        /// </param>
        /// <param name="factor" type="float" constraint="factor>=0.0f">
        /// 每个单位代表着1.0f/256.0f的rgb值的增加
        /// </param>
        /// <returns type="Vector3">
        /// 返回处理过后的颜色向量
        /// </returns>
        public static Vector3 FadeAColor(Vector3 color, float factor)
        {
            return new Vector3(Math.Min(color.x + factor / 256.0f, 1.0f)
                , Math.Min(color.y + factor / 256.0f, 1.0f)
                , Math.Min(color.z + factor / 256.0f, 1.0f));
        }

        /// <summary>
        /// 淡化颜色，使颜色变得更白
        /// </summary>
        /// <param name="color" type="Color">
        /// 传进来的颜色
        /// </param>
        /// <param name="factor" type="float" constraint="factor>=0.0f">
        /// 每个单位代表着1.0f/256.0f的rgb值的增加
        /// </param>
        /// <returns type="Color">
        /// 返回处理过后的颜色
        /// </returns>
        public static Color FadeAColor(Color color, float factor)
        {
            return new Color(Math.Min(color.r + factor / 256.0f, 1.0f)
                , Math.Min(color.g + factor / 256.0f, 1.0f)
                , Math.Min(color.b + factor / 256.0f, 1.0f));
        }

        /// <summary>
        /// 增强颜色，使颜色变得更黑
        /// </summary>
        /// <param name="color" type="Vector3"></param>
        /// <param name="factor" type="float" constraint="factor>=0.0f">
        /// 每个单位代表着1/256.0f的rgb值得减少
        /// </param>
        /// <returns type="Vector3"></returns>
        public static Vector3 DeepAColor(Vector3 color, float factor)
        {
            return new Vector3(Math.Max(color.x - factor / 256.0f, 0.0f)
                , Math.Max(color.y - factor / 256.0f, 0.0f)
                , Math.Max(color.z - factor / 256.0f, 0.0f));
        }

        /// <summary>
        /// 增强颜色，使颜色变得更黑
        /// </summary>
        /// <param name="color" type="Color"></param>
        /// <param name="factor" type="float" constraint="factor>=0.0f">
        /// 每个单位代表着1/256.0f的rgb值得减少
        /// </param>
        /// <returns type="Color"></returns>
        public static Color DeepAColor(Color color, float factor)
        {
            return new Color(Math.Max(color.r - factor / 256.0f, 0.0f)
                , Math.Max(color.g - factor / 256.0f, 0.0f)
                , Math.Max(color.b - factor / 256.0f, 0.0f));
        }

        /// <summary>
        /// 从Vector3构造一个颜色
        /// </summary>
        /// <param name="vec" type="Vector3"></param>
        /// <returns type="Color"></returns>
        public static Color GetColorFromVector3(Vector3 vec)
        {
            return new Color(vec.x, vec.y, vec.z);
        }

        /// <summary>
        /// 从Vector4构造一个颜色
        /// </summary>
        /// <param name="vec" type="Vector4"></param>
        /// <returns type="Color"></returns>
        [Obsolete]
        public static Color GetColorFromVector4(Vector4 vec)
        {
            return new Color(vec.x, vec.y, vec.z, vec.w);
        }

        /// <summary>
        /// 从颜色中获得Vector3
        /// </summary>
        /// <param name="color" type="Color"></param>
        /// <returns type="Vector3"></returns>
        public static Vector3 GetVector3FromColor(Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }

        /// <summary>
        /// 从颜色中获得Vector4
        /// </summary>
        /// <param name="color" type="Color"></param>
        /// <returns type="Vector4"></returns>
        [Obsolete]
        public static Vector4 GetVector4FromColor(Color color)
        {
            return new Vector4(color.r, color.g, color.b, color.a);
        }

        //================================================================================
        //==========        一些关于样式GUIStyle的基本操作         ===============================
        //================================================================================

        /// <summary>
        /// 得到默认的字体样式（全默认,颜色黑色）
        /// </summary>
        /// <returns type="GUIStyle"></returns>
        public static GUIStyle GetDefaultTextStyle()
        {
            return GetDefaultTextStyle(blackColor);
        }

        /// <summary>
        /// 得到指定颜色的默认字体样式
        /// </summary>
        /// <param name="color" type="Vector3"></param>
        /// <returns type="GUIStyle"></returns>
        [Obsolete]
        public static GUIStyle GetDefaultTextStyle(Vector3 color)
        {
            return GetDefaultTextStyle(GetColorFromVector3(color), defaultFontSize);
        }

        /// <summary>
        /// 得到指定颜色的默认字体样式
        /// </summary>
        /// <param name="color" type="Color"></param>
        /// <returns type="GUIStyle"></returns>
        public static GUIStyle GetDefaultTextStyle(Color color)
        {
            return GetDefaultTextStyle(color, defaultFontSize);
        }

        /// <summary>
        /// 得到指定字体大小、颜色的默认字体样式
        /// </summary>
        /// <param name="color" type="Color"></param>
        /// <param name="fontSize" type="int"></param>
        /// <returns type="GUIStyle"></returns>
        public static GUIStyle GetDefaultTextStyle(Color color, int fontSize)
        {
            return GetDefaultTextStyle(color, fontSize, defaultAnchor);
        }

        /// <summary>
        /// 得到指定字体大小，颜色，字体对齐方式的默认字体样式
        /// </summary>
        /// <param name="color" type="Color"></param>
        /// <param name="fontSize" type="int"></param>
        /// <param name="textAnchor" type="TextAnchor"></param>
        /// <returns type="GUIStyle"></returns>
        public static GUIStyle GetDefaultTextStyle(
            Color color,
            int fontSize, 
            TextAnchor textAnchor)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.fontStyle = FontStyle.Normal;
            style.font = microsoftSansSerif;
            style.alignment = textAnchor;
            style.fontSize = fontSize;
            return style;
        }

        //================================================================================
        //==========        一些坐标转换的操作         ===============================
        //================================================================================

        /// <summary>
        /// 从GUI的位置转到屏幕坐标
        /// </summary>
        /// <param name="guiPoint" type="Vector2"></param>
        /// <returns type="Vector2"></returns>
        public static Vector2 GUIPositionToScreenPoint(Vector2 guiPoint)
        {
            return GUIUtility.GUIToScreenPoint(guiPoint);
        }

        /// <summary>
        /// 从指定点以指定比例缩放GUI
        /// </summary>
        /// <param name="scale" type="Vector2"></param>
        /// <param name="pivot" type="Vector2"></param>
        public static void ScaleGUIAroundPivot(Vector2 scale, Vector2 pivot)
        {
            GUIUtility.ScaleAroundPivot(scale, pivot);
        }

        /// <summary>
        /// 从屏幕坐标到GUI位置
        /// </summary>
        /// <param name="screenPoint" type="Vector2"></param>
        /// <returns type="Vector2"></returns>
        public static Vector2 ScreenPointToGUIPositon(Vector2 screenPoint)
        {
            return GUIUtility.ScreenToGUIPoint(screenPoint);
        }

        /// <summary>
        /// 从屏幕矩形转到GUI的矩形
        /// </summary>
        /// <param name="screenRect" type="Rect"></param>
        /// <returns type="Rect"></returns>
        public static Rect ScreenRectToGUIRect(Rect screenRect)
        {
            return GUIUtility.ScreenToGUIRect(screenRect);
        }

        /// <summary>
        /// 根据具体的字的大小修正一下GUI的位置，
        /// 最后的bool值决定是否解决颠倒效应，如果要解决颠倒效应，必须传入相机的pixelHeight
        /// 建议：解决，并传入相机高
        /// </summary>
        /// <param name="guiPosition" type="Vector2"></param>
        /// <param name="fontSize" type="int" default="defaultFontSize">
        /// 默认字体大小
        /// </param>
        /// <param name="fixedTopDown" type="bool" default="false">
        /// 是否解决颠倒效应：就是GUI坐标的XY中原点是左上角，Y值往下为正。
        /// 如果设置为true的话，应在后面传入相机的高，如果不传入相机的高，那么结果和false是一样的
        /// </param>
        /// <param name="cameraPixelHeight" type="int" default="0">
        /// camera.pixelHeight
        /// </param>
        /// <returns type="Rect">
        /// 返回的矩形
        /// </returns>
        public static Rect GetFixedRectDueToFontSize(
            Vector2 guiPosition,
            int fontSize = defaultFontSize,
            bool fixedTopDown = false, 
            int cameraPixelHeight = 0)
        {
            // 传高+Bool
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

        /// <summary>
        /// 获得世界坐标，传入一个相机，然后直接获得修正过大小后的一个Rect 
        /// 主要是方便任务系统,任务目标点可以直接获得其在屏幕上显示的Rect
        /// </summary>
        /// <param name="worldPosition" type="Vector3"></param>
        /// <param name="camera" type="Camera"></param>
        /// <param name="fontSize" type="int"></param>
        /// <returns type="Rect"></returns>
        public static Rect GetFixedRectDirectlyFromWorldPosition(Vector3 worldPosition, Camera camera, int fontSize = defaultFontSize)
        {
            Vector3 guiPosition = camera.WorldToScreenPoint(worldPosition);
            return GetFixedRectDueToFontSize(
                new Vector2(guiPosition.x, guiPosition.y),
                fontSize,
                true,
                camera.pixelHeight);
        }


        // 下面这些Display开头的函数都要放在OnGUI()里面调用


        //================================================================================
        //==========        一些显示字幕的函数         ===============================
        //================================================================================

        /// <summary>
        /// 在默认的位置显示字幕。字幕居中
        /// </summary>
        /// <param name="subtitle" type="string">
        /// 要显示的字幕
        /// </param>
        /// <param name="camera" type="Camera">
        /// 在哪个相机
        /// </param>
        public static void DisplaySubtitleInDefaultPosition(string subtitle, Camera camera)
        {
            DisplaySubtitleInDefaultPosition(subtitle, camera, defaultFontSize, defaultSubtitleRatioHeight);
        }

        /// <summary>
        /// 以指定大小的字体显示默认位置的字幕。字幕居中
        /// </summary>
        /// <param name="subtitle" type="string">要显示的字幕</param>
        /// <param name="camera" type="Camera">显示字幕的相机</param>
        /// <param name="fontSize" type="int">字体大小</param>
        public static void DisplaySubtitleInDefaultPosition(string subtitle, Camera camera, int fontSize)
        {
            DisplaySubtitleInDefaultPosition(subtitle, camera, fontSize, defaultSubtitleRatioHeight);
        }

        /// <summary>
        /// 以指定大小的字体显示指定高度比例的字幕。字幕居中
        /// </summary>
        /// <param name="subtitle" type="string">要显示的字幕</param>
        /// <param name="camera" type="Camera">显示字幕的相机</param>
        /// <param name="fontSize" type="int">字体大小</param>
        /// <param name="subtitleRatioHeight" type="float">
        /// 字幕距屏幕上方的距离占整个屏幕的高的比例
        /// </param>
        public static void DisplaySubtitleInDefaultPosition(string subtitle, Camera camera, int fontSize, float subtitleRatioHeight)
        {
            GUIStyle style = GetDefaultTextStyle(subtitleNormalColor, fontSize);
            GUI.Label(
                new Rect(
                    new Vector2(0.0f, camera.pixelHeight * subtitleRatioHeight),
                    new Vector2(camera.pixelWidth, fontSize)
                    ), subtitle, style);
        }

        /// <summary>
        /// 以指定大小的字体指定的颜色显示指定高度比例的字幕。字幕居中
        /// </summary>
        /// <param name="subtitle" type="string"></param>
        /// <param name="camera" type="Camera"></param>
        /// <param name="fontSize" type="int"></param>
        /// <param name="subtitleRatioHeight" type="float"></param>
        /// <param name="color" type="Color"></param>
        public static void DisplaySubtitleInDefaultPosition(
            string subtitle,
            Camera camera, 
            int fontSize, 
            float subtitleRatioHeight,
            Color color)
        {
            GUIStyle style = GetDefaultTextStyle(color, fontSize);
            GUI.Label(
                new Rect(
                    new Vector2(0.0f, camera.pixelHeight * subtitleRatioHeight),
                    new Vector2(camera.pixelWidth, fontSize)
                    ), subtitle, style);
        }

        //================================================================================
        //==========        一些显示指定内容的默认参数         ===============================
        //================================================================================

        /// <summary>
        /// 在指定位置显示指定内容
        /// </summary>
        /// <param name="content" type="string">要显示的内容</param>
        /// <param name="positionRect" type="Rect">显示的位置</param>
        public static void DisplayContentInGivenPosition(string content, Rect positionRect)
        {
            DisplayContentInGivenPosition(content, positionRect, DefaultFontSize);
        }

        /// <summary>
        /// 在指定位置以指定字体大小显示指定内容
        /// </summary>
        /// <param name="content" type="string">要显示的内容</param>
        /// <param name="positionRect" type="Rect">显示的位置</param>
        /// <param name="fontSize" type="int">字体的大小</param>
        public static void DisplayContentInGivenPosition(string content, Rect positionRect, int fontSize)
        {
            GUIStyle style = GetDefaultTextStyle(subtitleNormalColor, fontSize);
            GUI.Label(positionRect, content, style);
        }

        /// <summary>
        /// 在指定位置显示内容，并有指定样式
        /// </summary>
        /// <param name="content" type="content">要显示的内容</param>
        /// <param name="positionRect" type="Rect">显示的位置</param>
        /// <param name="style" type="GUIStyle">传进来的字体样式</param>
        public static void DisplayContentInGivenPosition(string content, Rect positionRect, GUIStyle style)
        {
            GUI.Label(positionRect, content, style);
        }

        /// <summary>
        /// 在指定的比例位置显示内容，有指定颜色，字体大小等（解决Digit占两个位置的问题）
        /// </summary>
        /// <param name="content" type="string">要显示的内容</param>
        /// <param name="camera" type="Camera">显示内容的摄像头</param>
        /// <param name="offsetRatioX" type="float">字幕距离屏幕左边的距离占整个屏幕宽度的比例</param>
        /// <param name="offsetRatioY" type="float">字幕距离屏幕上面的距离占整个屏幕高度的比例</param>
        /// <param name="color" type="Color">字体颜色</param>
        /// <param name="fontSize" type="int">字体大小</param>
        /// <param name="textAnchor" type="TextAnchor">文本框锚点</param>
        public static void DisplayContentInGivenPosition(
            string content,
            Camera camera,
            float offsetRatioX,
            float offsetRatioY,
            Color color,
            int fontSize=defaultFontSize,
            TextAnchor textAnchor = defaultAnchor)
        {
            // 当前计算字母位的漂移距离
            float nowXOffset = 0.0f;
            for(int i = 0; i < content.Length; i++)
            {

                if (IsDigit(content[i]))
                {
                    GUI.Label(
                        new Rect(
                            new Vector2(
                                offsetRatioX * camera.pixelWidth + nowXOffset,
                                offsetRatioY * camera.pixelHeight),
                            new Vector2(fontSize / 2, fontSize)
                            ), "" + content[i], GetDefaultTextStyle(color, fontSize, textAnchor)
                            );
                    nowXOffset += fontSize / 2.0f;
                }
                else
                {
                    GUI.Label(
    new Rect(
        new Vector2(
            offsetRatioX * camera.pixelWidth + nowXOffset,
            offsetRatioY * camera.pixelHeight),
        new Vector2(fontSize, fontSize)
        ), "" + content[i], GetDefaultTextStyle(color, fontSize, textAnchor)
        );
                    nowXOffset += fontSize;
                }
            }
        }
        
        
        //================================================================================
        //==========        一些关于任务目标的显示函数         ===============================
        //================================================================================

        /// <summary>
        /// 在默认的位置显示任务目标，以默认的颜色
        /// </summary>
        /// <param name="missionContent" type="string">任务目标的内容</param>
        /// <param name="camera" type="Camera">显示的摄像头</param>
        /// <param name="inLeft" type="bool">是否靠左显示</param>
        public static void DisplayMissionTargetDefault(string missionContent, Camera camera, bool inLeft = true)
        {
            DisplayMissionTargetDefault(missionContent, camera, missionContentNormalColor, inLeft, defaultFontSize);
        }

        /// <summary>
        /// 默认的显示任务目标，有指定的颜色，字体大小。
        /// </summary>
        /// <param name="missionContent" type="string">任务目标内容</param>
        /// <param name="camera" type="Camera">显示的相机</param>
        /// <param name="color" type="Color">字体颜色</param>
        /// <param name="inLeft" type="bool">是否靠左显示</param>
        /// <param name="fontSize" type="int">字体大小</param>
        public static void DisplayMissionTargetDefault(string missionContent, Camera camera,
            Color color, bool inLeft = true,int fontSize=defaultFontSize)
        {
            if (inLeft)
            {
                DisplayContentInGivenPosition(missionContent,
                        new Rect(
                            defaultMissionTargetOffsetLeft
                            *camera.pixelWidth, 
                            camera.pixelHeight / 20.0f + fontSize, 
                            fontSize * missionContent.Length, fontSize),
                        GetDefaultTextStyle(color, fontSize, TextAnchor.LowerLeft));
            }
            else
            {
                DisplayContentInGivenPosition(missionContent,
                        new Rect(camera.pixelWidth - defaultMissionTargetOffsetLeft * camera.pixelWidth - fontSize * missionContent.Length, camera.pixelHeight / 20.0f + fontSize, fontSize * missionContent.Length, fontSize),
                        GetDefaultTextStyle(color, fontSize, TextAnchor.LowerRight));
            }
        }

        private static float frameTimerDMTDS = -0.3f;  // 用来计时的计时器
        private static int missionContentCounterDMTDS = 0;  // 要显示的任务目标的第几个字
        private static string rememberStringDMTDS = string.Empty;  // 记住上一帧的任务显示目标，时刻对比
        private static bool hasRememberStringInitDMTDS = false;  // 是否已经初始化记录字符串
        /// <summary>
        /// 默认的显示任务目标，按顺序以指定的时间间隔从左到右显示出来
        /// </summary>
        /// <param name="missionContent" type="string">任务目标内容</param>
        /// <param name="camera" type="Camera">显示的相机</param>
        /// <param name="color" type="Color">字体颜色</param>
        /// <param name="interval" type="float" default="0.5f">每个字显示的时间间隔</param>
        /// <param name="fontSize" type="int" default="defaultFontSize">字体大小</param>
        /// <param name="inLeft" type="bool">是否靠左显示</param>
        public static void DisplayMissionTargetDefaultSequently(
            string missionContent,
            Camera camera, 
            Color color,
            float interval = 0.5f, 
            int fontSize = defaultFontSize, 
            bool inLeft = true)
        {
            // 记录上一帧的字符串
            if (!hasRememberStringInitDMTDS)
            {
                hasRememberStringInitDMTDS = true;
                rememberStringDMTDS = missionContent;
            }
            // 如果任务目标出现了变化
            else if (missionContent != rememberStringDMTDS)
            {
                ResetFrame(Timer.DMTDS);
                missionContentCounterDMTDS = 0;
                frameTimerDMTDS = -0.3f;
            }
            if (missionContentCounterDMTDS >= missionContent.Length - 1)
            {
                missionContentCounterDMTDS = missionContent.Length - 1;
                frameTimerDMTDS = 0;
            }
            else
            {
                frameTimerDMTDS += FrameTime(Timer.DMTDS);
                if (frameTimerDMTDS > 0)
                {
                    frameTimerDMTDS = -interval;
                    missionContentCounterDMTDS++;
                }
            }
            if (inLeft)
            {
                for (int i = 0; i <= missionContentCounterDMTDS; i++)
                {
                    DisplayContentInGivenPosition(
                            "" + missionContent[i],
                            new Rect(
                                new Vector2(defaultMissionTargetOffsetLeft * camera.pixelWidth + i * fontSize, camera.pixelHeight / 15.0f),
                                new Vector2(fontSize, fontSize)),GetDefaultTextStyle(color,fontSize)
                            );
                }
            }
            else
            {
                for (int i = 0; i <= missionContentCounterDMTDS; i++)
                {
                    DisplayContentInGivenPosition(
                            "" + missionContent[i],
                            new Rect(
                                new Vector2(camera.pixelWidth-defaultMissionTargetOffsetLeft * camera.pixelWidth - i * fontSize, 
                                camera.pixelHeight / 15.0f),
                                new Vector2(fontSize, fontSize)), GetDefaultTextStyle(color, fontSize)
                            );
                }
            }
            rememberStringDMTDS = missionContent;
        }

        /// <summary>
        /// 显示任务点
        /// </summary>
        /// <param name="targetPosition">目标任务点</param>
        /// <param name="camera">传入相机</param>
        /// <param name="color">显示的颜色，可以直接获得missionPointColor</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="labelOffsetHeight">目标点显示有多高</param>
        public static void DisplayMissionPoint(
            Vector3 targetPosition,
            Camera camera,
            Color color,
            int fontSize = defaultFontSize,
            float labelOffsetHeight=0.0f
            )
        {
            //Vector3 point = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0); // 屏幕中心
            //Ray ray = camera.ScreenPointToRay(point); // 在摄像机所在位置创建射线
            //Vector3 direction1 = ray.direction; // 摄像头的方向
            //Vector3 direction2 = targetPosition - camera.transform.position; // 到摄像头的方向
            //// 如果物体大方向在人视线背后的话，就不显示了
            //if (Vector3.Dot(direction1, direction2) <= 0) return;
            //float nowDistance = Vector3.Distance(targetPosition,
            //         camera.transform.position);
            //targetPosition = new Vector3(targetPosition.x, targetPosition.y + labelOffsetHeight, targetPosition.z);
            //GUIStyle style = GetDefaultTextStyle(color, fontSize: 18);
            //Rect rect = GetFixedRectDirectlyFromWorldPosition(targetPosition, camera);
            //// 指定颜色
            //GUI.Label(rect, (int)nowDistance + "m\n●", style);
            float nowDistance = Vector3.Distance(targetPosition,camera.transform.position);
            DisplayWorldPointInScreen(targetPosition, camera, (int)nowDistance + "m\n●", color, fontSize, labelOffsetHeight);
        }


        /// <summary>
        /// 在屏幕上显示世界上的某个位置显示内容
        /// </summary>
        /// <param name="targetPosition">世界坐标</param>
        /// <param name="camera">传入的相机</param>
        /// <param name="content">要显示的内容</param>
        /// <param name="color">颜色</param>
        /// <param name="fontSize">显示的字体大小</param>
        /// <param name="labelOffsetHeight">距离有多高</param>
        public static void DisplayWorldPointInScreen(
            Vector3 targetPosition,
            Camera camera,
            string content,
            Color color,
            int fontSize=defaultFontSize,
            float labelOffsetHeight = 0.0f)
        {
            Vector3 point = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0); // 屏幕中心
            Ray ray = camera.ScreenPointToRay(point); // 在摄像机所在位置创建射线
            Vector3 direction1 = ray.direction; // 摄像头的方向
            Vector3 direction2 = targetPosition - camera.transform.position; // 到摄像头的方向
            // 如果物体大方向在人视线背后的话，就不显示了
            if (Vector3.Dot(direction1, direction2) <= 0) return;
            targetPosition = new Vector3(targetPosition.x, targetPosition.y + labelOffsetHeight, targetPosition.z);
            GUIStyle style = GetDefaultTextStyle(color, fontSize: fontSize);
            Rect rect = GetFixedRectDirectlyFromWorldPosition(targetPosition, camera);
            // 指定颜色
            GUI.Label(rect, content, style);
        }

        /// <summary>
        /// 是不是一个字母或者数字或者符号或者空格等一系列的占一个位置的字符
        /// </summary>
        /// <param name="a">传进来的字符</param>
        /// <returns type="bool"></returns>
        private static bool IsDigit(char a)
        {
            if (a >= 'A' && a <= 'Z') return false; 
            return a >= ' ' && a <= '~';
        }

        /// <summary>
        /// 根据是否是字母，数字来计算真正的字符串长度，如果是digit的话就只占半个Fontsize的位置，反之两个
        /// </summary>
        /// <param name="source" type="string">字符串</param>
        /// <param name="fontSize" type="int">字体大小</param>
        /// <returns type="int">返回字符串的总长度fontSize</returns>
        private static int GetTrueStringFontTotalSize(string source,int fontSize)
        {
            int toReturn = 0;
            foreach(char a in source)
            {
                toReturn += IsDigit(a) ? fontSize / 2 : fontSize;
            }
            return toReturn;
        }

        private static float frameTimerDCIGPS = -0.3f;
        private static int contentCounterDCIGPS = 0;
        private static string rememberStringDCIGPS = string.Empty;
        private static bool hasRememberStringInitDCIGPS = false;
        /// <summary>
        /// 默认的显示指定内容，按顺序以指定的时间间隔从左到右显示出来
        /// </summary>
        /// <param name="content" type="string">显示的内容</param>
        /// <param name="camera" type="Camera">显示的摄像头</param>
        /// <param name="color" type="Color">字体颜色</param>
        /// <param name="startPositionOffsetXRatio" type="float">内容离屏幕左边的距离占整个屏幕宽的比例</param>
        /// <param name="startPositionOffsetYRatio" type="float">内容离屏幕上面的距离占整个屏幕高的比例</param>
        /// <param name="interval" type="float" default="0.5f">每个字显示的间隔</param>
        /// <param name="fontSize" type="int" default="defaultFontSize">字体大小</param>
        /// <param name="withALine_" type="bool" default="false">是否在右边带一个_</param>
        public static void DisplayContentInGivenPositionSequently(
            string content,
            Camera camera,
            Color color,
            float startPositionOffsetXRatio,  // 内容离屏幕左边的距离占整个屏幕的比例
            float startPositionOffsetYRatio,  // 内容离屏幕上面的距离占整个屏幕的比例
            float interval = 0.5f,
            int fontSize = defaultFontSize,
            bool withALine_=false)
        {
            // 记录上一帧的字符串
            if (!hasRememberStringInitDCIGPS)
            {
                hasRememberStringInitDCIGPS = true;
                rememberStringDCIGPS = content;
            }
            // 如果任务目标出现了变化
            else if (content != rememberStringDCIGPS)
            {
                ResetFrame(Timer.DCIGPS);
                contentCounterDCIGPS = 0;
                frameTimerDCIGPS = -0.3f;
            }
            if (contentCounterDCIGPS >= content.Length - 1)
            {
                contentCounterDCIGPS = content.Length - 1;
                frameTimerDCIGPS = 0;
                //DisplayContentInGivenPosition(content,
                //    new Rect(
                //            new Vector2(startPositionOffsetXRatio * camera.pixelWidth,
                //            startPositionOffsetYRatio * camera.pixelHeight),
                //            new Vector2(fontSize, fontSize)), GetDefaultTextStyle(color, fontSize)
                //        );
                //goto End;
            }
            else
            {
                frameTimerDCIGPS += FrameTime(Timer.DCIGPS);
                if (frameTimerDCIGPS > 0)
                {
                    frameTimerDCIGPS = -interval;
                    contentCounterDCIGPS++;
                }
            }
            //float nowOffset = 0.0f;
            DisplayContentInGivenPosition(
                content.Substring(0, contentCounterDCIGPS + 1),
                camera,
                startPositionOffsetXRatio,
                startPositionOffsetYRatio,
                color,
                fontSize);
            //Debug.Log("withAVerticalLine    "+ withAVerticalLine);
            //Debug.Log("contentCounter1+1    " + (contentCounter1 + 1));
            //Debug.Log("content.Length    " + content.Length);

            if (withALine_)
            {
                if (contentCounterDCIGPS + 1 != content.Length)
                {
                    DisplayContentInGivenPosition(
                    "_",
                    camera,
                    startPositionOffsetXRatio +
                        GetTrueStringFontTotalSize(
                            content.Substring(0, contentCounterDCIGPS + 1), fontSize)
                            / (float)camera.pixelWidth,
                    startPositionOffsetYRatio,
                    color,
                    fontSize);
                }
            }
            //for (int i = 0; i <= contentCounter1; i++)
            //{

            //    // 如果是字母
            //    if (isDigit(content[i]))
            //    {
            //        nowOffset += fontSize / 2.0f;
            //        DisplayContentInGivenPosition(
            //                "" + content[i],
            //                new Rect(
            //                    new Vector2(startPositionOffsetXRatio * camera.pixelWidth + nowOffset,
            //                    startPositionOffsetYRatio * camera.pixelHeight),
            //                    new Vector2(fontSize/2, fontSize)), GetDefaultTextStyle(color, fontSize)
            //                );
            //    }
            //    else
            //    {
            //        nowOffset += fontSize / 1.0f;
            //        DisplayContentInGivenPosition(
            //                "" + content[i],
            //                new Rect(
            //                    new Vector2(startPositionOffsetXRatio * camera.pixelWidth + nowOffset,
            //                    startPositionOffsetYRatio * camera.pixelHeight),
            //                    new Vector2(fontSize, fontSize)), GetDefaultTextStyle(color, fontSize)
            //                );
            //    }
            //}
            //End:
            rememberStringDCIGPS = content;
        }

        private static List<int> frequentNumbers = new List<int>(); // 记录哪几个位置的字符是正确的
        private static float frameTimerDMTIMS1 = 0.0f; // 第一个计时器
        private static float frameTimerDMTIMS2 = 0.0f; // 第二个计时器
        private static string rememberStringDMTIMS = string.Empty; // 用来记录上一帧的任务目标
        private static bool hasRememberStringInitDMTIMS = false; // 记录字符串是否已经初始化
        private static char[] toDisplayDMTIMS; // 要显示的char数组

        /// <summary>
        /// 默认的显示任务目标，先乱后正。
        /// </summary>
        /// <param name="missionContent" type="string">任务目标的内容</param>
        /// <param name="camera" type="Camera">显示的摄像头</param>
        /// <param name="color" type="Color">任务目标的字体颜色</param>
        /// <param name="interval" type="float">任务目标每个字出现的速度</param>
        /// <param name="blingInterval">任务目标每个字闪烁的速度</param>
        /// <param name="fontSize">任务目标字体大小</param>
        /// <param name="inLeft">是否靠左显示</param>
        /// <param name="sequentClear">是否按顺序的变正，如果不然，就会随机变正</param>
        public static void DisplayMissionTargetInMessSequently(
            string missionContent,
            Camera camera,
            Color color,
            float interval = 0.5f,
            float blingInterval = 0.1f,
            int fontSize=defaultFontSize,
            bool inLeft=true,
            bool sequentClear=true)
        {
            if (frameTimerDMTIMS2 > blingInterval||frameTimerDMTIMS2==0.0f)
            {
                frameTimerDMTIMS2 = 0.0f;
                toDisplayDMTIMS = GetMessyCodeInFrequentChar(missionContent.Length).ToCharArray();
            }
            frameTimerDMTIMS2 += FrameTime(Timer.DMTIMS2);
            if (!hasRememberStringInitDMTIMS)
            {
                hasRememberStringInitDMTIMS = true;
                rememberStringDMTIMS = missionContent;
            }
            // 如果任务目标出现了变化
            else if (missionContent != rememberStringDMTIMS)
            {
                ResetFrame(Timer.DMTIMS1);
                frequentNumbers = new List<int>();
                frameTimerDMTIMS1 = 0.0f;
            }
            if (frequentNumbers.Count >= missionContent.Length)
            {
                DisplayMissionTargetDefault(missionContent, camera, color, inLeft, fontSize); // TODO:重载一下这个函数，接受字体大小和颜色,顺便重载一下Sequently那个，让他也调用这个Default
                return;
            }
            else
            {
                frameTimerDMTIMS1 += FrameTime(Timer.DMTIMS1);
                if (frameTimerDMTIMS1 >= interval)
                {
                    frameTimerDMTIMS1 = 0;
                    while (true)
                    {
                        int i = UnityEngine.Random.Range(0, missionContent.Length);
                        if (frequentNumbers.Contains(i))
                        {
                            continue;
                        }
                        else
                        {
                            frequentNumbers.Add(i);
                            break;
                        }
                    }
                }
                if (sequentClear)
                {
                    for (int i = 0; i < frequentNumbers.Count; i++)
                    {
                        toDisplayDMTIMS[i] = missionContent[i];
                    }
                }
                else
                {
                    for (int i = 0; i < frequentNumbers.Count; i++)
                    {
                        toDisplayDMTIMS[frequentNumbers[i]] = missionContent[frequentNumbers[i]];
                    }
                }
            }
            string toDisplayStr = new string(toDisplayDMTIMS);
            DisplayMissionTargetDefault(toDisplayStr, camera, color, inLeft, fontSize);
            rememberStringDMTIMS = missionContent;
            
        }

        /// <summary>
        /// 获得指定长度的乱码，例如  譆 @譆 `譆 €譆 白@ 雷@ 嘧 
        /// </summary>
        /// <param name="length" type="int">乱码字符串长度</param>
        /// <returns type="string">返回乱码</returns>
        public static string GetMessyCode(int length)
        {
            char charMin = char.MinValue;
            char charMax = char.MaxValue;
            int min = charMin;
            int max = charMax;
            string toReturn = string.Empty;
            for (int i = 0; i < length; i++)
            {
                char ran = (char)UnityEngine.Random.Range(min, max + 1);
                toReturn += ran;
            }
            return toReturn;
        }

        /// <summary>
        /// 获得指定长度的乱码，例如 as8ut29yth39rengi9q42jh80t2vhuj340-trhjnq03-4jtv023h409834h0-gjh4t0g
        /// 要注意的是，空格和一些符号也是包含在内的
        /// </summary>
        /// <param name="length" type="int">要获得的乱码长度</param>
        /// <returns type="string">返回指定长度的乱码</returns>
        public static string GetMessyCodeInFrequentChar(int length)
        {
            int min = 33;
            int max = 126;
            string toReturn = string.Empty;
            for(int i = 0; i < length; i++)
            {
                toReturn += (char)UnityEngine.Random.Range(min, max + 1);
            }
            return toReturn;
        }

        // 用指定的文法显示任务目标！！！！！TODO:NotImplement
        public static void DisplayMissionTargetInGivenGrammar(string missionContent, Camera camera, bool inLeft = true)
        {
            throw new NotImplementedException();
        }

        private static float frameTimerDMDD = 0.01f; // 一个计时器
        private static int missionDetailIndexDMDD = 0; // 任务细节数组的索引
        private static bool canBeStopDMDD = false; // 是否可以停止
        private static float minusFactorAlphaDMDD=0.0f; // 结束时变得透明的速度

        /// <summary>
        /// 显示任务时间地点等等的任务细节
        /// </summary>
        /// <param name="missionDetails" type="string[]">任务细节的数组</param>
        /// <param name="camera" type="Camera">显示的摄像头</param>
        /// <param name="color" type="Color">任务细节的字体颜色</param>
        /// <param name="fontSize" type="int">任务细节的字体大小</param>
        /// <param name="wordTransparentInterval" type="float" default="0.005f">字变得透明的间隔</param>
        /// <param name="wordAppearanceInterval" type="float" default="0.5f">字出现的间隔</param>
        /// <param name="lineSubsequentlyInterval" type="float" default="defaultMissionDetailInterval">每一行出现的间隔</param>
        public static void DisplayMissionDetailDefault(
            string[] missionDetails,
            Camera camera,
            Color color,
            int fontSize = defaultFontSize,
            float wordTransparentInterval = 0.005f,  // 字变得透明的速度
            float wordAppearanceInterval = 0.5f,  // 字出现的速度
            float lineSubsequentlyInterval = defaultMissionDetailInterval  // 每一行出现的速度
            )
        {
            if (canBeStopDMDD)
            {
                if (color.a - minusFactorAlphaDMDD == 0.0f) return;
                else
                {
                    color.a -= minusFactorAlphaDMDD;
                    minusFactorAlphaDMDD += wordTransparentInterval;
                }
            }
            frameTimerDMDD += FrameTime(Timer.DMDD);
            if (missionDetailIndexDMDD == 0)
            {
                DisplayContentInGivenPositionSequently(
                        missionDetails[missionDetailIndexDMDD],
                        camera,
                        color,
                        defaultMissionDetailOffsetLeft,
                        defaultMissionDetailOffsetUp + missionDetailIndexDMDD * fontSize / camera.pixelHeight,
                        interval: wordAppearanceInterval,
                        fontSize: fontSize,
                        withALine_:true);
            }
            else
            {
                for(int i = 0; i < Math.Min(missionDetailIndexDMDD,missionDetails.Length-1); i++)
                {
                    //DisplayContentInGivenPosition(missionDetails[i],
                    //    new Rect(new Vector2(defaultMissionDetailOffsetLeft * camera.pixelWidth,
                    //    defaultMissionDetailOffsetUp * camera.pixelHeight+fontSize*i),
                    //    new Vector2(missionDetails[i].Length * fontSize, fontSize)),GetDefaultTextStyle(color,fontSize,defaultAnchor)
                    //    );
                    DisplayContentInGivenPosition(
                        missionDetails[i],
                        camera, defaultMissionDetailOffsetLeft,
                        defaultMissionDetailOffsetUp + (float)fontSize*i / camera.pixelHeight,
                        color,
                        fontSize);
                }
                DisplayContentInGivenPositionSequently(
                        missionDetails[missionDetailIndexDMDD],
                        camera,
                        color,
                        defaultMissionDetailOffsetLeft,
                        defaultMissionDetailOffsetUp + (float)missionDetailIndexDMDD * (fontSize) / camera.pixelHeight,
                        interval: wordAppearanceInterval,
                        fontSize: fontSize,
                        withALine_:true);
            }
            //float startPositionX = defaultMissionDetailOffsetLeft * camera.pixelWidth;
            //float startPositionY = defaultMissionDetailOffsetUp * camera.pixelHeight;
            if (frameTimerDMDD > 
                lineSubsequentlyInterval+wordAppearanceInterval*3.0f*(missionDetails[missionDetailIndexDMDD].Length))
            {
                if (missionDetailIndexDMDD+1 >= missionDetails.Length)
                {
                    canBeStopDMDD = true;
                }
                else
                {
                    ++missionDetailIndexDMDD;
                    missionDetailIndexDMDD = Math.Min(missionDetailIndexDMDD, missionDetails.Length - 1);
                }
                frameTimerDMDD = 0.0f;
            }
            
        }


        public static void DisplayMissionDetailInGivenTime(
            string[] missionDetails,
            Camera camera,
            Color color,
            float lastTime,
            int fontSize = defaultFontSize,
            float wordTransparentInterval = 0.005f,  // 字变得透明的速度
            float wordAppearanceInterval = 0.5f,  // 字出现的速度
            float lineSubsequentlyInterval = defaultMissionDetailInterval ) // 每一行出现的速度)
        {

        }

        //================================================================================
        //==        显示字幕，要用指定的文法的                                         ===============================
        //==        文法示例：                                                        =========================
        //==        ^w你好,^r温蒂艾斯^w,我是^b爱思文迪^w,我们要找到^y托卡米克之心"       ===================================================
        //==        ^w作为标签，后面跟着的全是白色，直到遇到后面的标签，                  =====================================================================
        //==        ^w 白色， ^r 红色， ^b 蓝色， ^y 黄色，^k 黑色                      =====================================================================
        //===============================================================================

        /// <summary>
        /// 显示字幕，用指定的文法.
        /// 只有一行字幕传进来,加一个字体大小参数,再加一个高度的比例参数，默认是3/4
        /// 文法示例  "^w你好,^r温蒂艾斯^w,我是^b爱思文迪^w,我们要找到^y托卡米克之心"
        /// </summary>
        /// <param name="subtitle" type="string">显示得字幕内容</param>
        /// <param name="camera" type="Camera">显示得摄像头</param>
        /// <param name="fontSize" type="int">字幕的默认大小</param>
        /// <param name="subtitleRatioHeight" type="float">字幕距离屏幕上面的距离占整个屏幕高的比例</param>
        /// <param name="transparent">每一帧透明度变化速度</param>
        public static void DisplaySubtitleInGivenGrammar(
            string subtitle,
            Camera camera,
            int fontSize = defaultFontSize,
            float subtitleRatioHeight = defaultSubtitleRatioHeight,
            int transparent = 0
            )
        {
            List<ColorTempMemory> colors;
            // 先进行文法编译
            string theTrueSubtitle = SubtitleParser.ParseALine(subtitle, out colors);
            // 四种颜色的GUIStyle
            GUIStyle styleYellow = GetDefaultTextStyle(TransparentMoreColor(subtitleYellow,transparent), fontSize);
            GUIStyle styleBlue = GetDefaultTextStyle(TransparentMoreColor(subtitleBlue, transparent), fontSize);
            GUIStyle styleRed = GetDefaultTextStyle(TransparentMoreColor(subtitleRed, transparent), fontSize);
            GUIStyle styleGreen = GetDefaultTextStyle(TransparentMoreColor(subtitleGreen, transparent), fontSize);
            GUIStyle styleWhite = GetDefaultTextStyle(TransparentMoreColor(whiteColor, transparent), fontSize);
            GUIStyle styleBlack= GetDefaultTextStyle(TransparentMoreColor(blackColor, transparent), fontSize);
            // 先计算出来整行字幕的位置
            float startPositionX = camera.pixelWidth / 2 - theTrueSubtitle.Length * fontSize / 2;
            float positionY = camera.pixelHeight * subtitleRatioHeight;
            foreach (var color in colors)
            {
                int theLength = color.endIndex - color.startIndex + 1;
                float stringTotalLength = GetTrueStringFontTotalSize(theTrueSubtitle.Substring(color.startIndex, theLength), fontSize);
                float theStartPositionX;
                if (startPositionX == 0) theStartPositionX = startPositionX;
                else theStartPositionX = startPositionX + GetTrueStringFontTotalSize(theTrueSubtitle.Substring(0, color.startIndex), fontSize);
                switch (color.color)
                {
                    case SubtitleParser.YELLOW:
                        GUI.Label(
                            new Rect(
                                new Vector2(theStartPositionX, positionY),
                                new Vector2(stringTotalLength, fontSize)
                                ), theTrueSubtitle.Substring(color.startIndex, theLength), styleYellow
                            );
                        break;
                    case SubtitleParser.BLUE:
                        GUI.Label(
                            new Rect(
                                new Vector2(theStartPositionX, positionY),
                                new Vector2(stringTotalLength, fontSize)
                                ), theTrueSubtitle.Substring(color.startIndex, theLength), styleBlue
                            );
                        break;
                    case SubtitleParser.RED:
                        GUI.Label(
                            new Rect(
                                new Vector2(theStartPositionX, positionY),
                                new Vector2(stringTotalLength, fontSize)
                                ), theTrueSubtitle.Substring(color.startIndex, theLength), styleRed
                            );
                        break;
                    case SubtitleParser.GREEN:
                        GUI.Label(
                            new Rect(
                                new Vector2(theStartPositionX, positionY),
                                new Vector2(stringTotalLength, fontSize)
                                ), theTrueSubtitle.Substring(color.startIndex, theLength), styleGreen
                            );
                        break;
                    case SubtitleParser.WHITE:
                        GUI.Label(
                            new Rect(
                                new Vector2(theStartPositionX, positionY),
                                new Vector2(stringTotalLength, fontSize)
                                ), theTrueSubtitle.Substring(color.startIndex, theLength), styleWhite
                            );
                        break;
                    case SubtitleParser.BLACK:
                        GUI.Label(
                            new Rect(
                                new Vector2(theStartPositionX, positionY),
                                new Vector2(stringTotalLength, fontSize)
                                ), theTrueSubtitle.Substring(color.startIndex, theLength), styleBlack
                            );
                        break;
                }
            }

        }

        /// <summary>
        /// 用指定的文法显示字幕
        /// </summary>
        /// <param name="subtitle" type="string">字幕内容</param>
        /// <param name="camera" type="Camera">显示得摄像头</param>
        public static void DisplaySubtitleInGivenGrammar(string subtitle, Camera camera)
        {
            DisplaySubtitleInGivenGrammar(subtitle, camera, defaultFontSize);
        }

        /// <summary>
        /// 用指定的文法显示字幕
        /// </summary>
        /// <param name="subtitle" type="string">字幕内容</param>
        /// <param name="camera" type="Camera">显示得摄像头</param>
        /// <param name="fontSize" type="int">显示得字体大小</param>
        public static void DisplaySubtitleInGivenGrammar(string subtitle, Camera camera, int fontSize)
        {
            DisplaySubtitleInGivenGrammar(subtitle, camera, fontSize, defaultSubtitleRatioHeight);
        }

        /// <summary>
        /// 用指定的文法显示字幕
        /// </summary>
        /// <param name="subtitle" type="string">要显示的字幕</param>
        /// <param name="camera" type="Camera">摄像头</param>
        /// <param name="subtitleRatioHeight">字幕距离屏幕上面的距离占整个屏幕高的比例</param>
        public static void DisplaySubtitleInGivenGrammar(string subtitle, Camera camera, float subtitleRatioHeight)
        {
            DisplaySubtitleInGivenGrammar(subtitle, camera, defaultFontSize, subtitleRatioHeight);
        }


        private static bool isFrameCounterStartDSIGGIF = false;
        private static int frameCounterDSIGGIF;
        public static bool canBeStopDisplaySubtitleInGivenGrammarInFrames = false;
        /// <summary>
        /// 在给定的帧数里显示给定的字幕。没有淡入淡出的效果
        /// 如果想自行在程序外部判断是不是显示完毕的话
        /// 可以看canBeStopDisplaySubtitleInGivenGrammarInFrames是否为真
        /// </summary>
        /// <param name="subtitle" type="string">要显示的字幕</param>
        /// <param name="camera" type="Camera">摄像头</param>
        /// <param name="fontSize" type="int">字体大小</param>
        /// <param name="subtitleRatioHeight" type="float">字幕距离屏幕上方的距离占整个屏幕高的比例</param>
        /// <param name="frames" type="int">显示持续的帧数</param>
        [Obsolete]   
        public static void DisplaySubtitleInGivenGrammarInFrames(
            string subtitle,
            Camera camera,
            int fontSize, 
            float subtitleRatioHeight, 
            int frames)
        {
            if (canBeStopDisplaySubtitleInGivenGrammarInFrames) return;
            if (!isFrameCounterStartDSIGGIF)
            {
                frameCounterDSIGGIF = frames;
                isFrameCounterStartDSIGGIF = true;
            }
            else
            {
                if (frameCounterDSIGGIF-- >= 0)
                {
                    DisplaySubtitleInGivenGrammar(subtitle, camera, fontSize, subtitleRatioHeight);
                }
                else
                {
                    canBeStopDisplaySubtitleInGivenGrammarInFrames = true;
                    isFrameCounterStartDSIGGIF = false;
                    return;
                }
            }
        }

        private static float frameTimerDSIGG = 0;
        public static bool canBeStopDisplaySubtitleInGivenGrammarInSeconds = false;
        private static int transparentFactorDSIGG = 0;
        private static bool hasInitRememberStringDSIGG = false;
        private static string rememberStringDSIGG = string.Empty;
        /// <summary>
        /// 在给定的时间里显示字幕。
        /// 如果想要判断该函数是否已经显示完毕的话，可以获取canBeStopDisplaySubtitleInGivenGrammarInSeconds，
        /// 为真的时候显示完毕
        /// </summary>
        /// <param name="subtitle" type="string">显示的字幕</param>
        /// <param name="camera" type="Camera">摄像头</param>
        /// <param name="fontSize" type="int">字体大小</param>
        /// <param name="subtitleRatioHeight" type="float">字幕距离屏幕上方的距离占整个屏幕高度的比例</param>
        /// <param name="secondOfEachWord" type="float">字幕每个字显示的时间</param>
        /// <param name="secondOfEachLine" type="float">字幕这一行显示的时间，如果传这个值，那么就不采用上面那个</param>>
        public static void DisplaySubtitleInGivenGrammar(
            string subtitle,
            Camera camera,
            int fontSize, 
            float subtitleRatioHeight,
            float secondOfEachWord,
            float secondOfEachLine=0.0f)
        {
            // 最开始的时候调用，这时候还没有初始化记下字幕的变量
            if (!hasInitRememberStringDSIGG)
            {
                rememberStringDSIGG = subtitle;
                hasInitRememberStringDSIGG = true;
            }
            if (rememberStringDSIGG != subtitle)
            {
                ResetFrame(Timer.DSIGG);
                frameTimerDSIGG = 0;
                transparentFactorDSIGG = 0;
                canBeStopDisplaySubtitleInGivenGrammarInSeconds = false;
                rememberStringDSIGG = subtitle;
            }
            if (transparentFactorDSIGG > 255) return;
            if (canBeStopDisplaySubtitleInGivenGrammarInSeconds)
            {
                transparentFactorDSIGG += 4;
                frameTimerDSIGG = 0;
                DisplaySubtitleInGivenGrammar(subtitle, camera, fontSize, subtitleRatioHeight,transparent:transparentFactorDSIGG);

                return;
            }
            frameTimerDSIGG += FrameTime(Timer.DSIGG);
            transparentFactorDSIGG = Math.Min(transparentFactorDSIGG + 4, 255);
            DisplaySubtitleInGivenGrammar(subtitle, camera, fontSize, subtitleRatioHeight,transparent:255-transparentFactorDSIGG);
             //如果采用每一行的时间
            if (secondOfEachLine == 0.0f)
            {
                 //达到时间了
                if (frameTimerDSIGG >= secondOfEachWord * (subtitle.Length))
                {
                    canBeStopDisplaySubtitleInGivenGrammarInSeconds = true;
                    transparentFactorDSIGG = 0;
                    frameTimerDSIGG=0.0f;
                }
            }
            else
            {
                // 达到时间了
                if (frameTimerDSIGG >= secondOfEachLine)
                {
                    canBeStopDisplaySubtitleInGivenGrammarInSeconds = true;
                    transparentFactorDSIGG = 0;
                    frameTimerDSIGG = 0.0f;
                }
            }
        }

        private static float frameTimerDSsIGG = 0.0f;
        private static string[] rememberSubtitlesDSsIGG;
        private static int displayingSubtitlesIndexDSsIGG = 0;
        public static bool canBeStopDisplaySubtitlesInGivenGrammar = false;
        /// <summary>
        /// 按时间显示多行字幕，每次只显示一行。
        /// </summary>
        /// <param name="subtitles" type="string[]">显示的字幕的数组</param>
        /// <param name="camera" type="Camera">摄像头</param>
        /// <param name="fontSize" type="int">字体大小</param>
        /// <param name="subtitleRatioHeight" type="float">字幕离屏幕上方的距离占整个屏幕高的比例</param>
        /// <param name="secondOfEachWord" type="float">每个字显示的秒数</param>
        /// <param name="secondBetweenLine" type="float">行与行字幕之间的显示间隔秒数</param>
        public static void DisplaySubtitlesInGivenGrammar(
            string[] subtitles,
            Camera camera,
            int fontSize,
            float subtitleRatioHeight,
            float secondOfEachWord,
            float secondBetweenLine)
        {
            if (rememberSubtitlesDSsIGG == null)
            {
                rememberSubtitlesDSsIGG = subtitles;
            }
            // 要显示的总字幕发生了变化
            if (!IsEqual(rememberSubtitlesDSsIGG, subtitles))
            {
                ResetFrame(Timer.DSsIGG);
                frameTimerDSsIGG = 0.0f;
                displayingSubtitlesIndexDSsIGG = 0;
                canBeStopDisplaySubtitlesInGivenGrammar = false;
            }
            if (canBeStopDisplaySubtitlesInGivenGrammar) return;
            rememberSubtitlesDSsIGG = subtitles;
            DisplaySubtitleInGivenGrammar(
                subtitles[displayingSubtitlesIndexDSsIGG],
                camera,
                fontSize: fontSize,
                subtitleRatioHeight: subtitleRatioHeight,
                secondOfEachWord: secondOfEachWord);
            if (canBeStopDisplaySubtitleInGivenGrammarInSeconds)
            {
                frameTimerDSsIGG += FrameTime(Timer.DSsIGG);
                if (frameTimerDSsIGG >= secondBetweenLine)
                {
                    if (displayingSubtitlesIndexDSsIGG == subtitles.Length - 1)
                    {
                        canBeStopDisplaySubtitlesInGivenGrammar = true;
                    }
                    displayingSubtitlesIndexDSsIGG = Math.Min(displayingSubtitlesIndexDSsIGG + 1, subtitles.Length - 1);

                    frameTimerDSsIGG = 0;
                }
            }
        }

        private static float frameTimerDSsIGGWT = 0.0f;
        private static string[] rememberSubtitlesDSsIGGWT;
        private static int displayingSubtitlesIndexDSsIGGWT = 0;
        public static bool canBeStopDisplaySubtitlesInGivenGrammarWithTimeStamp = false;
        /// <summary>
        /// 给定每一行的时间和间隔的时间，显示指定的字幕
        /// </summary>
        /// <param name="subtitles">字幕数组</param>
        /// <param name="camera">相机</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="subtitleRatioHeight">字幕的相对高度</param>
        /// <param name="secondsOfEachLine">每一行字幕显示的时间</param>
        /// <param name="secondBetweenLine">行与行字幕显示的间隔时间</param>
        public static void DisplaySubtitlesInGivenGrammarWithTimeStamp(
            string[] subtitles,
            Camera camera,
            int fontSize,
            float subtitleRatioHeight,
            float[] secondsOfEachLine,
            float[] secondBetweenLine)
        {
            if (rememberSubtitlesDSsIGGWT == null)
            {
                rememberSubtitlesDSsIGGWT = subtitles;
            }
            if (subtitles == null)
            {
                Debug.Log("传入空字幕");
                return;
            }
            // 要显示的总字幕发生了变化
            if (!IsEqual(rememberSubtitlesDSsIGGWT,subtitles) )
            {
                ResetFrame(Timer.DSsIGGWT);
                frameTimerDSsIGGWT = 0.0f;
                displayingSubtitlesIndexDSsIGGWT = 0;
                canBeStopDisplaySubtitlesInGivenGrammarWithTimeStamp = false;
            }
            if (canBeStopDisplaySubtitlesInGivenGrammarWithTimeStamp&&frameTimerDSsIGGWT>3.0F) return;
            rememberSubtitlesDSsIGGWT = subtitles;
            DisplaySubtitleInGivenGrammar(
                subtitles[displayingSubtitlesIndexDSsIGGWT],
                camera,
                fontSize: fontSize,
                subtitleRatioHeight: subtitleRatioHeight,
                secondOfEachWord: 
                    secondsOfEachLine[displayingSubtitlesIndexDSsIGGWT]/
                        subtitles[displayingSubtitlesIndexDSsIGGWT].Length,
                secondOfEachLine: secondsOfEachLine[displayingSubtitlesIndexDSsIGGWT]);
            if (canBeStopDisplaySubtitleInGivenGrammarInSeconds)
            {
                frameTimerDSsIGGWT += FrameTime(Timer.DSsIGGWT);
                if (frameTimerDSsIGGWT >= secondBetweenLine[displayingSubtitlesIndexDSsIGGWT])
                {
                    if (displayingSubtitlesIndexDSsIGGWT >= subtitles.Length-1)
                    {
                        canBeStopDisplaySubtitlesInGivenGrammarWithTimeStamp = true;
                    }
                    displayingSubtitlesIndexDSsIGGWT = 
                        Math.Min(displayingSubtitlesIndexDSsIGGWT + 1, subtitles.Length-1);

                    frameTimerDSsIGGWT = 0;
                }
            }
        }

        private static bool IsEqual(string[] a,string[] b)
        {
            if (a.Length != b.Length) return false;
            for(int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

    }
}
