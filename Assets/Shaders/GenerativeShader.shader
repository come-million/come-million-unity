Shader "Custom/Generative1"
{
	Properties
	{
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

			float4 _Tint;

			struct v2f
			{
				TRI_COMMON
				float2 pos : TEXCOORD2; 
				float2 pos2 : TEXCOORD4; 
			};

			v2f vert(appdata v) {
				v2f o;
				TRI_INITIALIZE(o);
				o.pos = v.uv2;
				// o.pos2 = o.vertex * 0.5 + 0.5;
				float2 s = _Resolution.zw * _TexelSize.zw;
				// o.pos2 = (v.uv2 + _Resolution.xy / _Resolution.zw) * s;
				// o.pos2 = (v.uv2 + (_Resolution.xy + 0.5) / _Resolution.zw) * s;
				float fx = _Resolution.y % 2 == 0;
				o.pos2 = (v.uv2 + (_Resolution.xy - float2(fx, 0.5)) / _Resolution.zw) * s;
				// o.pos2 = (v.uv2 + (_Resolution.xy - fx) / _Resolution.zw) * s;
				
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.pos;
				float2 uv2 = i.pos2;

				// return float4(uv2.xy, 0, 1);
				// return float4(uv, 0, 1);
				// return i.bc;
				// return float4(i.uv, 0, 1);

				// uint2 c = uint2(uv.xy * _TexelSize.xy);
				// float d = c.x == c.y;
				// return float4(d.xxx, 1);
				// return float4(_Resolution.xy * _TexelSize.zw, 0, 1);

				float4 col = 0;
				float fx = uint(_Resolution.y) % 2 == 0;
				col.r = uint((uv.x + 2*_Time.y/_Resolution.z*2 - fx/_Resolution.z*1) * _Resolution.z) % 5 == 0;
				// col.r = uint((uv.x + _Time.y/_Resolution.z*2) * _Resolution.z) % 5 == 0;

				col.g = uint((uv.y + _Time.y/_Resolution.w*2) * _Resolution.w + 1) % 5 == 0;
				col.g *= tri(_Time.y , 0.5);

				// bool b1 = uint((uv.x + _Time.y/_Resolution.z) * _Resolution.z) % 2 == 0;
				// bool b2 = uint((uv.y) * _Resolution.w + 1) % 2 == 0;
				// col.b = (b1 ^ b2);
				// col.b *= tri(_Time.y, 1);
				col.b = saw( tri(uv2.x - 0.1, 0.2) * 0.4 + uv2.y - _Time.y*0.5, 0.5);
				// col = lerp(col, col * 0.6 + 0.4, tri(_Time.y, 2));

				col.r *= step(square(_Time.y - 0, 1.0/12), 0) * (1 - step(square(_Time.y - 4, 1.0/12), 0));
				col.g *= step(square(_Time.y - 4, 1.0/12), 0) * (1 - step(square(_Time.y - 8, 1.0/12), 0));
				col.b *= step(square(_Time.y - 8, 1.0/12), 0) * (1 - step(square(_Time.y - 12, 1.0/12), 0));

				// col.a = 1;
				// col.a = any(col.rgb > 0);

				return col.rgba;


			}
			ENDCG
		}
	}
}
