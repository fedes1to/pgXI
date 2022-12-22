Shader "iPhone/ScreenEffect" {
Properties {
	//_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	R("R", Range(0,1)) = 1
	G("G", Range(0,1)) = 1
	B("B", Range(0,1)) = 1
	_Alpha("Alpha", Range(0,1)) = 0.5
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {

		
	Blend SrcAlpha OneMinusSrcAlpha
	//Cull Off 
	Lighting Off 
	ZWrite Off
	ZTest Always
	//ColorMask RGB
	SubShader {
		Tags { "Queue"="Overlay"}
		Pass {	
			SetTexture [_MainTex] {
				constantColor ([R],[G],[B],[_Alpha])
				combine constant * texture DOUBLE
			}
			
		}
	}
}
}
