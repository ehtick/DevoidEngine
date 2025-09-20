using DevoidEngine.Engine.Rendering;
using DevoidGPU;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public class Texture2D
    {
        ISampler sampler;
        ITexture2D texture;

        public TextureFormat TextureFormat { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Tex2DDescription Description { get; private set; }

        private bool isMutable = false;
        private bool isRenderTarget = false;
        private bool isDepthTarget = false;
        
        SamplerDescription samplerDescription = SamplerDescription.Default;

        public static Texture2D WhiteTexture { get; private set; }
        public static ISampler DefaultSampler { get; private set; }

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
            this.Description = description;
            this.Width = description.Width;
            this.Height = description.Height;
            this.TextureFormat = description.Format;

            this.isMutable = description.IsMutable;
            this.isRenderTarget = description.IsRenderTarget;
            this.isDepthTarget = description.IsDepthStencil;

            sampler = Renderer.graphicsDevice.CreateSampler(samplerDescription);
            texture = Renderer.graphicsDevice.TextureFactory.CreateTexture2D(description);

        }

        public void SetData(byte[] data)
        {
            texture.SetData(data);
        }

        public void SetData<T>(T[] data) where T : unmanaged
        {
            texture.SetData<T>(data);
        }

        public void GenerateMipmaps()
        {
            texture.GenerateMipmaps();
        }

        public void Resize(int width, int height)
        {
            texture?.Dispose();
            texture = Renderer.graphicsDevice.TextureFactory.CreateTexture2D(Description);
        }

        public void SetFilter(TextureFilter min, TextureFilter mag)
        {
            samplerDescription.MinFilter = min;
            samplerDescription.MagFilter = mag;

            RecreateSampler();
        }

        public void SetWrapMode(TextureWrapMode wrapS, TextureWrapMode wrapT)
        {
            samplerDescription.WrapU = wrapS;
            samplerDescription.WrapV = wrapT;
            
            RecreateSampler();
        }

        public void SetAnisotropy(float value)
        {
            samplerDescription.MaxAnisotropy = (int)value;

            RecreateSampler();
        }

        public void Bind(int slot = 0, ShaderStage stages = ShaderStage.Fragment, BindMode mode = BindMode.ReadOnly)
        {
            if (mode == BindMode.ReadOnly)
            {
                texture.Bind(slot);
            } else if (mode == BindMode.ReadWrite)
            {
                texture.BindMutable(slot);
            }
        }

        public void BindSampler(int slot)
        {
            sampler.Bind(slot);
        }

        public void UnBind(int slot)
        {
            texture.UnBind(slot);
        }

        public ITexture2D GetDeviceTexture()
        {
            return texture;
        }

        public void RecreateSampler()
        {
            this.sampler = Renderer.graphicsDevice.CreateSampler(samplerDescription);
        }

        


    }
}
