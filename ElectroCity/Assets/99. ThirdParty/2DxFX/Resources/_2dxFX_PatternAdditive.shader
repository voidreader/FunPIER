﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//////////////////////////////////////////////
/// 2DxFX - 2D SPRITE FX - by VETASOFT 2017 //
/// http://vetasoft.store/2dxfx/            //
//////////////////////////////////////////////

Shader "2DxFX/Standard/PatternAdditive"
{
Properties
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_MainTex2 ("Pattern (RGB)", 2D) = "white" {}
_Alpha ("Alpha", Range (0,1)) = 1.0
_OffsetX ("OffsetX", Range (0,1)) = 0
_OffsetY ("OffsetY", Range (0,1)) = 0
_Color ("Tint", Color) = (1,1,1,1)
// required for UI.Mask
_StencilComp ("Stencil Comparison", Float) = 8
_Stencil ("Stencil ID", Float) = 0
_StencilOp ("Stencil Operation", Float) = 0
_StencilWriteMask ("Stencil Write Mask", Float) = 255
_StencilReadMask ("Stencil Read Mask", Float) = 255
_ColorMask ("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"}
ZWrite Off Blend SrcAlpha One Cull Off

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp] 
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{
CGPROGRAM

#pragma vertex vert
#pragma fragment frag

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};



v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;

return OUT;
}

sampler2D _MainTex;
sampler2D _MainTex2;
float4 _Color;
float _Alpha;
float _OffsetX;
float _OffsetY;

float4 frag(v2f IN) : COLOR
{
float4 t =  tex2D(_MainTex, IN.texcoord);
float4 t2 =  tex2D(_MainTex2, IN.texcoord+float2(_OffsetX,_OffsetY)) * IN.color;
t2.a = t2.a * t.a - _Alpha;
return t2;
}
ENDCG
}
}
Fallback "Sprites/Default"

}