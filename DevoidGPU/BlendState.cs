namespace DevoidGPU
{
    public enum BlendMode
    {
        Opaque,       // no blending
        AlphaBlend,   // srcAlpha / (1 - srcAlpha)
        Additive,     // src + dst
        Multiply
    }
}