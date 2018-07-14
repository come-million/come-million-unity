Shader "Unlit/UVShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Resolution;
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv *= _Resolution.zw * _MainTex_TexelSize.xy;
				o.uv += _Resolution.xy * _MainTex_TexelSize.xy;
				#if SHADER_API_GLCORE
				o.uv.y = 1 - o.uv.y;
				o.uv.x += 0.5 * _MainTex_TexelSize.x;
				o.uv.y -= 0.5 * _MainTex_TexelSize.y;
				#else
				o.uv.x += 0.5 * _MainTex_TexelSize.x;
				o.uv.y += 0.5 * _MainTex_TexelSize.y;
				#endif
				o.color = v.color;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// return i.color;
				// return float4(i.uv, 0, 1);
				float4 col = tex2D(_MainTex, i.uv);
				float r = 2 - pow(length(i.color), 2);
				col = col * r + col * r;
				return col.rgba;
			}
			ENDCG
		}
	}
}
