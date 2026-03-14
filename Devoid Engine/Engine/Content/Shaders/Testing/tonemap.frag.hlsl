struct PSInput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL;
    float4 Tangent : TANGENT; // xyz = tangent, w = handedness
    float3 BiTangent : BINORMAL;
    float2 UV0 : TEXCOORD0;
    float2 UV1 : TEXCOORD1;
    float3 FragPos : TEXCOORD2;
    float3 WorldPos : TEXCOORD3;
};

cbuffer Material : register(b2)
{
    float4 color;
};

Texture2D MAT_SceneColor : register(t0);
SamplerState MAT_SceneColorSampler : register(s0);

float3 AgxLog(float3 x)
{
    return log2(max(x, 1e-6));
}

float3 AgxExp(float3 x)
{
    return exp2(x);
}

float3 AgxContrast(float3 x)
{
    return x * x * (3.0 - 2.0 * x);
}

float3 AgxLookPunchy(float3 color)
{
    float luma = dot(color, float3(0.2126, 0.7152, 0.0722));
    return lerp(luma.xxx, color, 1.35);
}

float3 TonemapAgX(float3 color)
{
    color = log2(max(color, 1e-6));

    const float minEv = -12.47393;
    const float maxEv = 4.026069;

    color = (color - minEv) / (maxEv - minEv);
    color = saturate(color);

    color = color * color * (3.0 - 2.0 * color);

    return color;
}


float4 PSMain(PSInput input) : SV_TARGET
{
    float3 hdr = MAT_SceneColor.Sample(MAT_SceneColorSampler, input.UV0).rgb;

    float exposure = 0.1;
    hdr *= exposure;

    float3 color = TonemapAgX(hdr);
    //color = AgxLookPunchy(color);

    color = pow(color, 1.0 / 2.2);

    return float4(color, 1.0);
}