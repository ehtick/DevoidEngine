namespace DevoidGPU
{
    public enum DepthTest
    {
        Disabled,   // don’t test against depth buffer
        Always,     // always passes
        Less,       // pass if incoming depth < stored depth
        LessEqual,  // (default for opaque)
        Greater,
        GreaterEqual,
        Equal,
        NotEqual
    }

}
