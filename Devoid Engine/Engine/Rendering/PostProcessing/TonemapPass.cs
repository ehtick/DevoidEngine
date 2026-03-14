using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidGPU;

namespace DevoidEngine.Engine.Rendering.PostProcessing
{
    public class TonemapPass : RenderGraphPass
    {
        Texture2D output;
        Framebuffer framebuffer;

        public override Texture2D OutputTexture => output;

        public TonemapPass(int width, int height)
        {
            output = new Texture2D(new TextureDescription()
            {
                Width = width,
                Height = height,
                Format = TextureFormat.RGBA16_Float,
                IsRenderTarget = true
            });

            framebuffer = new Framebuffer();
            framebuffer.AttachRenderTexture(output);
        }

        public override void Setup()
        {
            Read("SceneColor");
            Write("ToneMapped");
        }

        public override void Execute(RenderGraphContext ctx)
        {
            Texture2D input = ctx.GetTexture("SceneColor");

            RenderAPI.RenderToBuffer(input, framebuffer);

            ctx.SetTexture("ToneMapped", output);
            //ctx.SetTexture("Final", output); // optional if this is last pass
        }
    }
}