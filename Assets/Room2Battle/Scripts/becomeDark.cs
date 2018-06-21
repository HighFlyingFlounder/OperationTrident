using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace room2Battle
{
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

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            factor = Mathf.Clamp(factor, 0.0f, 1.0f);
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            material.SetFloat("_Factor", factor);
            Graphics.Blit(src, dest, material);
        }
    }
}
