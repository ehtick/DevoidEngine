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
    float3 Normal : NORMAL;
    float4 Tangent : TANGENT; // xyz = tangent, w = handedness
    float3 BiTangent : BINORMAL;
    float2 UV0 : TEXCOORD0;
    float2 UV1 : TEXCOORD1;
    float3 FragmentPosition : TEXCOORD2;
    float3 WorldspacePosition : TEXCOORD3;
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

cbuffer MeshData : register(b1)
{
    float4x4 Model;
    float4x4 invModel;
};

PSInput VSMain(VSInput input)
{
    PSInput output = (PSInput) 0; // unconditional initialization
    output.UV0 = input.UV0;
    output.UV1 = input.UV1;

    float4 worldPos = mul(Model, float4(input.Position, 1.0f));
    
    float4 viewPos = mul(View, worldPos);
    output.Position = mul(Projection, viewPos);
    
    return output;
}
