Shader "iPhone/SolidTextureBright" {
    Properties {
        _TintColor ("Main Color", Color) = (0.8,0.8,0.8,1)
        //_SpecColor ("Spec Color", Color) = (1,1,1,1)
        //_Emission ("Emmisive Color", Color) = (0,0,0,0)
        //_Shininess ("Shininess", Range (0.01, 1)) = 0.7
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
	
    SubShader {
        Pass {
			/*
            Material {
                Diffuse [_TintColor]
                Ambient [_TintColor]
                Shininess [_Shininess]
                Specular [_SpecColor]
                Emission [_Emission]
            }
            */
            Lighting Off
            //SeparateSpecular On
            SetTexture [_MainTex] {
                constantColor [_TintColor]
                Combine texture * constant Quad
            }
        }
    }
} 