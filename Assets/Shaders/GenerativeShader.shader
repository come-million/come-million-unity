Shader "Custom/Generative1"
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
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float4 _Resolution;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 bc : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;

				float2 pos = v.uv * 2 - 1;
				// pos += v.color.xy;
				// pos += v.color.xy * 2;
				pos += -v.color.xy + 2.0/_Resolution.xy;
				o.vertex = float4(pos, 0, 1);
				o.uv = v.uv;
				o.bc = v.color;
				return o;
			}
			
			
			inline float square(float t, float f) {
				return 2 * (2 * floor(f*t) - floor(2*f*t)) + 1;
			}

			inline float saw(float t, float a) {
				return 2 * (t/a - floor(t/a + 0.5));
			}

			inline float tri(float t, float a) {
				return abs(saw(t, a));
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
				// if(i.bc.x > 2.0/_Resolution.x || i.bc.y > 2.0/_Resolution.y)
					// discard;
				clip(-i.bc.xy + 2.0/_Resolution.xy);
				// return float4(i.uv, 0.0, 1);

				// uint2 c = uint2(i.uv.xy * _Resolution.xy);
				// float d = c.x == c.y;
				// return float4(d.xxx, 1);

				float4 col = 0;
				col.r = uint((i.uv.x + _Time.y/_Resolution.x) * _Resolution.x) % 3 == 0;

				col.g = uint((i.uv.y + _Time.y/_Resolution.y*2) * _Resolution.y) % 3 == 0;
				col.g *= tri(_Time.y , 0.5);

				bool b1 = uint((i.uv.x + _Time.y/_Resolution.x) * _Resolution.x) % 2 == 0;
				bool b2 = uint((i.uv.y) * _Resolution.y + 1) % 2 == 0;
				col.b = (b1 ^ b2);
				col.b *= tri(_Time.y, 1);

				col = lerp(col, col * 0.6 + 0.4, tri(_Time.y, 2));

				col.r *= step(square(_Time.y - 0, 1.0/12), 0) * (1 - step(square(_Time.y - 4, 1.0/12), 0));
				col.g *= step(square(_Time.y - 4, 1.0/12), 0) * (1 - step(square(_Time.y - 8, 1.0/12), 0));
				col.b *= step(square(_Time.y - 8, 1.0/12), 0) * (1 - step(square(_Time.y - 12, 1.0/12), 0));

				col.a = 1;

				return col;


			}
			ENDCG
		}
	}
}
