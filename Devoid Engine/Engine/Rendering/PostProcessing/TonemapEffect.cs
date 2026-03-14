using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering.PostProcessing
{
    public class ToneMapEffect : IPostProcessEffect
    {
        ToneMapPass pass;

        public void Initialize(int width, int height)
        {
            pass = new ToneMapPass(width, height);
        }

        public void Resize(int width, int height)
        {
            pass.Resize(width, height);
        }

        public void Build(RenderGraph graph, RGResource input, out RGResource output)
        {
            pass.Input = input;

            graph.AddPass(pass);

            output = pass.Output;
        }
    }
}
