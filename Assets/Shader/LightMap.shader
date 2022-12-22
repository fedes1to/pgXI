Shader "iPhone/LightMap" 
{
	Properties 
	{
		//_Color ("Main Color", Color) = (0.8,0.8,0.8,1)
		_texBase ("MainTex", 2D) = "" {}
		_texLightmap ("LightMap", 2D) = "" {}
	}
	
	SubShader 
	{
		
		Pass
		{
			Tags { "RenderType"="Geometry" }

			Lighting Off
			Zwrite ON
			//Color [_Color]
			BindChannels 
			{
				Bind "Vertex", vertex
				Bind "texcoord", texcoord0
				Bind "texcoord1", texcoord1
			}
			
			SetTexture [_texBase] {combine texture }
			SetTexture [_texLightmap] { combine previous * texture}
		}
	}
}
