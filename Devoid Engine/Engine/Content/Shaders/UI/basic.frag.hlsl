struct PSInput
{
    float4 Position : SV_POSITION;
    float2 NDC : TEXCOORD4;
    float2 UV0 : TEXCOORD0;
};

#include "../Common/render_constants.hlsl"

cbuffer MATERIAL
{
    float4 COLOR;
};

float4 PSMain(PSInput input) : SV_TARGET
{   
    return float4(COLOR);
}
