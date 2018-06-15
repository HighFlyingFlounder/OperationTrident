Shader "Custom/test"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Contrast("Constrast",Range(0,4)) = 2
		_Brightness("Brightness",Range(0,2)) = 1
		_NightVisionColor("Night Vision Color",Color) = (1,1,1,1)
		_RandomValue("RandomValue",Float) = 0
		_distortion("distortion",Float) = 0.2
		_scale("scale",Float) = 0.8
		_VignetteTex("Vignette Texture", 2D) = "white"{}
		_ScanLineTileTex("Scan Line Tile Texture", 2D) = "white"{}
		_ScanLineTileAmount("Scan Line Tile Amount", Float) = 4.0
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_NoiseXSpeed("Noise X Speed", Float) = 100.0
		_NoiseYSpeed("Noise Y Speed",Float) = 100.0
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hit_fastest
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

			sampler2D _MainTex;
			uniform sampler2D _ScanLineTileTex;
			uniform sampler2D _NoiseTex;
			uniform sampler2D _VignetteTex;
			fixed _Contrast;
			fixed _Brightness;
			fixed _RandomValue;
			fixed _distortion;
			fixed _scale;
			fixed _NoiseYSpeed;
			fixed _NoiseXSpeed;
			fixed _ScanLineTileAmount;
			fixed4 _NightVisionColor;

			float2 barrelDistortion(float2 coord)
			{
				float2 h = coord.xy - float2(0.5, 0.5);
				float r2 = h.x * h.x + h.y * h.y;
				float f = 1.0 + r2 * (_distortion * sqrt(r2));

				return f * _scale * h + 0.5;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//桶型畸变算出的新uv
				half2 distortedUV = barrelDistortion(i.uv);
				fixed4 renderTex = tex2D(_MainTex, distortedUV);
				fixed4 vignetteTex = tex2D(_VignetteTex, distortedUV);

				half2 scanLinesUV = half2(i.uv.x * _ScanLineTileAmount, i.uv.y *_ScanLineTileAmount);
				fixed4 scanLineTex = tex2D(_ScanLineTileTex, scanLinesUV);

				//噪声贴图,使用sin函数？
				half2 noiseUV = half2(i.uv.x + (_RandomValue * _SinTime.z *_NoiseXSpeed), i.uv.y + (_Time.x * _NoiseYSpeed));
				fixed noiseTex = tex2D(_NoiseTex, noiseUV);

				//混合颜色
				fixed lum = dot(fixed3(0.299, 0.587, 0.114), renderTex.rgb);
				lum += _Brightness; // 补光
				fixed4 finalColor = (lum * 2) + _NightVisionColor;

				finalColor = pow(finalColor, _Contrast);
				finalColor *= vignetteTex;
				finalColor *= scanLineTex * noiseTex;

				return finalColor;
		}
		ENDCG
	}
	}
	FallBack "Diffuse"
}
