struct PSInput
{
    float4 Position : SV_POSITION;
    float2 NDC : TEXCOORD4;
    float2 UV0 : TEXCOORD0;
};

cbuffer TransformData : register(b1)
{
    float4x4 Model;
    float4 id;
}

Texture2D uiTexture : register(t0);
SamplerState uiSampler : register(s0);

float4 PSMain(PSInput input) : SV_TARGET
{
    if (id.y == 1)
    {
        return uiTexture.Sample(uiSampler, input.UV0);
    }
    
    float3 color = float3(
        (id.x * 16807u % 255) / 255.0,
        (id.x * 48271u % 255) / 255.0,
        (id.x * 69621u % 255) / 255.0
    );

    return float4(color, 1.0);
}
