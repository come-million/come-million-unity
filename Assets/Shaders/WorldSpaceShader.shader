Shader "Custom/WorldSpaceShader"
{
	Properties
	{
		// _Offset ("Offset", Vector) = (0, 0, 0, 0)
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
			#include "Common.cginc"

			float4x4 _ModelMatrix;
			
			
			float4 _Offset;
			float4 _Tint;

			struct v2f
			{
				TRI_COMMON
				float4 pos : TEXCOORD2;
			};

			v2f vert (appdata v)
			{
				v2f o;

				TRI_INITIALIZE(o);

                o.pos = v.vertex;
                o.pos = mul(_ModelMatrix, o.pos);
				return o;
			}


			fixed4 frag (v2f i) : SV_Target
			{
                // return i.pos;
				#define PI 3.1415926535897932384626433832795
				return float4(cos(i.pos.xyz * 0.5 + _Offset.xyz) * _Tint.rgb, 1);
				// // return float4(cos(i.pos.x * 2 + _Offset.x), 0, 0, 1);
				// float4 col = 0;
                // col.r = tri(i.pos.x * 2 + _Time.y, 1);
                // col.a = 1;
                // return col;
			}
			ENDCG
		}
	}
}
