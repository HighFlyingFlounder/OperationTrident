Shader "Custom/nightView" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Contrast("Constrast",Range(0,4)) = 2
		_Brightness("Brightness",Range(0,2)) = 1
		_NightVisionColor("Night Vision Color",Color) = (1,1,1,1)
		_RandomValue("RandomValue",Float) = 0
	    _distortion("distortion",Float) = 0.2
		_scale("scale",Float) = 0.8
		_VignetteTex("Vignette Texture", 2D) = "white"{}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_NoiseXSpeed("Noise X Speed", Float) = 100.0
		_NoiseYSpeed("Noise Y Speed",Float) = 100.0
		_Threshold("Threshold for determine intensty",Float) = 0.2
		_Threshold2("Threshold for determine intensty",Float) = 0.8

	}
		SubShader{
			Pass{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hit_fastest
			#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform sampler2D _NoiseTex;
		uniform sampler2D _VignetteTex;
		fixed _Contrast;
		fixed _Brightness;
		fixed _RandomValue;
		fixed _distortion;
		fixed _scale;
		fixed _NoiseYSpeed;
		fixed _NoiseXSpeed;
		fixed _Threshold;
		fixed _Threshold2;
		fixed4 _NightVisionColor;
		

		struct Input {
			float2 uv_MainTex;
		};

		//@brief桶型畸变
		//@param coord 传入uv
		float2 barrelDistortion(float2 coord)
		{
			float2 h = coord.xy - float2(0.5, 0.5);
			float r2 = h.x * h.x + h.y * h.y;
			float f = 1.0 + r2 * (_distortion * sqrt(r2));

			return f * _scale * h + 0.5;
		}

		fixed4 frag(v2f_img i) : COLOR
		{
			//桶型畸变算出的新uv
			//half2 distortedUV = barrelDistortion(i.uv);
			half2 distortedUV = i.uv;
			fixed4 renderTex = tex2D(_MainTex, distortedUV);
			fixed4 vignetteTex = tex2D(_VignetteTex, distortedUV);

			//噪声贴图,使用sin函数？
			half2 noiseUV = half2(i.uv.x + (_RandomValue * _SinTime.z *_NoiseXSpeed), i.uv.y + (_Time.x * _NoiseYSpeed));
			fixed noiseTex = tex2D(_NoiseTex, noiseUV);

			//得到灰度图
			fixed lum = dot(fixed3(0.299, 0.587, 0.114), renderTex.rgb);
			lum += _Brightness; // 补光
			//fixed4 finalColor = (lum * 2) + _NightVisionColor;
			fixed factor;
			if (lum > _Threshold)
			{
				factor = lerp(_Threshold2, 1, (lum - _Threshold) / (1.0 - _Threshold));
			}
			else
			{
				factor = lerp(0, _Threshold2, lum / _Threshold);
			}
			
			fixed4 light_ = fixed4(1.0f, 1.0, 1.0, 1.0);
			fixed4 finalColor = factor * light_;

			finalColor = pow(finalColor, _Contrast);
			finalColor *= vignetteTex;
			finalColor *=  noiseTex;

			return finalColor;
		}
		ENDCG
		}
	}
	FallBack "Diffuse"
}
