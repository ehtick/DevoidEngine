using DevoidEngine.Engine.Rendering;
using DevoidGPU;

namespace DevoidEngine.Engine.Core
{
    public class Texture2D : Texture
    {
        private ITexture2D _deviceTexture;

        public static Texture2D WhiteTexture { get; private set; }
        public static ISampler DefaultSampler { get; private set; }

        public Tex2DDescription Description { get; private set; }

        static Texture2D()
        {
            WhiteTexture = new Texture2D(new Tex2DDescription()
            {
                Width = 1,
                Height = 1,
                Format = TextureFormat.RGBA16_Float,
                GenerateMipmaps = false,
                MipLevels = 1,
                IsDepthStencil = false,
                IsRenderTarget = true,
                IsMutable = false
            });
            DefaultSampler = Renderer.graphicsDevice.CreateSampler(SamplerDescription.Default);
        }

        public Texture2D(Tex2DDescription description)
        {
            Description = description;

            Width = description.Width;
            Height = description.Height;

            _deviceTexture = Renderer.graphicsDevice
                .TextureFactory
                .CreateTexture2D(description);

            _sampler = Renderer.graphicsDevice
                .CreateSampler(_samplerDescription);
        }

        public override void Bind(int slot = 0,
                                  ShaderStage stage = ShaderStage.Fragment,
                                  BindMode mode = BindMode.ReadOnly)
        {
            if (mode == BindMode.ReadOnly)
                _deviceTexture.Bind(slot);
            else
                _deviceTexture.BindMutable(slot);

            _sampler?.Bind(slot);
        }

        public override void UnBind(int slot)
        {
            _deviceTexture.UnBind(slot);
        }

        public void SetData(byte[] data)
        {
            _deviceTexture.SetData(data);
        }

        public void SetData<T>(T[] data) where T : unmanaged
        {
            _deviceTexture.SetData(data);
        }

        public void GenerateMipmaps()
        {
            _deviceTexture.GenerateMipmaps();
        }

        public ITexture2D GetDeviceTexture() { return _deviceTexture; }

        public void Resize(int width, int height)
        {
            _deviceTexture?.Dispose();

            var desc = Description;
            desc.Width = width;
            desc.Height = height;

            Width = width;
            Height = height;

            _deviceTexture = Renderer.graphicsDevice
                .TextureFactory
                .CreateTexture2D(desc);

            Description = desc;
        }
    }
}
