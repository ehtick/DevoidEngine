using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering;
using DevoidGPU;

namespace DevoidEngine.Engine.Rendering.PostProcessing
{
    public class TonemapPass : RenderGraphPass
    {
        Texture2D input;
        Texture2D output;

        Framebuffer framebuffer;

        int width;
        int height;

        public Texture2D OutputTexture => output;

        public TonemapPass(int width, int height)
        {
            this.width = width;
            this.height = height;

            CreateResources();
        }

        void CreateResources()
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

        public void SetInput(Texture2D input)
        {
            this.input = input;
        }

        public override void Setup()
        {
            Read("SceneColor");
            Write("ToneMapped");
            Console.WriteLine("Tonemap Pass has been set up!");
        }

        public override void Execute(RenderGraphContext ctx)
        {
            //RenderAPI.RenderToBuffer(input, framebuffer);
        }
    }
}