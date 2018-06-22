using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
    /// <summary>
    /// 后处理，挂在到主相机上，通过全部颜色 * 0.1实现变暗的效果
    /// </summary>
    public class becomeDark : MonoBehaviour
    {
        //变黑的程度,越小越黑
        public float factor = 0.1f;

        //unity editor里绑定Depth Sensor Post processing shader
        public Shader m_Shader;

        //绑定的相机
        private Camera m_Camera = null;

        //当前材质
        private Material curMaterial;

        //材质getter
        #region Properties
        Material material
        {
            get
            {
                if (curMaterial == null)
                {
                    curMaterial = new Material(m_Shader);
                    curMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return curMaterial;
            }
        }
        #endregion

        //保证factor的范围不要越界
        void Update()
        {
            factor = Mathf.Clamp(factor, 0.0f, 1.0f);
        }

        //直接改
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            material.SetFloat("_Factor", factor);
            Graphics.Blit(src, dest, material);
        }
    }
}
