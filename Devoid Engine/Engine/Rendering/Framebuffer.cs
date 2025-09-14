using DevoidEngine.Engine.Core;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public class Framebuffer
    {
        IFramebuffer frameBuffer;

        List<Texture2D> RenderTextures;
        public Framebuffer()
        {
            frameBuffer = Renderer.graphicsDevice.BufferFactory.CreateFramebuffer();
            RenderTextures = new List<Texture2D>();

        }

        public void Bind()
        {
            frameBuffer.Bind();
        }

        public void Clear()
        {
            frameBuffer.ClearColor(new Vector4(0,0,0,1));
            frameBuffer.ClearDepth(0);
        }

        public Texture2D GetRenderTexture(int index)
        {
            return RenderTextures[index];
        }

        public void AttachRenderTexture(Texture2D texture)
        {
            frameBuffer.AddColorAttachment(texture.GetDeviceTexture());
            RenderTextures.Add(texture);
        }

        public void AttachDepthTexture(Texture2D texture)
        {
            frameBuffer.AddDepthAttachment(texture.GetDeviceTexture());
        }



    }
}
