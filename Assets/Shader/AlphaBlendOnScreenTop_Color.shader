Shader "iPhone/AlphaBlendOnScreenTop_Color" {
	Properties {
		R("R", Range(0,1)) = 1
		G("G", Range(0,1)) = 1
		B("B", Range(0,1)) = 1
		_Alpha("Alpha", Range(0,1)) = 0.5
		_MainTex ("Texture", 2D) = "white" {}
	}


Category {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True"}
		
	BindChannels {
		//Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" }
		
		Color ([R],[G],[B],[_Alpha])
		Lighting Off 
		//Cull Off 
		ZTest Always 
		ZWrite Off 
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {	
			SetTexture [_MainTex] {			
				combine texture * primary DOUBLE, texture*primary
			}

			
		}

	}
	
}
}



