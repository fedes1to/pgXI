Shader "iPhone/LightMap_AmbientLight" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_texBase ("MainTex", 2D) = "" {}
		_texLightmap ("LightMap", 2D) = "" {}
	}
	
	SubShader 
	{
		Material {

			Ambient [_Color]

		} 
		
		Pass
		{
			Tags { "RenderType"="Geometry" }

			Lighting On
			Zwrite ON
			
			BindChannels 
			{
				Bind "Vertex", vertex
				Bind "texcoord", texcoord0
				Bind "texcoord1", texcoord1
			}
			
			SetTexture [_texBase] {combine texture * primary}
			SetTexture [_texLightmap] { combine previous * texture  DOUBLE}
		}
	}
}
