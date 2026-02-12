struct VSInput
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float2 UV0 : TEXCOORD0;
    float2 UV1 : TEXCOORD1;
    float3 Tangent : TANGENT;
    float3 BiTangent : BINORMAL;
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float2 NDC : TEXCOORD4;
    float2 UV0 : TEXCOORD0;
};


cbuffer CameraData : register(b0)
{
    float4x4 View;
    float4x4 Projection;
    float4x4 InverseProjection;
    float3 Position;
    float NearClip;
    float FarClip;
    float2 ScreenSize;
    float _padding0;
};

cbuffer RenderData : register(b1)
{
    float4x4 Model;
    float4 id;
}
 

PSInput VSMain(VSInput input)
{
    PSInput output;

    float4 local = float4(input.Position.xy, 0.0, 1.0);
    float4 world = mul(Model, local);
    float4 clip = mul(Projection, world);

    output.Position = clip;
    output.NDC = clip.xy / clip.w; // ✅ VALID HERE
    output.UV0 = input.UV0;

    return output;
}

