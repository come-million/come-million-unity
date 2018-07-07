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
			};

			v2f vert(appdata v) {
				v2f o;
				TRI_INITIALIZE(o);
				o.pos = v.uv2;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.pos;
				// return float4(uv, 0, 1);
				// return i.bc;
				// return float4(i.uv, 0, 1);

				// uint2 c = uint2(uv.xy * _TexelSize.xy);
				// float d = c.x == c.y;
				// return float4(d.xxx, 1);
				// return float4(_Resolution.xy * _TexelSize.zw, 0, 1);

				float4 col = 0;
				float fx = int(_Resolution.y) % 2 == 0;
				col.r = uint((uv.x + 2*_Time.y/_Resolution.z*2 - fx/_Resolution.z*1) * _Resolution.z) % 5 == 0;
				// col.r = uint((uv.x + _Time.y/_Resolution.z*2) * _Resolution.z) % 5 == 0;

				col.g = uint((uv.y + _Time.y/_Resolution.w*2) * _Resolution.w + 1) % 5 == 0;
				col.g *= tri(_Time.y , 0.5);

				bool b1 = uint((uv.x + _Time.y/_Resolution.z) * _Resolution.z) % 2 == 0;
				bool b2 = uint((uv.y) * _Resolution.w + 1) % 2 == 0;
				col.b = (b1 ^ b2);
				col.b *= tri(_Time.y, 1);
				// col = lerp(col, col * 0.6 + 0.4, tri(_Time.y, 2));

				col.r *= step(square(_Time.y - 0, 1.0/12), 0) * (1 - step(square(_Time.y - 4, 1.0/12), 0));
				col.g *= step(square(_Time.y - 4, 1.0/12), 0) * (1 - step(square(_Time.y - 8, 1.0/12), 0));
				col.b *= step(square(_Time.y - 8, 1.0/12), 0) * (1 - step(square(_Time.y - 12, 1.0/12), 0));

				// col.a = 1;
				// col.a = any(col.rgb > 0);

				return col.grba;


			}
			ENDCG
		}
	}
}
