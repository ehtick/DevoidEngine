struct PSInput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL;
    float4 Tangent : TANGENT;
    float3 BiTangent : BINORMAL;
    float2 UV0 : TEXCOORD0;
    float2 UV1 : TEXCOORD1;
    float3 FragPos : TEXCOORD2;
    float3 WorldPos : TEXCOORD3;
};

Texture2D MAT_SceneColor : register(t0);
Texture2D MAT_SceneDepth : register(t1);

SamplerState MAT_SceneColorSampler : register(s0);
SamplerState MAT_SceneDepthSampler : register(s1);

cbuffer Material : register(b3)
{
    float2 ScreenSize;
    float DepthMultiplier;
    float NormalMultiplier;
    float OutlineThreshold;
};

cbuffer CameraData : register(b0)
{
    float NearPlane;
    float FarPlane;
};


// ------------------------------------------------------------
// Linearize depth (required for stable edge detection)
// ------------------------------------------------------------

float LinearizeDepth(float depth)
{
    return NearPlane * FarPlane /
           (FarPlane - depth * (FarPlane - NearPlane));
}


// ------------------------------------------------------------
// Pixel Shader
// ------------------------------------------------------------

float4 PSMain(PSInput input) : SV_TARGET
{
    float2 uv = input.UV0;

    float2 texel = 1.0 / ScreenSize;

    //----------------------------------------------------------
    // Sample color
    //----------------------------------------------------------

    float4 color = MAT_SceneColor.Sample(MAT_SceneColorSampler, uv);

    //----------------------------------------------------------
    // Sample and linearize depth
    //----------------------------------------------------------

    float depth = LinearizeDepth(MAT_SceneDepth.Sample(MAT_SceneDepthSampler, uv));

    float depthR = LinearizeDepth(MAT_SceneDepth.Sample(MAT_SceneDepthSampler,
                    clamp(uv + float2(texel.x, 0), 0.0, 1.0)));

    float depthL = LinearizeDepth(MAT_SceneDepth.Sample(MAT_SceneDepthSampler,
                    clamp(uv - float2(texel.x, 0), 0.0, 1.0)));

    float depthU = LinearizeDepth(MAT_SceneDepth.Sample(MAT_SceneDepthSampler,
                    clamp(uv + float2(0, texel.y), 0.0, 1.0)));

    float depthD = LinearizeDepth(MAT_SceneDepth.Sample(MAT_SceneDepthSampler,
                    clamp(uv - float2(0, texel.y), 0.0, 1.0)));

    //----------------------------------------------------------
    // Edge detection (stable max method)
    //----------------------------------------------------------

    float edge =
        max(abs(depth - depthR),
        max(abs(depth - depthL),
        max(abs(depth - depthU),
            abs(depth - depthD))));

    edge *= DepthMultiplier;

    //----------------------------------------------------------
    // Convert edge strength to outline mask
    //----------------------------------------------------------

    float outline = smoothstep(
        OutlineThreshold,
        OutlineThreshold * 2.0,
        edge
    );

    //----------------------------------------------------------
    // Apply outline
    //----------------------------------------------------------

    float3 outlineColor = float3(0, 0, 0);

    color.rgb = lerp(color.rgb, outlineColor, outline);

    return color;
}