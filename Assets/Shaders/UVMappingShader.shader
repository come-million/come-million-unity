Shader "Custom/UVMappingShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;				
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4x4 _Matrix;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				// o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float4 pos = mul(_Matrix, v.vertex);
				// float4 pos = v.vertex;
				// pos = mul(unity_CameraProjection, pos);
				// pos = mul(UNITY_MATRIX_VP, pos);
				pos /= pos.w;
				// pos -= 0.5;
				// o.uv = pos.xy * 0.5 + 0.5;
				o.uv = pos.xy;
				// o.uv.y += 1;
				// o.uv.y = 1 - o.uv.y;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// return float4(i.uv, 0, 1);
				if(i.uv.x < 0 || i.uv.y < 0 || i.uv.x > 1 || i.uv.y > 1)
					return 0;
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
