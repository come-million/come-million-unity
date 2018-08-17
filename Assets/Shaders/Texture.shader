// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Texture"
{
	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {}
		[NoScaleOffset]_UVMap ("UV", 2D) = "white" {}
		_Alpha ("Alpha", Float) = 1
	}
	SubShader
	{
		Cull Off
		// ZTest Off ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		// Blend One One

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Common.cginc"
			#pragma vertex vert_img
			// #pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _UVMap;
			float4 _Tint;
			float4 _Offset;
			float _Alpha;

			// struct v2f
			// {
			// 	TRI_COMMON
			// 	float2 pos : TEXCOORD2; 
			// };

			// v2f vert(appdata v) {
			// 	v2f o;
			// 	TRI_INITIALIZE(o);
			// 	// o.pos = o.vertex * 0.5 + 0.5;
			// 	float2 s = _Resolution.zw * _TexelSize.zw;
			// 	float fx = _Resolution.y % 2 == 0;
			// 	o.pos = (v.uv2 + (_Resolution.xy - float2(fx, 0)) / _Resolution.zw) * s;
			// 	// o.pos = (v.uv2 + (_Resolution.xy - fx) / _Resolution.zw) * s;
			// 	// o.pos = (v.uv2 + (_Resolution.xy) / _Resolution.zw) * s;
			// 	// o.pos = (v.uv2 + (_Resolution.xy + 0.5) / _Resolution.zw) * s;
			// 	o.pos += _Group.xy / _Group.zw;
			// 	o.pos = o.pos * _MainTex_ST.xy + _MainTex_ST.zw;
			// 	return o;
			// }

			// v2f_img vert( appdata_img v )
			// {
			// 	v2f_img o;
			// 	o.pos = UnityObjectToClipPos (v.vertex);
			// 	// o.uv = TRANSFORM_TEX(_MainTex, v.texcoord);
			// 	o.uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			// 	return o;
			// }


			fixed4 _MainTex_Offset;

			// fixed4 frag (v2f i) : SV_Target
			fixed4 frag (v2f_img i) : SV_Target
			{
				float2 uv = tex2D(_UVMap, i.uv).xy;
				uv = uv * _MainTex_Offset.xy + _MainTex_Offset.zw;
				// uv = uv * _MainTex_ST.xy + _MainTex_ST.zw;
				// return float4(uv, 0, 1);
				float4 col = tex2D(_MainTex, uv + _Offset.xy);
				col.a *= _Alpha;
				return (col * _Tint).rgba;
			}
			ENDCG
		}
	}
}
