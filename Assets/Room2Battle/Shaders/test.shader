Shader "test"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
		_WaveTex("waveTexture", 2D) = "white" {}
		_WaveMaskTex("waveMaskTexture",2D) = "white"{}
		//_CameraYawAngle("cameraYawAngle",float) = 0
		//_MaxDistance("MaxDistance", float) = 0.02
	}
		SubShader
	{
		//Post Processing, No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _CameraDepthTexture;
	uniform sampler2D _WaveTex;
	uniform sampler2D _WaveMaskTex;
	uniform float _CameraYawAngle;
	//uniform float _MaxDistance;

	fixed4 frag(v2f i) : SV_Target
	{
	float nonLinearDepth = tex2D(_CameraDepthTexture, i.uv);
	float depth = Linear01Depth(nonLinearDepth);
	float tmpX = i.uv.x - 0.5f;
	float tmpY = i.uv.y - 0.5f;
	//用z算出像素射线的length
	float length = depth * sqrt(1 + tmpX * tmpX + tmpY* tmpY);
	//深度决定v，于是加随时间变化v的偏移，就能实现“往外扩散的波浪”
	//至于fix u还是V,就取决于条纹纹理是怎么ps的了
	float v_scale = 10.0f;


	//衰减系数
	float attFactor = 10.0f;
	float attenuation = 1.0f / (1.0f + attFactor *length );
	//color = attenuation * tex2D(_WaveTex, float2(0.5f, length * v_scale - _TexCoordOffset));
	//_Time是float4的内置变量
	float amplifiedDepthColor = clamp(1.0f - 40.0f * depth, 0, 1.0f);//增加深度的变化幅度（相当于把far-plane拉近
	//float baseColorScale = 0.7f;
	//float4 baseColor = float4(0.0f, 0.0f, 0.0f, 1.0f);// float4(baseColorScale, baseColorScale, baseColorScale, 1.0f)* float4(amplifiedDepthColor, amplifiedDepthColor, amplifiedDepthColor, 1.0f);
	float4 maskColor = tex2D(_WaveMaskTex, float2(0.5f, length * v_scale - _Time.y*1.0f));
	float4 depthColor = float4(0.1f, 0.3f, 0.5f, 1.0f);
	float4 waveColor = float4(0.5f, 0.5f, 0.5f, 1.0f);// tex2D(_WaveTex, i.uv);
	//深度图的一点颜色+感应波
	float4 color =  attenuation * (depthColor * amplifiedDepthColor+ maskColor * waveColor);

	/*if (length < _MaxDistance)
	{
		color = float4(depth,depth,depth,0.4f) + tex2D(_WaveTex,float2(depth,depth))* tex2D(_WaveMaskTex, float2(0.5f, length * v_scale - _TexCoordOffset));
	}
	else
	{
		float _Attactor = 50.0;
		float attenuation = 1.0f / (1.0f + _Attactor * length);
		color = attenuation * tex2D(_WaveTex, float2(0.5f, length * v_scale - _TexCoordOffset));
	}*/


	return color;
	}
		ENDCG
	}
	}
}
