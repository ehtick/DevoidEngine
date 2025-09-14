using DevoidEngine.Engine.Rendering;
using DevoidGPU;
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
        
        SamplerDescription samplerDescription = SamplerDescription.Default;

        public Texture2D()
        {
            sampler = Renderer.graphicsDevice.CreateSampler(samplerDescription);
            texture = Renderer.graphicsDevice.TextureFactory.CreateTexture2D(1, 1, TextureFormat.RGBA16_Float);

        }

        public Texture2D(int width, int height, TextureFormat format, bool isMutable = false, bool isRenderTarget = true, bool isDepthTarget = false)
        {
            sampler = Renderer.graphicsDevice.CreateSampler(samplerDescription);
            texture = Renderer.graphicsDevice.TextureFactory.CreateTexture2D(width, height, format, isMutable, isDepthTarget, isRenderTarget);

        }

        public void SetData(byte[] data)
        {
            texture.SetData(data);
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

        public void Bind(int slot = 0, BindMode mode = BindMode.ReadOnly)
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
