Shader "Custom/TransparentDoubleSided" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _LightMap ("Lightmap", 2D) = "white" {}
    }

    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

        // Make sure the shader is rendered behind other objects
        ZWrite Off

        // Set the blending mode to alpha blending
        Blend SrcAlpha OneMinusSrcAlpha

        // Render both front and back faces
        Cull Off

        // Set up the main pass
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv1 = v.uv1;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _LightMap;
            float4 _Color;

            fixed4 frag (v2f i) : SV_Target {
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                fixed4 lightMap = tex2D(_LightMap, i.uv1);
                return mainTex * lightMap * _Color;
            }
            ENDCG
        }
    }
}