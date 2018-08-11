Shader "Unlit/NormalDisplacement" {
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("Noise", 2D) = "white" {}
		_Color ("Tint", Color) = (1, 1, 1, 1)
		_Amount ("Amount", Float) = 1
		_Speed ("Speed", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float4 _Color;
			float _Amount;
			float _Speed;

			v2f vert (appdata v)
			{
				v2f o;
                float4 uv = float4(v.uv + _Time.x * _Speed, 0, 0);
                uv.xy = TRANSFORM_TEX(uv, _NoiseTex);
                float f =  cos(tex2Dlod(_NoiseTex, uv));
                v.vertex.xyz += v.normal * (f - 1.0) * _Amount;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col * i.color * _Color;
			}
			ENDCG
		}
	}
}
