//
// Author:
//   Based on the Unity3D built-in shaders
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://edelweissinteractive.com)
//

Shader "Decal/Transparent VertexLit" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

// 2/3 texture stage GPUs
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Offset -1,-1
	
	Alphatest Greater 0
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	ColorMask RGB
		
	// Non-lightmapped
	Pass {
		Tags { "LightMode" = "Vertex" }
		Material {
			Diffuse [_Color]
			Ambient [_Color]
		}
		Lighting On
		SetTexture [_MainTex] {
			Combine texture * primary DOUBLE, texture * primary
		} 
	}
	
	// Lightmapped, encoded as dLDR
	Pass {
		Tags { "LightMode" = "VertexLM" }
		
		BindChannels {
			Bind "Vertex", vertex
			Bind "normal", normal
			Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
			Bind "texcoord", texcoord1 // main uses 1st uv
		}
		SetTexture [unity_Lightmap] {
			matrix [unity_LightmapMatrix]
			constantColor [_Color]
			combine texture * constant
		}
		SetTexture [_MainTex] {
			combine texture * previous DOUBLE, texture * primary
		}
	}
	
	// Lightmapped, encoded as RGBM
	Pass {
		Tags { "LightMode" = "VertexLMRGBM" }
		
		BindChannels {
			Bind "Vertex", vertex
			Bind "normal", normal
			Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
			Bind "texcoord1", texcoord1 // unused
			Bind "texcoord", texcoord2 // main uses 1st uv
		}
		
		SetTexture [unity_Lightmap] {
			matrix [unity_LightmapMatrix]
			combine texture * texture alpha DOUBLE
		}
		SetTexture [unity_Lightmap] {
			constantColor [_Color]
			combine previous * constant
		}
		SetTexture [_MainTex] {
			combine texture * previous QUAD, texture * primary
		}
	}
}

// 1 texture stage GPUs
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	Offset -1,-1
	
	Alphatest Greater 0
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	ColorMask RGB
		
	Pass {
		Tags { "LightMode" = "Always" }
		Material {
			Diffuse [_Color]
			Ambient [_Color]
		}
		Lighting On
		SetTexture [_MainTex] {
			Combine texture * primary DOUBLE, texture * primary
		} 
	}	
}
}