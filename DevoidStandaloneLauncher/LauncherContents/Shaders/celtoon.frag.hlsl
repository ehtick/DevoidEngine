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

float4 PSMain(PSInput input) : SV_TARGET
{
    float3 N = normalize(input.Normal);
    float3 V = normalize(Position - input.WorldspacePosition);

    float3 albedo = float3(1.0, 1.0, 1.0);

    // Use your existing lighting system
    float3 lighting = ComputeLighting(
        input.WorldspacePosition,
        N,
        V,
        albedo
    );

    // Convert lighting intensity to grayscale
    float intensity = dot(lighting, float3(0.299, 0.587, 0.114));

    // Cartoon bands
    float shade;

    if (intensity > 0.75)
        shade = 1.0;
    else if (intensity > 0.45)
        shade = 0.7;
    else if (intensity > 0.2)
        shade = 0.35;
    else
        shade = 0.0;

    float3 color = float3(shade, shade, shade);
    //return float4(lighting, 1.0);
    return float4(color, 1.0);
}