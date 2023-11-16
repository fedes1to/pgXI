// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Legacy Shaders/Better Lightmapped/Transparent Diffuse" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_LightMap ("Lightmap (RGB)", 2D) = "black" {}
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 200

CGPROGRAM
#pragma surface surf Lambert alpha:fade

sampler2D _LightMap;
sampler2D _MainTex;
fixed4 _Color;

struct Input {
    float2 uv_MainTex;
	float2 uv2_LightMap;
};

void surf (Input IN, inout SurfaceOutput o) {
    float4 c = tex2D (_MainTex, IN.uv_MainTex) * float4(_Color.rgb / 2.5, _Color.a);
    //c.rgb = (c.rgb - 0.5) * (1.2) + 0.5;
    float4 lm = tex2D (_LightMap, IN.uv2_LightMap);
    lm.rgb = (lm.rgb - 0.5) * (1.2) + 0.5;
    lm.rgb *= 5;
    o.Albedo = c.rgb;
    o.Alpha = c.a;
	o.Emission = lm.rgb*o.Albedo.rgb;
}
ENDCG
}

Fallback "Legacy Shaders/Transparent/VertexLit"
}