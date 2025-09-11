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
        SamplerDescription samplerDescription = SamplerDescription.Default;

        public Texture2D()
        {
            sampler = Renderer.graphicsDevice.CreateSampler(samplerDescription);
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

        public void RecreateSampler()
        {
            this.sampler = Renderer.graphicsDevice.CreateSampler(samplerDescription);
        }

        


    }
}
