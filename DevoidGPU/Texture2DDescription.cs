namespace DevoidGPU
{
    public struct Tex2DDescription
    {
        public int Width;
        public int Height;
        public TextureFormat Format;

        public int MipLevels;
        public bool GenerateMipmaps;
        public bool IsRenderTarget;
        public bool IsDepthStencil;
        public bool IsMutable;


    }
}
