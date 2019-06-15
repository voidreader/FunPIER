// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit
// Supports vertex color
// No lightmap



Shader "RPG/RPGFont Text Detail" {
    Properties

    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D ) = "white" {}
        _DetailTex ("Detail (RGB)" , 2D  ) = "white" {}
        
        
    }
    SubShader

    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		
        Pass

        {

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
                
                
                struct vin
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
				};
                
                struct v2f
                {
                    fixed4 color : COLOR;
                    fixed4 pos : SV_POSITION;
                    fixed4 pack0 : TEXCOORD0;
                };
                
                sampler2D _MainTex;
                sampler2D _DetailTex;
                half4 _MainTex_ST;
                half4 _DetailTex_ST;
                fixed4 _Color;
                

                v2f vert(vin v)

                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                    o.pack0.zw = TRANSFORM_TEX(v.texcoord1, _DetailTex);
                    o.color = v.color;
                    
                    return o;

                }
                fixed4 frag(v2f i) : COLOR
                {

                    fixed4 tex = tex2D(_MainTex, i.pack0.xy);
                    fixed4 detailTex = tex2D(_DetailTex, i.pack0.zw);
                    fixed4 c = i.color *_Color * detailTex;
                    c.a *= tex.a;
                    return c;

                }

            ENDCG

        }

    }

}