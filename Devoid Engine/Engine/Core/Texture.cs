using DevoidEngine.Engine.Rendering;
using DevoidGPU;

namespace DevoidEngine.Engine.Core
{
    public abstract class Texture
    {
        protected ISampler _sampler;
        protected SamplerDescription _samplerDescription = SamplerDescription.Default;

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public abstract void Bind(int slot = 0,
                                  ShaderStage stage = ShaderStage.Fragment,
                                  BindMode mode = BindMode.ReadOnly);

        public abstract void UnBind(int slot);

        protected void RecreateSampler()
        {
            _sampler?.Bind(0); // optional cleanup
            _sampler = Renderer.graphicsDevice.CreateSampler(_samplerDescription);
        }

        public void SetFilter(TextureFilter min, TextureFilter mag)
        {
            _samplerDescription.MinFilter = min;
            _samplerDescription.MagFilter = mag;
            RecreateSampler();
        }

        public void SetWrapMode(TextureWrapMode wrapS, TextureWrapMode wrapT)
        {
            _samplerDescription.WrapU = wrapS;
            _samplerDescription.WrapV = wrapT;
            RecreateSampler();
        }

        public void SetAnisotropy(int value)
        {
            _samplerDescription.MaxAnisotropy = value;
            RecreateSampler();
        }

        public void BindSampler(int slot)
        {
            _sampler?.Bind(slot);
        }
    }
}
