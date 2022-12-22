Shader "iPhone/CutAlpha" {
Properties {
	//_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Alpha("Alpha", Range(0,1)) = 0.9
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" }
	//Blend SrcAlpha One
	//Blend SrcAlpha OneMinusSrcAlpha
	Blend SrcAlpha Zero
	AlphaTest Greater [_Alpha]
	ColorMask RGB
	Cull Off 
	Lighting Off 
	ZWrite Off 
	//Fog { Color (0,0,0,0) }
	/*
	BindChannels {
		//Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	*/
	//Color [_TintColor]
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				
				combine texture
			}
		}
	}
	
}
}
