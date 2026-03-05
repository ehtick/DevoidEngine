struct VSInput
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float2 UV0 : TEXCOORD0;
    float2 UV1 : TEXCOORD1;
    float3 Tangent : TANGENT;
    float3 BiTangent : BINORMAL; // not needed, but kept if mesh provides it
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL;
    float4 Tangent : TANGENT; // xyz + handedness
    float2 UV0 : TEXCOORD0;
    float2 UV1 : TEXCOORD1;
    float3 WorldspacePosition : TEXCOORD3;
};

const float outlineThickness = 0.2;

#include "render_constants.hlsl"

PSInput VSMain(VSInput input)
{
    PSInput output;

    float3 pos = input.Position + normalize(input.Normal) * outlineThickness;

    float4 world = mul(Model, float4(pos, 1));
    float4 view = mul(View, world);
    output.Position = mul(Projection, view);

    return output;
}