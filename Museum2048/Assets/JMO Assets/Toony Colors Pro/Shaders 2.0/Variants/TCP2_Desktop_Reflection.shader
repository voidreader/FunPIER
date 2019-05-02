
// Toony Colors Pro+Mobile 2
// (c) 2014-2019 Jean Moreno


Shader "Hidden/Toony Colors Pro 2/Variants/Desktop Reflection"
{
	Properties
	{
		//TOONY COLORS
		_Color ("Color", Color) = (1,1,1,1)
		_HColor ("Highlight Color", Color) = (0.785,0.785,0.785,1.0)
		_SColor ("Shadow Color", Color) = (0.195,0.195,0.195,1.0)
		
		//DIFFUSE
		_MainTex ("Main Texture (RGB) Spec/Refl Mask (A) ", 2D) = "white" {}
		
		//TOONY COLORS RAMP
		[TCP2Gradient] _Ramp ("#RAMPT# Toon Ramp (RGB)", 2D) = "gray" {}
		_RampThreshold ("#RAMPF# Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("#RAMPF# Ramp Smoothing", Range(0.01,1)) = 0.1
		
		//BUMP
		_BumpMap ("#NORM# Normal map (RGB)", 2D) = "bump" {}
		
		//REFLECTION
		_Cube ("#REFL# Reflection Cubemap", Cube) = "_Skybox" {}
		_ReflectColor ("#REFL# Reflection Color (RGB) Strength (Alpha)", Color) = (1,1,1,0.5)
		_ReflSmoothness ("#REFL_U5# Reflection Smoothness", Range(0.0,1.0)) = 1
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#include "../Include/TCP2_Include.cginc"
		
		#pragma surface surf ToonyColors nodirlightmap 
		#pragma target 3.0
		
		#pragma shader_feature TCP2_DISABLE_WRAPPED_LIGHT
		#pragma shader_feature TCP2_RAMPTEXT
		#pragma shader_feature TCP2_BUMP
		#pragma shader_feature TCP2_REFLECTION TCP2_REFLECTION_MASKED
		#pragma shader_feature TCP2_U5_REFLPROBE
		
		//================================================================
		// VARIABLES
		
		fixed4 _Color;
		sampler2D _MainTex;
		
	#if TCP2_BUMP
		sampler2D _BumpMap;
	#endif
	#if TCP2_REFLECTION || TCP2_REFLECTION_MASKED
		#if !TCP2_U5_REFLPROBE
		samplerCUBE _Cube;
		#else
		fixed _ReflSmoothness;
		#endif
		fixed4 _ReflectColor;
	#endif
		
		struct Input
		{
			half2 uv_MainTex : TEXCOORD0;
	#if TCP2_BUMP
			half2 uv_BumpMap : TEXCOORD1;
	#endif
	#if TCP2_REFLECTION || TCP2_REFLECTION_MASKED
			float3 worldRefl;
		#if TCP2_BUMP
			INTERNAL_DATA
		#endif
		#if TCP2_U5_REFLPROBE
			float3 worldPos;
			float3 worldNormal;
		#endif
	#endif
		};
		
		//================================================================
		// SURFACE FUNCTION
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = c.a * _Color.a;
			
	#if TCP2_BUMP
			//Normal map
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	#endif
	#if TCP2_REFLECTION || TCP2_REFLECTION_MASKED
			
		#if TCP2_U5_REFLPROBE
			half3 eyeVec = IN.worldPos.xyz - _WorldSpaceCameraPos.xyz;
			#if TCP2_BUMP
			half3 worldNormal = reflect(eyeVec, WorldNormalVector(IN, o.Normal));
			#else
			half3 worldNormal = reflect(eyeVec, IN.worldNormal);
			#endif
			
			float oneMinusRoughness = _ReflSmoothness;
			fixed3 reflColor = fixed3(0,0,0);
			#if UNITY_SPECCUBE_BOX_PROJECTION
			half3 worldNormal0 = BoxProjectedCubemapDirection (worldNormal, IN.worldPos, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax);
			#else
			half3 worldNormal0 = worldNormal;
			#endif
			half3 env0 = Unity_GlossyEnvironment (UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, worldNormal0, 1-oneMinusRoughness);
			
			#if UNITY_SPECCUBE_BLENDING
			const float kBlendFactor = 0.99999;
			float blendLerp = unity_SpecCube0_BoxMin.w;
			UNITY_BRANCH
			if (blendLerp < kBlendFactor)
			{
				#if UNITY_SPECCUBE_BOX_PROJECTION
				half3 worldNormal1 = BoxProjectedCubemapDirection (worldNormal, IN.worldPos, unity_SpecCube1_ProbePosition, unity_SpecCube1_BoxMin, unity_SpecCube1_BoxMax);
				#else
				half3 worldNormal1 = worldNormal;
				#endif
				
				half3 env1 = Unity_GlossyEnvironment (UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1,unity_SpecCube0), unity_SpecCube1_HDR, worldNormal1, 1-oneMinusRoughness);
				reflColor = lerp(env1, env0, blendLerp);
			}
			else
			{
				reflColor = env0;
			}
			#else
			reflColor = env0;
			#endif
			reflColor *= 0.5;
		#else
			#if TCP2_BUMP
			half3 worldRefl = WorldReflectionVector(IN, o.Normal);
			#else
			half3 worldRefl = IN.worldRefl;
			#endif
			fixed4 reflColor = texCUBE(_Cube, worldRefl);
		#endif
		#if TCP2_REFLECTION_MASKED
			reflColor.rgb *= c.a;
		#endif
			reflColor.rgb *= _ReflectColor.rgb * _ReflectColor.a;
			o.Emission += reflColor.rgb;
	#endif
		}
		
		ENDCG
		
	}
	
	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector"
}