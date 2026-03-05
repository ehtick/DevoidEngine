struct PSInput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL;
    float4 Tangent : TANGENT; // xyz + handedness
    float2 UV0 : TEXCOORD0;
    float2 UV1 : TEXCOORD1;
    float3 WorldspacePosition : TEXCOORD3;
};

#include "render_constants.hlsl"
#include "light_constructs.hlsl"

float4 PSMain() : SV_TARGET
{
    return float4(1, 0, 0, 1);
}