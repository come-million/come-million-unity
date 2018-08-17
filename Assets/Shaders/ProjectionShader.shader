Shader "Custom/ProjectionShader"
{
	Properties
	{
		// _Offset ("Offset", Vector) = (0, 0, 0, 0)
		_MainTex ("Projection Texture", 2D) = "white" {}
		[NoScaleOffset]_UVMap ("UV", 2D) = "white" {}
		[NoScaleOffset]_PosMap ("Position", 2D) = "white" {}
		_Alpha ("Alpha", Float) = 1

		[Header(Color Correction)]
		_Gamma ("Gamma", Float) = 1
		_Brightness ("Brightness", Float) = 0
		_Contrast ("Contrast", Float) = 1
		_Saturation ("Saturation", Float) = 1
		_Hue ("Hue", Float) = 0	
	}
	SubShader
	{
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Common.cginc"
			// #pragma vertex vert
			#pragma vertex vert_img
			#pragma fragment frag
		
			float4 _Offset;
			float4 _Tint;
			float _Alpha;

			sampler2D _MainTex;
			float4x4 _ModelMatrix;
			float4x4 _ViewMatrix;
			float4x4 _ProjectionMatrix;
			bool _IsOrtho;

			sampler2D _UVMap;
			sampler2D _PosMap;

			float _Gamma;
			float _Brightness;
			float _Contrast;
			float _Saturation;
			float _Hue;


			struct v2f
			{
				float4 pos : SV_Target;
				float2 uv : TEXCOORD0;
				float4 pos2 : TEXCOORD1;
			};

			// v2f vert (appdata_img v)
			// {
			// 	v2f o;
 			// 	o.pos = UnityObjectToClipPos (v.vertex);

			// 	// o.pos2 = tex2Dlod(_PosMap, float4(v.texcoord.xy, 0, 0));
			// 	o.pos2 = float4(1,0,0,1);
			// 	// o.pos2 = float4(v.texcoord, 0, 1);
			// 	o.uv = v.texcoord;

            //     // // o.pos = v.vertex;
            //     // o.pos2 = mul(_ModelMatrix, o.pos2);
            //     // o.pos2 = mul(_ViewMatrix, o.pos2);
            //     // o.pos2 = mul(_ProjectionMatrix, o.pos2);
	
			// 	// o.pos2.w = -o.pos2.w;
			// 	// if(_IsOrtho)
			// 	// 	o.pos2.xy = -o.pos2.xy;
			// 	// o.pos2 /= o.pos2.w;
			// 	// o.pos2 = o.pos2 * 0.5 + 0.5;
			// 	return o;
			// }
		

			// fixed4 frag (v2f i) : SV_Target
			fixed4 frag (v2f_img i) : SV_Target
			{
				// return float4(1,0,1,1);
				// return float4(i.uv, 0, 1);

				float2 uv = tex2D(_UVMap, i.uv).xy;
				// return float4(uv, 0, 1);

				float4 pos = tex2D(_PosMap, i.uv);
				// return pos;

				// pos = mul(_ModelMatrix, pos);
                pos = mul(_ViewMatrix, pos);
                pos = mul(_ProjectionMatrix, pos);

				pos.w = -pos.w;
				if(_IsOrtho)
					pos.xy = -pos.xy;
				pos /= pos.w;
				pos = pos * 0.5 + 0.5;
				// return pos;

				// return float4(i.pos2.xyz, 1);
				// // return float4(i.uv, 0, 1);
				// // return i.pos;
				
				// // if(i.pos.z > 0)
				// 	// discard;

				if(pos.x < 0 || pos.x > 1)
					discard;
				if(pos.y < 0 || pos.y > 1)
					discard;

				float4 col = tex2D(_MainTex, pos.xy) * _Tint;
				col.a *= _Alpha;

				col.rgb = pow(col.rgb, _Gamma);
				col.rgb = (col.rgb - 0.5) * _Contrast + 0.5 + _Brightness;
				col.rgb = lerp(dot(col.rgb, float3(0.2126, 0.7152, 0.0722)), col.rgb, _Saturation);
				col.rgb = RGBtoHSV(col.rgb);
				col.r = fmod(col.r + _Hue, 1);
				col.rgb = HSVtoRGB(col.rgb);


				return col.rgba;
			}
			ENDCG
		}
	}
}
