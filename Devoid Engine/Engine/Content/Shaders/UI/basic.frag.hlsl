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


float4 PSMain(PSInput input) : SV_TARGET
{
    float2 uv = input.UV0;

    // Border thickness in UV space (e.g., 5% of the quad)
    float border = 0.05;

    // Check if we're inside the border region
    bool isBorder =
        uv.x < border || uv.x > (1.0 - border) ||
        uv.y < border || uv.y > (1.0 - border);

    // Black border, white fill
    return isBorder
        ? float4(0.0, 0.0, 0.0, 1.0) // black
        : float4(1.0, 1.0, 1.0, 1.0); // white
}