Shader "Custom/AudioShader1"
{
	Properties
	{
	}
	SubShader
	{
		Cull Off
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Common.cginc"
			#pragma vertex vert
			#pragma fragment frag

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
				float fx = _Resolution.y % 2 == 0;
				o.pos = (v.uv2 + (_Resolution.xy - float2(fx, 0)) / _Resolution.zw) * s;
				// o.pos = (v.uv2 + (_Resolution.xy - fx) / _Resolution.zw) * s;
				// o.pos = (v.uv2 + (_Resolution.xy) / _Resolution.zw) * s;
				// o.pos = (v.uv2 + (_Resolution.xy + 0.5) / _Resolution.zw) * s;
				o.pos += _Group.xy / _Group.zw;
				return o;
			}

			uniform float _FFT[64];

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.pos;
				// return float4(uv, 0, 1);
				// uint f = uint(uv.x * 64);
				uint f = uint(lerp(0.01, 0.15, uv.x) * 64);
				float a = _FFT[f];
				float v = step(uv.y, a * 3);
				
				float3 col = lerp(float3(0, 1, 0), float3(1, 0, 0), saturate(2 * uv.y / exp(a)));
				return float4(v * col, 1);
			}
			ENDCG
		}
	}
}
