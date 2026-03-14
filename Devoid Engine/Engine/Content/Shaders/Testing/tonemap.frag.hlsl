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

float3 TonemapAgX(float3 x)
{
    const float3x3 agx_inset = float3x3(
        0.842479062253094, 0.0423282422610123, 0.0423756549057051,
        0.0784335999999992, 0.878468636469772, 0.0784336,
        0.0792237451477643, 0.0791661274605434, 0.879142973793104
    );

    const float3x3 agx_outset = float3x3(
        1.19687900512017, -0.0528968517574562, -0.0529716355144438,
        -0.0980208811401368, 1.15190312990417, -0.0980434501171241,
        -0.0990297440797205, -0.0989611768448433, 1.15107367264116
    );

    x = mul(agx_inset, x);

    const float min_ev = -12.47393;
    const float max_ev = 4.026069;

    x = log2(max(x, 1e-6));
    x = (x - min_ev) / (max_ev - min_ev);
    x = saturate(x);

    x = x * x * (3.0 - 2.0 * x);

    x = mul(agx_outset, x);

    return x;
}

float3 TonemapSimple(float3 c)
{
    return c / (1.0 + c);
}


float4 PSMain(PSInput input) : SV_TARGET
{
    float3 hdr = MAT_SceneColor.Sample(MAT_SceneColorSampler, input.UV0).rgb;

    hdr *= 1.0;

    float3 color = TonemapAgX(hdr);

    color = pow(color, 1.0 / 2.2);

    return float4(color, 1);
}