Shader "iPhone/AlphaBlendOnScreenTop" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}


Category {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True"}
	
	SubShader {
		Lighting Off 
		Cull Off 
		ZTest Always 
		ZWrite Off 
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {	
			SetTexture [_MainTex] {
				combine texture
			}
			
		}
	}
	
}
}



