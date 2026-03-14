using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering.PostProcessing
{
    public class PostProcessor
    {
        RenderGraph graph = new();

        public void AddPass(RenderGraphPass pass)
        {
            graph.AddPass(pass);
        }

        public void RemovePass(RenderGraphPass pass)
        {
            graph.RemovePass(pass);
        }

        public Texture2D Run(Texture2D input)
        {
            return graph.Execute(input);
        }
    }
}
