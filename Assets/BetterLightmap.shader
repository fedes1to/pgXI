// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Legacy Shaders/Better Lightmapped/Diffuse" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _LightMap ("Lightmap (RGB)", 2D) = "black" {}
}

SubShader {
    LOD 200
    Tags { "RenderType" = "Opaque" }
CGPROGRAM
#pragma surface surf Lambert nodynlightmap
struct Input {
  float2 uv_MainTex;
  float2 uv2_LightMap;
};
sampler2D _MainTex;
sampler2D _LightMap;
fixed4 _Color;
void surf (Input IN, inout SurfaceOutput o)
{
  float4 c = tex2D (_MainTex, IN.uv_MainTex) * (_Color / 2.5);
  //c.rgb = (c.rgb - 0.5) * (1.2) + 0.5;
  float4 lm = tex2D (_LightMap, IN.uv2_LightMap);
  lm.rgb = (lm.rgb - 0.5) * (1.2) + 0.5;
  lm.rgb *= 5;
  o.Albedo = c.rgb;
  o.Emission = lm.rgb*o.Albedo.rgb;
  o.Alpha = lm.a * _Color.a;
}
ENDCG
}
FallBack "Legacy Shaders/Lightmapped/VertexLit"
}