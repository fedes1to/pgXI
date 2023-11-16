// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Legacy Shaders/Transparent/Diffuse Multiplied" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Mult ("Multiplier", Float) = 1.0
}

SubShader {
	Cull Off
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 200

CGPROGRAM
#pragma surface surf Lambert alpha:fade

sampler2D _MainTex;
fixed4 _Color;
float _Mult;

struct Input {
    float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * float4(_Color.rgb * _Mult, _Color.a);
    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
}

Fallback "Legacy Shaders/Transparent/VertexLit"
}