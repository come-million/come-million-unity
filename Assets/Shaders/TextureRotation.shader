Shader "Custom/TextureRotation"
{
	Properties
	{
		_MainTex ("MainTex", 2D) = "white" {}
		_Speed ("Rotation Speed", Float) = 4
		_Alpha ("Alpha", Float) = 1
	}
	SubShader
	{
		Cull Off
		// ZTest Off ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		// Blend One One

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Common.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _GradientTex;
			float4 _Tint;
			float4 _Offset;
			float _Alpha;
			float _Speed;

			struct v2f
			{
				TRI_COMMON
				float2 pos : TEXCOORD2; 
			};

			float2 rotate(float2 v, float a) {
				float s = sin(a);
				float c = cos(a);
				float2x2 m = float2x2(c, -s, s, c);
				return mul(m, v);
			}

			v2f vert(appdata v) {
				v2f o;
				TRI_INITIALIZE(o);
				// o.pos = o.vertex * 0.5 + 0.5;
				float2 s = _Resolution.zw * _TexelSize.zw;
				float fx = _Resolution.y % 2 == 0;
				o.pos = (v.uv2 + (_Resolution.xy - float2(fx, 0)) / _Resolution.zw) * s;
				o.pos += _Group.xy / _Group.zw;
				o.pos = o.pos * _MainTex_ST.xy + _MainTex_ST.zw;
				o.pos = rotate(o.pos - 0.5, _Time.y*_Speed) + 0.5;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.pos;
				float4 col = tex2D(_MainTex, uv);
				col.a *= _Alpha;
				return (col * _Tint).rgba;
			}
			ENDCG
		}
	}
}
