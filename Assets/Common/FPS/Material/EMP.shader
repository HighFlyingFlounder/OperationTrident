/*
	Created by chenjd & JiGe!
	http://www.cnblogs.com/murongxiaopifu/
*/
Shader "chenjd/ForceField"
{
	Properties
	{
		_Color("_Color", Color) = (0,0,0,0)
		_ColorMultiplier("ColorMultiplier", Vector) = (0,0,0,0)
		_NoiseTex("NoiseTexture", 2D) = "white" {}
		_DistortStrength("DistortStrength", Range(0,1)) = 0.2
		_DistortTimeFactor("DistortTimeFactor", Range(0,1)) = 0.2
		_RimStrength("RimStrength",Range(0, 10)) = 2
		_IntersectPower("IntersectPower", Range(0, 3)) = 2
		_Alpha("Alpha", Range(0,1)) = 0
	}

		SubShader
		{
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			Tags
			{
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
			}

			GrabPass
			{
				"_GrabTempTex"
			}

		Pass
		{
			CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

			struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float3 normal : NORMAL;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 screenPos : TEXCOORD1;
			float4 grabPos : TEXCOORD2;
			float3 normal : NORMAL;
			float3 viewDir : TEXCOORD3;
		};

		sampler2D _GrabTempTex;
		float4 _GrabTempTex_ST;
		sampler2D _NoiseTex;
		float4 _NoiseTex_ST;
		float _DistortStrength;
		float _DistortTimeFactor;
		float _RimStrength;
		float _IntersectPower;
		float _Alpha;

		sampler2D _CameraDepthTexture;

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);

			o.grabPos = ComputeGrabScreenPos(o.vertex);

			o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);

			o.screenPos = ComputeScreenPos(o.vertex);

			COMPUTE_EYEDEPTH(o.screenPos.z);

			o.normal = UnityObjectToWorldNormal(v.normal);

			o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

			return o;
		}

		float4 _Color;
		float4 _ColorMultiplier;


		fixed4 frag(v2f i) : SV_Target
		{
			//获取已有的深度信息,此时的深度图里没有力场的信息
			//判断相交
			float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
			float partZ = i.screenPos.z;

			float diff = sceneZ - partZ;
			float intersect = (1 - diff) * _IntersectPower;

			//圆环
			float3 viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, i.vertex)));
			float rim = 1 - abs(dot(i.normal, normalize(i.viewDir))) * _RimStrength;
			float edgeGlowColor = max(intersect, rim);

			//扭曲
			float4 offset = tex2D(_NoiseTex, i.uv - _Time.xy * _DistortTimeFactor);
			i.grabPos.xy -= offset.xy * _DistortStrength;
			fixed4 bodyColor = tex2Dproj(_GrabTempTex, i.grabPos);

			//float4 col = _ColorMultiplier * _Color * edgeGlowColor + _ColorMultiplier * _Color * bodyColor + (0, 0, 0, _Alpha);
			float4 col = _ColorMultiplier * _Color * edgeGlowColor + bodyColor + (0, 0, 0, _Alpha);
			return col;

		}

		ENDCG
		}
		}
}