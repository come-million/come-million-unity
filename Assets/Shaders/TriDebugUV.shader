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
			};

			v2f vert(appdata v) {
				v2f o;
				TRI_INITIALIZE(o);
				o.pos = v.uv2;
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
					return float4(1,0,1,1);
				// return i.color;
				// return float4(i.uv1, 0, 1);
				return float4(i.uv, 0, 1).grba;
			}
			ENDCG
		}
	}
}
