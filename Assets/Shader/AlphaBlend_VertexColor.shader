Shader "iPhone/AlphaBlend_VertexColor" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
}

Category {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent"}
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	Blend SrcAlpha OneMinusSrcAlpha
	//Cull Off 
	Lighting Off 
	ZWrite Off
	//ColorMask RGB
	
	SubShader {
		
		Pass {	
			SetTexture [_MainTex] {
				combine texture * primary
			}

		}
	}
}
}
