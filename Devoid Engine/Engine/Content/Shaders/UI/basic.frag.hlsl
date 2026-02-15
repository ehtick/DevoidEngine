struct PSInput
{
    float4 Position : SV_POSITION;
    float2 NDC : TEXCOORD4;
    float2 UV0 : TEXCOORD0;
};

cbuffer MeshData : register(b1)
{
    float4x4 Model;
    float4x4 invModel;
};

float4 PSMain(PSInput input) : SV_TARGET
{   
    //float3 color = float3(
    //    (id.x * 16807u % 255) / 255.0,
    //    (id.x * 48271u % 255) / 255.0,
    //    (id.x * 69621u % 255) / 255.0
    //);

    return float4(1.0, 0.5, 1.0, 1.0);
}
