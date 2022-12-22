Shader "iPhone/AlphaBlend_Color_TwoSides" {
Properties {
	_TintColor ("Tint Color", Color) = (1,0.5,0.15,0.5)

	_MainTex ("Texture", 2D) = "white" {}
}

Category {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent"}
	
	
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off 
	Lighting Off 
	ZWrite Off
	//ColorMask RGB
	Color [_TintColor]
	
	SubShader {
		
		Pass {	
			SetTexture [_MainTex] {
				combine texture * primary DOUBLE, texture * primary
			}

		}
	}
}
}
