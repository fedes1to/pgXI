// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'glstate_matrix_mvp' with 'UNITY_MATRIX_MVP'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'unity_World2Shadow' with 'unity_WorldToShadow'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "Mobile/Transparent-Shop" {
Properties {
 _ColorRili ("Rili Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" { }
}
SubShader { 
 LOD 150
 Tags { "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _ColorRili;

			struct appdata{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoordMulti0 : TEXCOORD0;
			};

			struct v2f{
				float4 position : POSITION;
				float2 texcoord0 : TEXCOORD0;
				float3 texcoord1 : TEXCOORD1;
				float3 texcoord2 : TEXCOORD2;
				float3 texcoord3 : TEXCOORD3;
			};

			v2f vert(appdata v){
				v2f o;
				float3 worldNormal_1;
				float3 tmpvar_2;
				float4 v_3;
				v_3.x = unity_WorldToObject[0].x;
				v_3.y = unity_WorldToObject[1].x;
				v_3.z = unity_WorldToObject[2].x;
				v_3.w = unity_WorldToObject[3].x;
				float4 v_4;
				v_4.x = unity_WorldToObject[0].y;
				v_4.y = unity_WorldToObject[1].y;
				v_4.z = unity_WorldToObject[2].y;
				v_4.w = unity_WorldToObject[3].y;
				float4 v_5;
				v_5.x = unity_WorldToObject[0].z;
				v_5.y = unity_WorldToObject[1].z;
				v_5.z = unity_WorldToObject[2].z;
				v_5.w = unity_WorldToObject[3].z;
				float3 tmpvar_6;
				tmpvar_6 = normalize(((
				  (v_3.xyz * v.normal.x)
				 + 
				  (v_4.xyz * v.normal.y)
				) + (v_5.xyz * v.normal.z)));
				worldNormal_1 = tmpvar_6;
				tmpvar_2 = worldNormal_1;
				float3 normal_7;
				normal_7 = worldNormal_1;
				float4 tmpvar_8;
				tmpvar_8.w = 1.0;
				tmpvar_8.xyz = normal_7;
				float3 res_9;
				float3 x_10;
				x_10.x = dot (unity_SHAr, tmpvar_8);
				x_10.y = dot (unity_SHAg, tmpvar_8);
				x_10.z = dot (unity_SHAb, tmpvar_8);
				float3 x1_11;
				float4 tmpvar_12;
				tmpvar_12 = (normal_7.xyzz * normal_7.yzzx);
				x1_11.x = dot (unity_SHBr, tmpvar_12);
				x1_11.y = dot (unity_SHBg, tmpvar_12);
				x1_11.z = dot (unity_SHBb, tmpvar_12);
				res_9 = (x_10 + (x1_11 + (unity_SHC.xyz * 
				  ((normal_7.x * normal_7.x) - (normal_7.y * normal_7.y))
				)));
				res_9 = max (((1.055 * 
				  pow (max (res_9, float3(0.0, 0.0, 0.0)), float3(0.4166667, 0.4166667, 0.4166667))
				) - 0.055), float3(0.0, 0.0, 0.0));
				o.position = UnityObjectToClipPos(v.vertex);
				o.texcoord0 = ((v.texcoordMulti0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
				o.texcoord1 = tmpvar_2;
				o.texcoord2 = mul(unity_ObjectToWorld, v.normal).xyz;
				o.texcoord3 = max (float3(0.0, 0.0, 0.0), res_9);
				return o;
			}

			float alpha;
			float4 _LightColor0;

			fixed4 frag(v2f i) : SV_TARGET{
				float3 tmpvar_1;
				float3 tmpvar_2;
				float3 tmpvar_3;
				float3 lightDir_4;
				float3 tmpvar_5;
				tmpvar_5 = _WorldSpaceLightPos0.xyz;
				lightDir_4 = tmpvar_5;
				tmpvar_3 = i.texcoord1;
				float4 tmpvar_6;
				tmpvar_6 = (tex2D (_MainTex, i.texcoord0) * _ColorRili);
				tmpvar_1 = _LightColor0.xyz;
				tmpvar_2 = lightDir_4;
				float4 c_7;
				float4 c_8;
				float diff_9;
				float tmpvar_10;
				tmpvar_10 = max (0.0, dot (tmpvar_3, tmpvar_2));
				diff_9 = tmpvar_10;
				c_8.xyz = ((tmpvar_6.xyz * tmpvar_1) * diff_9);
				c_8.w = 0.2;
				c_7.w = c_8.w;
				c_7.xyz = (c_8.xyz + (tmpvar_6.xyz * i.texcoord3));
				return c_7;
			}

			ENDCG
		}
	}
}