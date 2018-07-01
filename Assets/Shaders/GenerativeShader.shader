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

			struct v2f
			{
				TRI_COMMON
			};

			v2f vert(appdata v) {
				v2f o;
				TRI_INITIALIZE(o);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// return i.bc;
				// return float4(i.uv, 0, 1);

				// uint2 c = uint2(i.uv.xy * _Resolution.xy);
				// float d = c.x == c.y;
				// return float4(d.xxx, 1);

				float4 col = 0;
				col.r = uint((i.uv.x + _Time.y/_Resolution.z*2) * _Resolution.z) % 3 == 0;

				col.g = uint((i.uv.y + _Time.y/_Resolution.w*2) * _Resolution.w) % 3 == 0;
				col.g *= tri(_Time.y , 0.5);

				bool b1 = uint((i.uv.x + _Time.y/_Resolution.z) * _Resolution.z) % 2 == 0;
				bool b2 = uint((i.uv.y) * _Resolution.w + 1) % 2 == 0;
				col.b = (b1 ^ b2);
				col.b *= tri(_Time.y, 1);

				// col = lerp(col, col * 0.6 + 0.4, tri(_Time.y, 2));

				col.r *= step(square(_Time.y - 0, 1.0/12), 0) * (1 - step(square(_Time.y - 4, 1.0/12), 0));
				col.g *= step(square(_Time.y - 4, 1.0/12), 0) * (1 - step(square(_Time.y - 8, 1.0/12), 0));
				col.b *= step(square(_Time.y - 8, 1.0/12), 0) * (1 - step(square(_Time.y - 12, 1.0/12), 0));

				// col.a = 1;
				// col.a = any(col.rgb > 0);

				return col;


			}
			ENDCG
		}
	}
}
