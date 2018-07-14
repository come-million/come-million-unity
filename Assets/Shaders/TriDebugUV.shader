Shader "Unlit/DebugTriangle"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		// _Resolution ("Resolution", Vector) = (10, 5, 10, 5)
		// _Highlight ("Highlight", Range(0, 50)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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
				float2 pos2 : TEXCOORD3;
			};

			v2f vert(appdata v) {
				v2f o;
				TRI_INITIALIZE(o);
				o.pos = v.uv2;
				// o.pos2 = o.vertex * 0.5 + 0.5;
				// o.pos2 = v.uv2 / _Resolution.zw;
				o.pos2 = _Resolution.xy * _TexelSize.zw;
				return o;
			}

			// v2f vert (appdata v)
			// {
			// 	v2f o;
			// 	o.vertex = UnityObjectToClipPos(v.vertex);
			// 	o.uv = v.uv;
			// 	o.uv1 = v.uv1;
			// 	o.color = v.color;
			// 	return o;
			// }
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			// float4 _Resolution;
			int _Highlight;

			fixed4 frag (v2f i) : SV_Target
			{
				int2 uvi = i.uv.xy * _Resolution.zw;
				int idx = uvi.x + uvi.y * _Resolution.z;
				if(idx == _Highlight)
					return float4(1,0,1,1) * _Tint;
				// return i.color;
				// return float4(i.uv1, 0, 1);

				// py = 0;
				// col.b = uint((i.pos2.x + 0.5) * _TexelSize.x + py) % 10 == 0;
				// float x = saw(i.pos2.x * 0.5 - _Time.y, 1);
				// float x = saw(i.pos2.x * 0.5, 1);
				float x = square(i.pos2.x * 0.5 - _Time.y*0.05 + 0.5 - 1*_TexelSize.z, 1);
				float x0 = square(i.pos2.x * 0.5 - _Time.y*0.05 + 0.5 - 1*_TexelSize.z + 5*_TexelSize.z, 1);
				x -= x0;
				// float y = 1 - saw(i.pos2.y * 0.5 - _Time.y, 0.5);
				// float y = 1 - saw(i.pos2.y * 0.5 + i.pos.y * _TexelSize.w - _Time.y, 1);
				float y = 0.5 - tri(i.pos2.y * 0.5 + i.pos.y * _TexelSize.w - _Time.y, 1);
				// return float4(x*y, y, 0, 1).grba;

				float4 col = float4(i.uv, 0, 1);
				col = lerp(col, float4(1, 0, 1, 1), saturate(x*y));
				return (col * _Tint).rgba;
			}
			ENDCG
		}
	}
}
