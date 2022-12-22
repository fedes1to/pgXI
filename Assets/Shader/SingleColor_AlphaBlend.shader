Shader "iPhone/SingleColor_AlphaBlend" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)

}

Category {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent"}
	
	
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off 
	Lighting Off 
	ZWrite Off
	//ColorMask RGB
	Color [_TintColor]
	
	SubShader {
		
		Pass {	

			Lighting Off
            SetTexture [_MainTex] {
                Combine primary
            }
		}
	}
}
}
