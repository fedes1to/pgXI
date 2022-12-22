Shader "iPhone/Additive_Bright" {
Properties {
	//_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	
	//Cull Off 
	Lighting Off ZWrite Off Fog { Color (0,0,0,0) }

	Color [_TintColor]
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture DOUBLE
			}

		}
	}
	
}
}
