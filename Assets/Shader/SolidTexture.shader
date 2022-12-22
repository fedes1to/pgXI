Shader "iPhone/SolidTexture" {
    Properties {
        _TintColor ("Main Color", Color) = (0.8,0.8,0.8,1)
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
	
    SubShader {
		/*
		Material {

			Ambient [_TintColor]

		}
		

		ColorMaterial [_TintColor]
		*/
        Pass {

            Lighting Off
			Color [_TintColor]
            SetTexture [_MainTex] {
				
                Combine texture * primary
            }

        }
    }
} 