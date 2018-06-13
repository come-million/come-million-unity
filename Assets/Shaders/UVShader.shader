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
				o.color = v.color;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			fixed4 frag (v2f i) : SV_Target
			{
				// return float4(i.uv, 0, 1);
				float4 col = tex2D(_MainTex, i.uv);
				col *= 2 - pow(length(i.color), 2);
				return col;
			}
			ENDCG
		}
	}
}
