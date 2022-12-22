
Shader "iPhone/CutEdgeAlphaTest" {
Properties {
	_Color ("Main Color", Color) = (1, 1, 1, 1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
}
	
SubShader {
	Tags { "Queue"="Geometry" "IgnoreProjector"="True" }
	Lighting off
	Cull Off
	
	Pass {  
		//Blend One One
		//Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater [_Cutoff]
		SetTexture [_MainTex] {
			constantColor [_Color]
			combine texture * constant, texture 
		}
	}
	
}

}
