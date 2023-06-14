Shader "Helltorch/ToonShading"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_Emission ("Emission", Range(0.0, 5.0)) = 0.0
		_MainTex ("Base (RGB) Illumin (A)", 2D) = "white" {}
		//_BumpMap ("Normalmap", 2D) = "bump" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}

		_DissCol ("Dissolve Color", Color) = (0.5,0.5,0.5,1)
		_DissEmis ("Dissolve Emission", Range(0.0, 5.0)) = 0.0
		_DissRate ("Dissolve Rate", Range(0.0, 1.0)) = 0.0
		_DissTex ("Dossolve Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Opaque" 
		}

		Cull Off
		LOD 400

		//-----
		CGPROGRAM
		#include "UnityCG.cginc"
	
		#pragma surface surf ToonRamp
		#pragma lighting ToonRamp exclude_path:prepass
	
		sampler2D _Ramp;
		half _Emission;
	
		half4 _DissCol;
		sampler2D _DissTex;
		half _DissEmis;
		half _DissRate;
	
		//-----
		inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
		{
#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
#endif // USING_DIRECTIONAL_LIGHT
	
			half d = dot (s.Normal, lightDir) * 0.5 + 0.5;
			half3 ramp = tex2D (_Ramp, float2 (d,d)).rgb;
	
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
			c.a = 0;
	
			return c;
		}
	
		//-----
		inline half4 LightingToonRamp_PrePass (SurfaceOutput s, half4 light)
		{
			half spec = light.a * s.Gloss;
			half d = Luminance (light.rgb) * 0.5;
			half3 ramp = tex2D (_Ramp, float2 (d,d)).rgb;
	
			half4 c;
			c.rgb = s.Albedo * ramp * light.rgb;
			c.a = 0;
	
			return c;
		}
	
		//-----
		sampler2D _MainTex;
		//sampler2D _BumpMap;
		float4 _Color;
		struct Input
		{
			float2 uv_MainTex : TEXCOORD0;
			//float2 uv_BumpMap;
		};
	
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c;
	
			//-----
			bool bUseDissolve = false;
			if (_DissRate > 0.0f)
			{
				if (_DissRate >= 1.0f)
					bUseDissolve = true;

				else
				{
					half4 dissC = tex2D (_DissTex, IN.uv_MainTex);
					if (dissC.a < _DissRate)
						bUseDissolve = true;
				}
			}
	
			//-----
			if (bUseDissolve)
			{
				c = tex2D (_DissTex, IN.uv_MainTex) * _DissCol;
				o.Albedo = c.rgb * (1.0 + _DissEmis);
			} 
			else
			{
				c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb * (1.0 + _Emission);
			}
	
			//-----
			o.Alpha = c.a;
		}

		//-----
		ENDCG
	}

	Fallback "Diffuse"
}