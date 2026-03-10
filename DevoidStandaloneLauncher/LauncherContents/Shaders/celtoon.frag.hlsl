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


float3 SepiaColor(float shade)
{
    float3 dark = float3(0.08, 0.05, 0.02); // ink shadow
    float3 mid = float3(0.35, 0.25, 0.12); // paper shadow
    float3 light = float3(0.82, 0.70, 0.45); // paper light

    if (shade > 0.8)
        return light;
    else if (shade > 0.5)
        return mid;
    else
        return dark;
}

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
    
    float intensity = dot(lighting, float3(0.299, 0.587, 0.114));

    float shade;

    if (intensity > 0.8)
        shade = 1.0;
    else if (intensity > 0.5)
        shade = 0.6;
    else
        shade = 0.2;

    float3 color = SepiaColor(shade);

    float rim = 1.0 - saturate(dot(N, V));
    rim = smoothstep(0.5, 1.0, rim);
    color *= lerp(1.0, 0.4, rim);

    return float4(color, 1.0);
}