using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering.PostProcessing
{
    public class TonemapPass : RenderGraphPass
    {
        Texture2D input;
        Framebuffer framebuffer;

        public Texture2D Output;

        public TonemapPass(Texture2D input, Texture2D output, Framebuffer framebuffer)
        {
            this.input = input;
            this.Output = output;
            this.framebuffer = framebuffer;
        }

        public override void Setup()
        {
            Read("SceneColor");
            Write("ToneMapped");
        }

        public override void Execute(RenderGraphContext ctx)
        {
            RenderAPI.RenderToBuffer(input, framebuffer);
        }
    }
}
