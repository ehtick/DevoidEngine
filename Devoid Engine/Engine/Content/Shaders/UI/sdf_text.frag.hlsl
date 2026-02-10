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

Texture2D fontSDFAtlas : register(t0);
SamplerState fontSDFAtlasSampler : register(s0);

float4 PSMain(PSInput input) : SV_TARGET
{
    // 1. Sample the distance field (Must use Linear Sampler!)
    float d = fontSDFAtlas.Sample(fontSDFAtlasSampler, input.UV0).r;
    
    // 2. Decode distance 
    // (Assuming you stored it as 0.5 + dist/spread)
    // If your "spread" in the generator was 8 pixels, strictly speaking you don't 
    // need to decode it back to pixels for this algorithm, but it helps mentally.
    // Let's stick to the raw 0..1 value for simplicity.
    
    // 3. Calculate Smoothing Factor (Anti-Aliasing)
    // fwidth(d) approximates the gradient of the distance field in screen pixels.
    // If the texture is low-res, this gradient might be noisy.
    
    // Ideally, we want the smoothing window to be 1 screen pixel wide.
    // length(fwidth(input.UV0)) gives us "how much UV changes per screen pixel".
    // We multiply by TextureSize to get "how many texels change per screen pixel".
    
    // However, the standard fwidth(d) approach usually works IF sampling is Linear.
    // Let's stick to your approach but clamp it to prevent artifacts.
    
    float lowerEdge = 0.5 - fwidth(d);
    float upperEdge = 0.5 + fwidth(d);

    // 4. Smoothstep with protection
    float alpha = smoothstep(lowerEdge, upperEdge, d);

    // 5. Optimization (Optional)
    if (alpha <= 0.01)
        discard;

    return float4(1, 1, 1, alpha);
}
