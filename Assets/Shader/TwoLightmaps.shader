Shader "DayNightCicle/TwoLightmaps" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" { }
 _LightMap ("Base (RGB) Gloss (A)", 2D) = "white" { }
 _LightMapNight ("Base (RGB) Gloss (A)", 2D) = "white" { }
 _Lerp ("Shininess", Range(0,1)) = 0
 _Color ("Color", Color) = (1,1,1,1)
 _Brightness ("Brightness", Range(0,4)) = 0
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}