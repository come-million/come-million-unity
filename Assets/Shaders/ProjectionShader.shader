Shader "Custom/ProjectionShader"
{
	Properties
	{
		// _Offset ("Offset", Vector) = (0, 0, 0, 0)
		_ProjectionTex ("Projection Texture", 2D) = "" {}
	}
	SubShader
	{
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Common.cginc"
			
			float4 _Offset;
			float4 _Tint;
			float _Alpha;

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
				// return float4(i.uv, 0, 1);
				// return i.pos;
				
				// if(i.pos.z > 0)
					// discard;

				if(i.pos.x < 0 || i.pos.x > 1)
					discard;
				if(i.pos.y < 0 || i.pos.y > 1)
					discard;

				float4 col = tex2D(_ProjectionTex, i.pos.xy) * _Tint;
				//col.a *= _Alpha;
				return col.rgba;
			}
			ENDCG
		}
	}
}
