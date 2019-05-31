Shader "Custom/InvisibleMask" {

    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" { }
    }
  SubShader {
    // draw after all opaque objects (queue = 2001):
    Tags { "Queue"="Geometry+1" }
    Pass {
      Blend Zero One // keep the image behind it
    }
  } 
}