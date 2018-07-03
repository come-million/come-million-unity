Shader "Custom/ProjectionShader"
{
	Properties
	{
		// _Offset ("Offset", Vector) = (0, 0, 0, 0)
		_ProjectionTex ("Projection Texture", 2D) = "" {}
	}
	SubShader
	{
		// Cull Off
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Common.cginc"

			
			
			float4 _Offset;
			float4 _Tint;

			sampler2D _ProjectionTex;
			float4x4 _ModelMatrix;
			float4x4 _ViewMatrix;
			float4x4 _ProjectionMatrix;
			bool _IsOrtho;

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
                o.pos = mul(_ViewMatrix, o.pos);
                o.pos = mul(_ProjectionMatrix, o.pos);
	
				o.pos.w = -o.pos.w;
				if(_IsOrtho)
					o.pos.xy = -o.pos.xy;
				o.pos /= o.pos.w;
				o.pos = o.pos * 0.5 + 0.5;
				return o;
			}
		

			fixed4 frag (v2f i) : SV_Target
			{



				if(i.pos.x < 0 || i.pos.x > 1)
					discard;
				if(i.pos.y < 0 || i.pos.y > 1)
					discard;


                // return i.pos;
				// return tex2D(_ProjectionTex, i.uv).grba;
				return (tex2D(_ProjectionTex, i.pos.xy) * _Tint ).grba;
				// return tex2Dproj(_ProjectionTex, i.pos).grba;
				// return tex2Dproj(_ProjectionTex, float4(i.uv.xy, 0, 1)).grba;
			}
			ENDCG
		}
	}
}
