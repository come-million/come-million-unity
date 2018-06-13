// base on http://codeflow.org/entries/2012/aug/02/easy-wireframe-display-with-barycentric-coordinates/

Shader "Custom/TriangleWireframe"
{
	Properties
	{
		_Color ("Color", Color) = (0, 1, 0, 1)
		_Width ("Width", Range(0, 10)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
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
				float3 bary : COLOR;
			};

			struct v2f
			{
				float3 bary : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			float _Width;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.bary = v.bary;
				return o;
			}
			
			
			float edgeFactor(float3 vBC, float width){
				float3 bary = float3(vBC.x, vBC.y, 1.0 - vBC.x - vBC.y);
				float3 d = fwidth(bary);
				float3 a3 = smoothstep(d * (width - 0.5), d * (width + 0.5), bary);
				return min(min(a3.x, a3.y), a3.z);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float g = edgeFactor(i.bary, _Width);
				return float4(_Color.rgb, 1 - g);
			}
			ENDCG
		}
	}
}
