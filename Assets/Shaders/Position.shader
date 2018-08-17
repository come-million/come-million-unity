Shader "Custom/Position"
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

            float4x4 _ModelMatrix;

			struct v2f
			{
				TRI_COMMON
				float4 pos : TEXCOORD2; 
			};

			v2f vert(appdata v) {
				v2f o;
				TRI_INITIALIZE(o);
                // o.pos = o.vertex;
                o.pos = v.vertex;
                o.pos = mul(_ModelMatrix, o.pos);
                return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
                return i.pos;
			}
			ENDCG
		}
	}
}
