namespace DevoidGPU
{
    public interface ITextureFactory
    {
        ITexture2D CreateTexture2D(Tex2DDescription description);
        ITexture3D CreateTexture3D();
        ITextureCube CreateTextureCube();


    }
}
