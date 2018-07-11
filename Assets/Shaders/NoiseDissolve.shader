Shader "Custom/NoiseDissolve"
{
	Properties
	{
		_MainTex ("Noise", 2D) = "white" {}
		_GradientTex ("Gradients", 2D) = "" {}
	}
	SubShader
	{
		Cull Off
		// ZTest Off ZWrite Off
		// Blend SrcAlpha OneMinusSrcAlpha
		// Blend One One

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Common.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _GradientTex;
			float4 _Tint;

			struct v2f
			{
				TRI_COMMON
				float2 pos : TEXCOORD2; 
			};

			v2f vert(appdata v) {
				v2f o;
				TRI_INITIALIZE(o);
				// o.pos = o.vertex * 0.5 + 0.5;
				float2 s = _Resolution.zw * _TexelSize.zw;
				// o.pos = (v.uv2 + (_Resolution.xy + 0.5) / _Resolution.zw) * s;
				float fx = _Resolution.y % 2 == 0;
				o.pos = (v.uv2 + (_Resolution.xy - float2(fx, 0)) / _Resolution.zw) * s;
				o.pos += _Group.xy / _Group.zw;
				o.pos = o.pos * _MainTex_ST.xy + _MainTex_ST.zw;
				// o.pos = v.uv2;
				// o.pos += _Resolution.xy * _TexelSize.xy;
				// o.pos += _Resolution.xy;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.pos;
				
				uv.x += _Time.y * 0.1;
				// return float4(uv, 0, 1);
				float r = tex2D(_MainTex, uv + _Time.xx * 0.0015).r;
				float z = tex2D(_MainTex, r.xx + _Time.xy * 0.125);
				z = pow(z + 0.25, 8);
				float4 col = tex2D(_GradientTex, float2(z, _Time.x * 0.5));
				float z2 = tex2D(_MainTex, z);
				float4 col2 = tex2D(_GradientTex, float2(z, _Time.x * 1 - 0.5));
				col = lerp(col, col2, r);
				return (col * _Tint).grba;
			}
			ENDCG
		}
	}
}
