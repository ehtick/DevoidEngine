using DevoidEngine.Engine.Core;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering.PostProcessing
{
    public class ToneMapPass : RenderGraphPass
    {
        public RGResource Input;
        public RGResource Output;

        Framebuffer framebuffer;
        Texture2D outputTexture;

        int width;
        int height;

        public ToneMapPass(int width, int height)
        {
            this.width = width;
            this.height = height;

            framebuffer = new Framebuffer();

            framebuffer.AttachRenderTexture(
                new Texture2D(new TextureDescription()
                {
                    Width = width,
                    Height = height,
                    GenerateMipmaps = false,
                    MipLevels = 1,
                    IsRenderTarget = true,
                    Type = TextureType.Texture2D,
                    Format = TextureFormat.RGBA8_UNorm
                })
            );

            outputTexture = framebuffer.GetRenderTexture(0);
        }

        public override void Setup(RenderGraphBuilder builder)
        {
            builder.Read(Input);

            Output = builder.Create(
                "ToneMapped",
                new TextureDescription
                {
                    Width = width,
                    Height = height,
                    Format = TextureFormat.RGBA8_UNorm,
                    IsRenderTarget = true
                });

            builder.Write(Output);
        }

        public override void Execute(RenderGraphContext ctx)
        {
            Texture2D input = ctx.GetTexture(Input);

            framebuffer.Bind();
            framebuffer.Clear();

            RenderAPI.RenderToBuffer(input, framebuffer);

            // override graph texture with framebuffer output
            ctx.SetTexture(Output, outputTexture);
        }

        public void Resize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}

