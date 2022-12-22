Shader "iPhone/Additive_VertexColor" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	Blend SrcAlpha One
	Lighting Off ZWrite Off Fog { Color (0,0,0,0) }

	Color [_TintColor]
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}

		}
	}
	
}
}
