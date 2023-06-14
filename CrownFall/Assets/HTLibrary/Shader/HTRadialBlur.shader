Shader "HTLibrary/RadialBlur"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "" {}
		_radius ("BlurRadius", Float) = 1.0
		_weight ("Weight", Float) = 0.93
		_density("Density", Float) = 1.0
		_sight("Sight", Float) = 0.5
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
	float _radius;
	float _weight;
	float _density;
	float _sight;
	 
	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 blurVector : TEXCOORD1;
	};
		
	v2f vert( appdata_img v )
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy =  v.texcoord.xy;
		o.blurVector = float2(0.5, 0.5) - v.texcoord.xy;
		
		return o; 
	}
	
	half4 frag(v2f i) : SV_Target 
	{
		half4 original = tex2D(_MainTex, i.uv);
		float sightDist = length(float2(0.5, 0.5) - i.uv);
		if (sightDist <= _sight)
			return original;
		
		float curWeight = 1.0f;
		half4 fragColor = original;
		float2 deltaTextCoord = i.blurVector * (_radius * 0.01);
		float2 coord = i.uv - (deltaTextCoord * 3.0);
		for(int i = 0; i < 5; i++)
		{
			coord += deltaTextCoord;

			half4 texel = tex2D(_MainTex, coord);
			fragColor = lerp(fragColor, texel, curWeight);
			curWeight *= _weight;
		}

		float sightRatio = clamp((sightDist - _sight) / (0.5 - _sight), 0.0, 1.0);
		return lerp(original, fragColor, _density * sightRatio);
	}

	ENDCG

	Subshader
	{
		Blend One Zero
		Lighting Off

		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}
	}

	Fallback off
}