Shader "iPhone/AlphaBlend_TwoSides_LightMap" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
	_texLightmap ("LightMap", 2D) = "" {}
}

Category {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent"}
	
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off 
	Lighting Off 
	ZWrite Off
	//ColorMask RGB
	
	SubShader {
		
		Pass {	
			SetTexture [_MainTex] {
				combine texture
			}
			SetTexture [_texLightmap] { combine previous * texture }
		}
	}
}
}
