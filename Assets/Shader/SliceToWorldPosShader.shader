Shader "Rilisoft/SliceToWorldPosShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
        _TopBorder ("TopBorder", Float) = 0
        _BottomBorder ("BottomBorder", Float) = 0
    }
    SubShader { 
        Tags { "RenderType"="Opaque" }
        Pass {
            Name "FORWARD"
            Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
            Cull Off
            GpuProgramID 63674
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 texcoord : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 color : TEXCOORD3;
                float4 vertex : SV_POSITION;
            };

            uniform float4 _MainTex_ST;
            uniform float4 _LightColor0;
            uniform float _TopBorder;
            uniform float _BottomBorder;
            uniform sampler2D _MainTex;

            v2f vert(appdata_t v) {
                v2f o;
                o.texcoord = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                float4x4 modelMatrix = unity_ObjectToWorld;
                o.normal = normalize(mul(float4(v.normal, 0), modelMatrix).xyz);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.color = float3(0, 0, 0); // Initialize color, adjust as needed
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
                float3 normal = normalize(i.normal);
                float3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float diff = max(0, dot(normal, lightDir));

                if (i.worldPos.y > _TopBorder || i.worldPos.y < _BottomBorder) {
                    discard;
                }

                float4 texColor = tex2D(_MainTex, i.texcoord);
                float3 diffuseColor = texColor.xyz * _LightColor0.xyz * diff;

                // Calculate the final color
                float3 finalColor = diffuseColor + texColor.xyz * i.color;

                return float4(finalColor, 1);
            }

            ENDCG
        }
    }
}