using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OperationTrident.Util
{
    public static class GUIUtil
    {
        public readonly static Vector3 black = new Vector3(0.0f, 0.0f, 0.0f);// 完全黑
        public readonly static Vector3 grey = new Vector3(0.5f, 0.5f, 0.5f);// 中间灰
        public readonly static Vector3 white = new Vector3(1.0f, 1.0f, 1.0f);// 完全白
        public readonly static Vector3 blue = new Vector3(0.0f, 0.0f, 1.0f);// 完全蓝
        public readonly static Vector3 red = new Vector3(1.0f, 0.0f, 0.0f);// 完全红
        public readonly static Vector3 green = new Vector3(0.0f, 1.0f, 0.0f);// 完全绿
        public readonly static Vector3 seaBlue = new Vector3(0.0f, 0.5f, 1.0f);// 深一点的蓝
        public readonly static Vector3 skyBlue = new Vector3(0.0f, 1.0f, 1.0f);// 浅一点的蓝
        public readonly static Vector3 deepPurple = new Vector3(0.5f, 0.0f, 1.0f);// 深一点的紫
        public readonly static Vector3 brightPurple = new Vector3(1.0f, 0.0f, 1.0f);// 亮一点的紫
        public readonly static Vector3 pink = new Vector3(1.0f, 0.0f, 0.5f);// 粉色
        public readonly static Vector3 orange = new Vector3(1.0f, 0.5f, 0.0f);// 橙色
        public readonly static Vector3 yellow = new Vector3(1.0f, 1.0f, 0.0f);// 黄色
        public readonly static Vector3 brightGreen = new Vector3(0.5f, 1.0f, 0.0f);// 亮一点的绿色
        public readonly static Vector3 deepGreen = new Vector3(0.0f, 1.0f, 0.5f);// 深一点的绿色

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

        // 淡化颜色，传入一个淡化的参数（意味着会变得更加明亮！）
        public static Vector3 FadeAColor(Vector3 color, float factor)
        {
            return new Vector3(Math.Max(color.x + factor, 1.0f)
                , Math.Max(color.y + factor, 1.0f)
                , Math.Max(color.z + factor, 1.0f));
        }

        // 增强颜色，传入一个增强的参数（意味着会变得更加黑暗！）
        public static Vector3 DeepAColor(Vector3 color, float factor)
        {
            return new Vector3(Math.Min(color.x - factor, 0.0f)
                , Math.Min(color.y - factor, 0.0f)
                , Math.Min(color.z - factor, 0.0f));
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

        public static GUIStyle GetDefaultTextStyle(Vector3 color)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = GetColorFromVector3(color);

            return style;
        }
    }
}
