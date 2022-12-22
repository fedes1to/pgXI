Shader "iPhone/LightMap&Fog" 
{
	Properties 
	{
		_texBase ("MainTex", 2D) = "" {}
		_texLightmap ("LightMap", 2D) = "" {}
		fogColor ("Fog Color", Color) = (1,1,1,0.5)
		fogDensity ("Fog Density", Range (0, 0.05)) = 0
	}
	
	SubShader 
	{
		Pass
		{
			Tags { "RenderType"="Geometry" }

			Lighting OFF
			Zwrite ON
			
		 Fog
          {
            Mode Exp
            Color [fogColor]
            Density [fogDensity]
            Range -100,100
          }
			
			BindChannels 
			{
				Bind "Vertex", vertex
				Bind "texcoord", texcoord0
				Bind "texcoord1", texcoord1
				Bind "Color", color
			}
			
			SetTexture [_texBase] {combine texture}
			SetTexture [_texLightmap] { combine previous * texture }
		}
	}
}
