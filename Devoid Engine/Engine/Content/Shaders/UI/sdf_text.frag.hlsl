struct PSInput
{
    float4 Position : SV_POSITION;
    float2 NDC : TEXCOORD4;
    float2 UV0 : TEXCOORD0;
};

struct GlyphMetrics
{
    uint charCode;
    int AtlasIndex;
    int Width;
    int Height;
    float HorizontalAdvance;
    float BearingX;
    float BearingY;
    float U;
    float V;
};

StructuredBuffer<GlyphMetrics> GlyphMetricsInfo;

cbuffer TransformData : register(b1)
{
    float4x4 Model;
    float4 id;
}

Texture2D fontSDFAtlas : register(t0);
SamplerState fontSDFAtlasSampler : register(s0);

float4 PSMain(PSInput input) : SV_TARGET
{
    return float4(10, 10, 10, 255);

}
