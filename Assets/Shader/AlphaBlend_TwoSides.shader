Shader "iPhone/AlphaBlend_TwoSides" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
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

		}
	}
}
}
