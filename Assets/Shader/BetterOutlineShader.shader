Shader "Unlit/Outline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _OutColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutValue ("Outline width", Range(0.0, 0.3)) = 0.3
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
        
        // OUTLINE
        Pass
        {
            Tags {
                "Queue"="Transparent"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutColor;
            float _OutValue;
            
            float4 outline(float4 vertexPos, float outValue) {
                float4x4 scale = float4x4
                (
                    1+outValue, 0, 0, 0,
                    0, 1+outValue, 0, 0,
                    0, 0, 1+outValue, 0,
                    0, 0, 0, 1+outValue
                );
                return mul(scale, vertexPos);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                float4 vertexPos = outline(v.vertex, _OutValue);
                o.vertex = UnityObjectToClipPos(vertexPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return float4(_OutColor.r, _OutColor.g, _OutColor.b, _OutColor.a);
            }
            ENDCG
        }
        // TEXTURE
		Pass
		{
            Tags {
                "Queue"="Transparent + 1"
            }
            Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
