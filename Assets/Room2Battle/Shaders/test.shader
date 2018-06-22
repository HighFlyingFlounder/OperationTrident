Shader "test"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
		_WaveTex("waveTexture", 2D) = "waveTex" {}
	_TexCoordOffset("texCoordOffset",float) = 0
		_MaxDistance("MaxDistance", float) = 0.02
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
	uniform float _TexCoordOffset;
	uniform float _MaxDistance;

	fixed4 frag(v2f i) : SV_Target
	{
		float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv));
	float tmpX = i.uv.x - 0.5f;
	float tmpY = i.uv.y - 0.5f;
	//用z算出像素射线的length
	float length = depth * sqrt(1 + tmpX * tmpX + tmpY* tmpY);
	//深度决定v，于是加随时间变化v的偏移，就能实现“往外扩散的波浪”
	//至于fix u还是V,就取决于条纹纹理是怎么ps的了
	float v_scale = 50.0f;
	fixed4 color;

	if (length < _MaxDistance)
	{
		color = tex2D(_WaveTex, float2(0.5f, length * v_scale - _TexCoordOffset));
	}
	else
	{
		float _Attactor = 50.0;
		float attenuation = 1.0f / (1.0f + _Attactor * length);
		color = attenuation * tex2D(_WaveTex, float2(0.5f, length * v_scale - _TexCoordOffset));
	}


	return color;
	}
		ENDCG
	}
	}
}
