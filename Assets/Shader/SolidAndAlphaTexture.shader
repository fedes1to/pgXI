Shader "iPhone/SolidAndAlphaTexture" 
{
	Properties 
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_texBase ("MainTex", 2D) = "" {}
		_tex2 ("Texture2", 2D) = "" {}
	}
	
	SubShader 
	{
		Pass
		{
			Tags { "RenderType"="Geometry" }

			Lighting OFF
			Zwrite ON
			Color [_TintColor]
			BindChannels 
			{
				Bind "Vertex", vertex
				Bind "texcoord", texcoord0
				Bind "texcoord1", texcoord1
			}
			
			
			SetTexture [_tex2] { combine texture * primary}
			SetTexture [_texBase] {combine previous + texture}
		}
	}
}
