#ifndef COMMON_INCLUDED
#define COMMON_INCLUDED

float4 _Resolution;
float4 _TexelSize;
float4 _Group;

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float2 uv2 : TEXCOORD1;
    float4 bc : COLOR;
    float4 normal : NORMAL;
};

struct v2g
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    // float2 pos : TEXCOORD1;
    float4 bc : COLOR;
};

struct g2f
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    // float2 pos : TEXCOORD1;
    float4 bc : COLOR;
};

#define TRI_COMMON \
    float4 vertex : POSITION; \
    float2 uv : TEXCOORD0; \
    float4 bc : COLOR;

[maxvertexcount(4)]
void tri_geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
{
    g2f o;

    o.vertex = IN[0].vertex;
    o.uv = IN[0].uv;
    o.bc = IN[0].bc;
    triStream.Append(o);
    
    o.vertex = IN[1].vertex;
    o.uv = IN[1].uv;
    o.bc = IN[1].bc;
    triStream.Append(o);

    o.vertex = IN[2].vertex;
    o.uv = IN[2].uv;
    o.bc = IN[2].bc;
    triStream.Append(o);

    o.vertex.x = IN[2].vertex.x;
    o.vertex.y = IN[1].vertex.y;
    o.uv = IN[2].uv;
    o.bc = IN[2].bc;
    triStream.Append(o);
}

float4 tri_pos(appdata v) {
    float2 s = _Resolution.zw * _TexelSize.zw;
    float2 pos = (v.uv + _Resolution.xy / _Resolution.zw) * s * 2 - 1;
    pos += -2*(v.bc.xy-1) * _TexelSize.zw;
    pos.y = -pos.y;
    return float4(pos, 0, 1);
}

#define TRI_INITIALIZE(o) { \
    o.vertex = tri_pos(v); \
    o.uv = v.uv; \
    o.bc = v.bc; \
    }


inline float square(float t, float f) {
    return 2 * (2 * floor(f*t) - floor(2*f*t)) + 1;
}

inline float saw(float t, float a) {
    return 2 * (t/a - floor(t/a + 0.5));
}

inline float tri(float t, float a) {
    return abs(saw(t, a));
}

#endif
