Shader "iPhone/AlphaBlend_Color" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)

	_MainTex ("Texture", 2D) = "white" {}
}

Category {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent"}
	
	
	Blend SrcAlpha OneMinusSrcAlpha
	//Cull Off 
	Lighting Off 
	ZWrite Off
	//ColorMask RGB
	//Color [_TintColor]
	
	SubShader {
		
		Pass {	
			
			SetTexture [_MainTex] {
				constantColor [_TintColor]
				combine texture * constant DOUBLE, texture * constant
			}

		}
	}
}
}
