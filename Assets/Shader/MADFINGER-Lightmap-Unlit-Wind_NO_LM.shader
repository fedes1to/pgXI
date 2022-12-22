Shader "MADFINGER/Environment/Lightmap + Wind_NO_LM" {
Properties {
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
 _Wind ("Wind params", Vector) = (1,1,1,1)
 _WindEdgeFlutter ("Wind edge fultter factor", Float) = 0.5
 _WindEdgeFlutterFreqScale ("Wind edge fultter freq scale", Float) = 0.5
}
SubShader { 
 LOD 100
 Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
 Pass {
  Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Transparent" "RenderType"="Transparent" }
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 _Wind;
uniform highp vec4 _MainTex_ST;
uniform highp float _WindEdgeFlutter;
uniform highp float _WindEdgeFlutterFreqScale;
varying highp vec2 xlv_TEXCOORD0;
varying lowp vec3 xlv_TEXCOORD2;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp float bendingFact_2;
  highp vec4 wind_3;
  lowp float tmpvar_4;
  tmpvar_4 = tmpvar_1.w;
  bendingFact_2 = tmpvar_4;
  highp mat3 tmpvar_5;
  tmpvar_5[0] = _World2Object[0].xyz;
  tmpvar_5[1] = _World2Object[1].xyz;
  tmpvar_5[2] = _World2Object[2].xyz;
  wind_3.xyz = (tmpvar_5 * _Wind.xyz);
  wind_3.w = (_Wind.w * bendingFact_2);
  highp vec2 tmpvar_6;
  tmpvar_6.y = 1.0;
  tmpvar_6.x = _WindEdgeFlutterFreqScale;
  highp vec4 pos_7;
  pos_7.w = _glesVertex.w;
  highp vec3 bend_8;
  highp vec4 v_9;
  v_9.x = _Object2World[0].w;
  v_9.y = _Object2World[1].w;
  v_9.z = _Object2World[2].w;
  v_9.w = _Object2World[3].w;
  highp float tmpvar_10;
  tmpvar_10 = dot (v_9.xyz, vec3(1.0, 1.0, 1.0));
  highp vec2 tmpvar_11;
  tmpvar_11.x = dot (_glesVertex.xyz, vec3((_WindEdgeFlutter + tmpvar_10)));
  tmpvar_11.y = tmpvar_10;
  highp vec4 tmpvar_12;
  tmpvar_12 = abs(((
    fract((((
      fract((((_Time.y * tmpvar_6).xx + tmpvar_11).xxyy * vec4(1.975, 0.793, 0.375, 0.193)))
     * 2.0) - 1.0) + 0.5))
   * 2.0) - 1.0));
  highp vec4 tmpvar_13;
  tmpvar_13 = ((tmpvar_12 * tmpvar_12) * (3.0 - (2.0 * tmpvar_12)));
  highp vec2 tmpvar_14;
  tmpvar_14 = (tmpvar_13.xz + tmpvar_13.yw);
  bend_8.xz = ((_WindEdgeFlutter * 0.1) * normalize(_glesNormal)).xz;
  bend_8.y = (bendingFact_2 * 0.3);
  pos_7.xyz = (_glesVertex.xyz + ((
    (tmpvar_14.xyx * bend_8)
   + 
    ((wind_3.xyz * tmpvar_14.y) * bendingFact_2)
  ) * wind_3.w));
  pos_7.xyz = (pos_7.xyz + (bendingFact_2 * wind_3.xyz));
  gl_Position = (glstate_matrix_mvp * pos_7);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_1.xyz;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  c_1.xyz = tmpvar_2.xyz;
  c_1.w = tmpvar_2.w;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 _Wind;
uniform highp vec4 _MainTex_ST;
uniform highp float _WindEdgeFlutter;
uniform highp float _WindEdgeFlutterFreqScale;
out highp vec2 xlv_TEXCOORD0;
out lowp vec3 xlv_TEXCOORD2;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp float bendingFact_2;
  highp vec4 wind_3;
  lowp float tmpvar_4;
  tmpvar_4 = tmpvar_1.w;
  bendingFact_2 = tmpvar_4;
  highp mat3 tmpvar_5;
  tmpvar_5[0] = _World2Object[0].xyz;
  tmpvar_5[1] = _World2Object[1].xyz;
  tmpvar_5[2] = _World2Object[2].xyz;
  wind_3.xyz = (tmpvar_5 * _Wind.xyz);
  wind_3.w = (_Wind.w * bendingFact_2);
  highp vec2 tmpvar_6;
  tmpvar_6.y = 1.0;
  tmpvar_6.x = _WindEdgeFlutterFreqScale;
  highp vec4 pos_7;
  pos_7.w = _glesVertex.w;
  highp vec3 bend_8;
  highp vec4 v_9;
  v_9.x = _Object2World[0].w;
  v_9.y = _Object2World[1].w;
  v_9.z = _Object2World[2].w;
  v_9.w = _Object2World[3].w;
  highp float tmpvar_10;
  tmpvar_10 = dot (v_9.xyz, vec3(1.0, 1.0, 1.0));
  highp vec2 tmpvar_11;
  tmpvar_11.x = dot (_glesVertex.xyz, vec3((_WindEdgeFlutter + tmpvar_10)));
  tmpvar_11.y = tmpvar_10;
  highp vec4 tmpvar_12;
  tmpvar_12 = abs(((
    fract((((
      fract((((_Time.y * tmpvar_6).xx + tmpvar_11).xxyy * vec4(1.975, 0.793, 0.375, 0.193)))
     * 2.0) - 1.0) + 0.5))
   * 2.0) - 1.0));
  highp vec4 tmpvar_13;
  tmpvar_13 = ((tmpvar_12 * tmpvar_12) * (3.0 - (2.0 * tmpvar_12)));
  highp vec2 tmpvar_14;
  tmpvar_14 = (tmpvar_13.xz + tmpvar_13.yw);
  bend_8.xz = ((_WindEdgeFlutter * 0.1) * normalize(_glesNormal)).xz;
  bend_8.y = (bendingFact_2 * 0.3);
  pos_7.xyz = (_glesVertex.xyz + ((
    (tmpvar_14.xyx * bend_8)
   + 
    ((wind_3.xyz * tmpvar_14.y) * bendingFact_2)
  ) * wind_3.w));
  pos_7.xyz = (pos_7.xyz + (bendingFact_2 * wind_3.xyz));
  gl_Position = (glstate_matrix_mvp * pos_7);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD2 = tmpvar_1.xyz;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
in highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture (_MainTex, xlv_TEXCOORD0);
  c_1.xyz = tmpvar_2.xyz;
  c_1.w = tmpvar_2.w;
  _glesFragData[0] = c_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
"!!GLES"
}
SubProgram "gles3 " {
"!!GLES3"
}
}
 }
}
}