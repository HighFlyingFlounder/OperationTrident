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

        // 默认的字体大小
        private const int defaultFontSize = 12;

        // 默认的字体对齐
        private const TextAlignment defaultAlignment = TextAlignment.Center;

        private const TextAnchor defaultAnchor = TextAnchor.MiddleCenter;

        public readonly static Font microsoftYaHei = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", defaultFontSize);

        public static int FontSize
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

        // 混合两个颜色
        public static Vector3 MixTwoColor(Vector3 colorA, Vector3 colorB)
        {
            return new Vector3((colorA.x + colorB.x) / 2, (colorA.y + colorB.y) / 2, (colorA.z + colorB.z) / 2);
        }

        // 从一个字符串里面构造颜色向量 格式"48 3F 1F"
        public static Vector3 GetColorFromString(string a)
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
            GUIStyle style = new GUIStyle();
            style.normal.textColor = blackColor;
            style.fontStyle = FontStyle.Normal;
            style.font = microsoftYaHei;
            style.alignment = defaultAnchor;
            return style;
        }

        // 得到默认的字体样式（颜色自己指定）
        public static GUIStyle GetDefaultTextStyle(Vector3 color)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = GetColorFromVector3(color);
            style.fontStyle = FontStyle.Normal;
            style.font = microsoftYaHei;
            style.alignment = defaultAnchor;
            return style;
        }

        // 得到默认的字体样式（颜色自己指定）
        public static GUIStyle GetDefaultTextStyle(Color color)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.fontStyle = FontStyle.Normal;
            style.font = microsoftYaHei;
            style.alignment = defaultAnchor;
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
    }
}
