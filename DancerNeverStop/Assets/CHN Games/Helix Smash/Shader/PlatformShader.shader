Shader "Custom/PlatformShader"
{
	Properties{
		_Color("Color", Color) = (1,1,1)
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		CGPROGRAM
	#pragma surface surf Standard fullforwardshadows

		sampler2D _MainTex;

	struct Input
	{
		float2 uv_MainTex;
	};
	fixed4 _Color;
	UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
		// Albedo comes from a texture tinted by color
		o.Albedo = _Color.rgb;
		}
	ENDCG
	}
		FallBack "Mobile/Diffuse"
}