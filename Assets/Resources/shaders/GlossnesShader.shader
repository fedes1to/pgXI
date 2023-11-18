// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'unity_World2Shadow' with 'unity_WorldToShadow'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Rilisoft/GlossnesShader" {
Properties {
 _MainTex ("Albedo (RGB)", 2D) = "white" { }
 _GlossTex ("GlossTex (RGB)", 2D) = "white" { }
}
SubShader { 
 LOD 200
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		struct v2f {
			float4 texcoord0 : TEXCOORD0;
			float3 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			float texcoord4 : TEXCOORD4;
			float4 position : POSITION;
		};

		struct appdata_t {
			float3 normal : NORMAL;
			float4 vertex : POSITION;
			float4 texcoordMulti0 : TEXCOORD0;
			float4 texcoordMulti1 : TEXCOORD1;
		};

		sampler2D _MainTex, _GlossTex, _ShadowMapTexture;
		float4 _MainTex_ST, _GlossTex_ST, _LightColor0;

		v2f vert(appdata_t v)
		{
			v2f o;
			float3 worldNormal_1;
			float4 tmpvar_2;
			float4 tmpvar_3;
			float3 tmpvar_4;
			float4 tmpvar_5;
			float4 tmpvar_6;
			tmpvar_2 = UnityObjectToClipPos(v.vertex);
			tmpvar_3.xy = ((v.texcoordMulti0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
			tmpvar_3.zw = ((v.texcoordMulti0.xy * _GlossTex_ST.xy) + _GlossTex_ST.zw);
			float4 tmpvar_7;
			tmpvar_7 = mul(unity_ObjectToWorld, v.vertex);
			float4 v_8;
			v_8.x = unity_WorldToObject[0].x;
			v_8.y = unity_WorldToObject[1].x;
			v_8.z = unity_WorldToObject[2].x;
			v_8.w = unity_WorldToObject[3].x;
			float4 v_9;
			v_9.x = unity_WorldToObject[0].y;
			v_9.y = unity_WorldToObject[1].y;
			v_9.z = unity_WorldToObject[2].y;
			v_9.w = unity_WorldToObject[3].y;
			float4 v_10;
			v_10.x = unity_WorldToObject[0].z;
			v_10.y = unity_WorldToObject[1].z;
			v_10.z = unity_WorldToObject[2].z;
			v_10.w = unity_WorldToObject[3].z;
			float3 tmpvar_11;
			tmpvar_11 = normalize(((
			  (v_8.xyz * v.normal.x)
			 + 
			  (v_9.xyz * v.normal.y)
			) + (v_10.xyz * v.normal.z)));
			worldNormal_1 = tmpvar_11;
			tmpvar_4 = worldNormal_1;
			tmpvar_5.xy = ((v.texcoordMulti1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
			tmpvar_6 = mul(unity_WorldToShadow[0], tmpvar_7);
			float tmpvar_12;
			tmpvar_12 = (unity_FogParams.x * tmpvar_2.z);
			o.position = tmpvar_2;
			o.texcoord0 = tmpvar_3;
			o.texcoord1 = tmpvar_4;
			o.texcoord2 = tmpvar_5;
			o.texcoord3 = tmpvar_6;
			o.texcoord4 = exp2((-(tmpvar_12) * tmpvar_12));
			return o;
		}

		float alpha;

		float4 frag(v2f i) : SV_TARGET
		{
			float3 lm_1;
			float4 c_2;
			float2 tmpvar_3;
			float2 tmpvar_4;
			tmpvar_3 = i.texcoord0.xy;
			tmpvar_4 = i.texcoord0.zw;
			float4 tmpvar_5;
			tmpvar_5 = tex2D (_GlossTex, tmpvar_4);
			float x_6;
			x_6 = (tmpvar_5.x - alpha);
			if ((x_6 < 0.0)) {
			  discard;
			};
			//float shadow_7;
			//shadow_7 = (_LightShadowData.x + (UNITY_SAMPLE_DEPTH(tex2D (_ShadowMapTexture, i.texcoord3.xyz)) * (1.0 - _LightShadowData.x)));
			float4 tmpvar_8;
			tmpvar_8 = UNITY_SAMPLE_TEX2D (unity_Lightmap, i.texcoord2.xy);
			float3 tmpvar_9;
			tmpvar_9 = (2.0 * tmpvar_8.xyz);
			lm_1 = tmpvar_9;
			c_2.w = tmpvar_5.x;
			c_2.xyz = (((
			  (tex2D (_MainTex, tmpvar_3) + 0.02)
			 * 25.0).xyz * tmpvar_5.xyz));
			float tmpvar_10;
			tmpvar_10 = clamp (i.texcoord4, 0.0, 1.0);
			c_2.xyz = lerp(unity_FogColor.xyz, c_2.xyz, float3((tmpvar_10).xxx));
			return c_2;
		}
		ENDCG
	}
}
}