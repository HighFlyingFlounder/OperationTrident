Shader "DepthSensorEffect"
{
	Properties
	{
		//_MainTex ("Texture", 2D) = "white" {}
		_WaveTex ("waveTexture", 2D) = "waveTex" {}
		_TexCoordOffset("texCoordOffset",float) = 0
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _CameraDepthTexture;
			uniform sampler2D _WaveTex;
			uniform float _TexCoordOffset;

			fixed4 frag (v2f i) : SV_Target
			{
				float depth = Linear01Depth(tex2D(_CameraDepthTexture, i.uv));
				float tmpX = i.uv.x - 0.5f;
				float tmpY = i.uv.y - 0.5f;
				//用z算出像素射线的length
				float length = depth * sqrt(1 + tmpX * tmpX + tmpY* tmpY);
				//深度决定v，于是加随时间变化v的偏移，就能实现“往外扩散的波浪”
				//至于fix u还是V,就取决于条纹纹理是怎么ps的了
				float v_scale = 50.0f;

				//深度衰减系数（不然连无限远的天空盒都能被Sensed不太好吧）
				//attactor
				float attactor = 100.0f;
				float attenuation = 1.0f/(1.0f + attactor * length);


				fixed4 color = attenuation * tex2D(_WaveTex,float2(0.5f,length * v_scale - _TexCoordOffset));
				//fixed4 col = color;
				return color;
			}
			ENDCG
		}
	}
}
