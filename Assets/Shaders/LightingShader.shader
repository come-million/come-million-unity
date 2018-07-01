Shader "Custom/LightingShader"
{
	Properties
	{
		// _Offset ("Offset", Vector) = (0, 0, 0, 0)
	}
	SubShader
	{
		Cull Off
		Blend One One
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

            float4x4 _Transform;

			fixed4 frag (v2f i) : SV_Target
			{
                // return i.pos;
				#define PI 3.1415926535897932384626433832795

				float3 p = transpose(_Transform)[3].xyz;
                // return float4(i.pos.xyz - p.xyz, 1);
				float r = distance(i.pos.xyz, p.xyz);
				r = 4 / r;
				r = smoothstep(0, 1, saturate(r));
				return float4(r * _Tint.rgb, 1);
			}
			ENDCG
		}
	}
}
