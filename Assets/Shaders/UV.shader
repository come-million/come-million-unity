Shader "Custom/UV"
{
	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {}
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
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;

			struct v2f
			{
				TRI_COMMON
				// float2 pos : TEXCOORD2; 
			};

			v2f vert(appdata v) {
				v2f o;
				TRI_INITIALIZE(o);
				// o.uv *= _Resolution.zw * _MainTex_TexelSize.xy;
				// o.uv += _Resolution.xy * _MainTex_TexelSize.xy;
				// o.uv += _Resolution.xy / _Resolution.zw;
				// o.uv = _Resolution.xy * _TexelSize.zw;
                // o.uv = o.uv * _Resolution.zw;

                // o.uv = v.uv2;
				float2 s = _Resolution.zw * _TexelSize.zw;
				float fx = _Resolution.y % 2 == 0;
				o.uv = (v.uv2 + (_Resolution.xy - float2(fx, 0.5)) / _Resolution.zw) * s;
				o.uv += _Group.xy / _Group.zw;
				
				// o.uv = (o.uv + _Resolution.xy) / _Resolution.zw;
				// o.uv *= _Resolution.zw * _MainTex_TexelSize.xy;
				// o.uv += _Resolution.xy * _MainTex_TexelSize.xy;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
                return float4(i.uv, 0, 1);
			}
			ENDCG
		}
	}
}
