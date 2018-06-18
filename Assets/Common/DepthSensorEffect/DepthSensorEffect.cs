using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一种向外界发射“深度感应波”的感应器的特效（有点像虐杀原形2的那个迷之感应）
public class DepthSensorEffect : MonoBehaviour
{
    //unity editor里绑定Depth Sensor Post processing shader
    public Shader m_Shader;

    //texture是可以直接绑定在shader上的（因为定义了shader property）
    //但是为了防止忘了，在脚本也挂一个Texture吧
    public Texture m_Texture;

    //post processing script绑定的Camera
    private Camera m_Camera = null;

    //用于Blit的shader的material
    private Material m_postProcessMat=null;

    //特效是否打开（按H切换）
    private bool m_IsDepthSensorEnabled = false;

    //用于纹理坐标的偏移
    private float m_TexCoordOffset=0.0f;

    // Use this for initialization
    void Start ()
    {
        //设置camera的深度贴图模式（可以选择一些简化版的G-Buffer供shader使用）
        //这里使得Camera渲染出深度图。搜索Camera's Depth Texture
        //因为想用post processing，所以就只能用z-buffer除个系数转成radius了
        m_Camera = GetComponent<Camera>();
        m_Camera.depthTextureMode = DepthTextureMode.Depth;
        m_Camera.backgroundColor = Color.black;

        //生成post processing shader对应的材质
        m_postProcessMat = new Material(m_Shader);
    }
	
	// Update is called once per frame
	void Update ()
    {
		//按下H来打开／深度感应器（depth sensor）模式
        if(Input.GetKeyDown(KeyCode.H))
        {
            if(m_IsDepthSensorEnabled)
            {
                m_Camera.clearFlags = CameraClearFlags.Skybox;
                m_IsDepthSensorEnabled = false;
                m_TexCoordOffset = 0.0f;//重置随时间变化的纹理坐标偏移
            }
            else
            {
                m_Camera.clearFlags = CameraClearFlags.Color;
                m_IsDepthSensorEnabled = true;
            }
        }

	}

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (m_IsDepthSensorEnabled)
        {
            m_TexCoordOffset += Time.deltaTime * 0.3f;
            //m_TexCoordOffset -= (int)m_TexCoordOffset;

            //把间隔条纹纹理及其uv偏移传进去
            m_postProcessMat.SetTexture("waveTex", m_Texture);
            m_postProcessMat.SetFloat("_TexCoordOffset", m_TexCoordOffset);

            //Graphics.Blit是用给定shader把src RenderTexture复制到dest
            //所以可以当作是post processing的draw call
            Graphics.Blit(src, dest, m_postProcessMat);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
