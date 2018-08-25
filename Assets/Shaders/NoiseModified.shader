Shader "Custom/NoiseModified"
{
	Properties
	{
		_MainTex ("Noise", 2D) = "white" {}
		_GradientTex ("Gradients", 2D) = "" {}
		[NoScaleOffset]_UVMap ("UV", 2D) = "white" {}
		_Alpha ("Alpha", Float) = 1


		[Header(Color Correction)]
		_Gamma ("Gamma", Float) = 1
		_Brightness ("Brightness", Float) = 0
		_Contrast ("Contrast", Float) = 1
		_Saturation ("Saturation", Float) = 1
		_Hue ("Hue", Float) = 0	
		_TimeValue1("_TimeValue1", Float) = 0.1
		_TimeValue2("_TimeValue2", Float) = 0.1
		_TimeValue3("_TimeValue3", Float) = 0.1
		_TimeValue4("_TimeValue4", Float) = 0.1
		_TimeValue5("_TimeValue5", Float) = 0.00025
			

	}
	SubShader
	{
		Cull Off
		// ZTest Off ZWrite Off
		//Blend SrcAlpha OneMinusSrcAlpha
		Blend SrcAlpha OneMinusSrcColor
		// Blend One One

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Common.cginc"
			#pragma vertex vert_img
			#pragma fragment frag
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _GradientTex;
			sampler2D _UVMap;

			fixed4 _MainTex_Offset;

			float4 _Tint;
			float _Alpha;

			float _Gamma;
			float _Brightness;
			float _Contrast;
			float _Saturation;
			float _Hue;
			float _TimeValue1;
			float _TimeValue2;
			float _TimeValue3;
			float _TimeValue4;
			float _TimeValue5;

			float _GlobalSkinBrightness;
			//float _MicLowGlobal;
			float _ReferenceSkinBrightness;

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
			// 	// o.pos = (v.uv2 + (_Resolution.xy + 0.5) / _Resolution.zw) * s;
			// 	float fx = _Resolution.y % 2 == 0;
			// 	o.pos = (v.uv2 + (_Resolution.xy - float2(fx, 0)) / _Resolution.zw) * s;
			// 	o.pos += _Group.xy / _Group.zw;
			// 	o.pos = o.pos * _MainTex_ST.xy + _MainTex_ST.zw;
			// 	// o.pos = v.uv2;
			// 	// o.pos += _Resolution.xy * _TexelSize.xy;
			// 	// o.pos += _Resolution.xy;
			// 	return o;
			// }

			fixed4 frag (v2f_img i) : SV_Target
			{
				// float2 uv = i.pos;
				float2 uv = tex2D(_UVMap, i.uv).xy;
				uv = uv * _MainTex_Offset.xy + _MainTex_Offset.zw;

				// return float4(uv, 0, 1);
				float r = tex2D(_MainTex, uv + _Time.x * _TimeValue1).r;
				r = 0.5f * (r + tex2D(_MainTex, uv + _Time.xy * _TimeValue2).r);
				//float z = tex2D(_MainTex, r*0.25 + _Time.xy * _TimeValue2);
				//float z = tex2D(_MainTex, uv * 2.0f + _Time.xy * _TimeValue2);
				//float z = tex2D(_MainTex, uv * 2.0f + _TimeValue2);
				float z = tex2D(_MainTex, r.xx + _Time.xy * _TimeValue5);
				// z = pow(z + 0.25, 8);
				float4 col = tex2D(_GradientTex, float2(z, _Time.x * _TimeValue3));
				// float z2 = tex2D(_MainTex, z);
				float anotherNumber = lerp(0.4, 0.6f, 0.5 + 0.45 * sin(_Time));
				float4 col2 = tex2D(_GradientTex, float2(z, 0.7));
				col = lerp(col, col2, r);
				//float brightnessWave = 0.95f + 0.05f * sin(0.2f * _Time.xx * 2.0f + _TimeValue4 * uv.y);
				//col *= brightnessWave;
				col.a *= _Alpha;

				col.rgb = pow(col.rgb, _Gamma);
				col.rgb = (col.rgb - 0.5) * _Contrast + 0.5 + _Brightness;
				//float saturationWave = 0.5f * _Saturation * sin(10.0f * _Time.xx * 3.14 * r);
				//float currentSaturation = fmod(_Saturation + saturationWave, 1);
				float currentSaturation = _Saturation;
				col.rgb = lerp(dot(col.rgb, float3(0.2126, 0.7152, 0.0722)), col.rgb, currentSaturation);
				col.rgb = RGBtoHSV(col.rgb);
				col.r = fmod(col.r + _Hue, 1);
				
				col.r = fmod(col.r + r, 1);
				float brightnessWave = 0.5f + 0.5f * sin(_Time.x * r * uv.x * _TimeValue4) * cos(_Time.x * r * uv.y * _TimeValue4);
				col.a *= saturate(brightnessWave * (_GlobalSkinBrightness/_ReferenceSkinBrightness));
				col.r = fmod(col.r + _Time * 0.03, 1);
				col.rgb = HSVtoRGB(col.rgb);

				//return (col * _Tint).rgba;
				return col.rgba;
			}
			ENDCG
		}
	}
}
