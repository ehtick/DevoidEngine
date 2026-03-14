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
        List<RenderGraphPass> passes = new();
        RenderGraph graph = new();

        public void AddPass(RenderGraphPass pass)
        {
            passes.Add(pass);
        }

        public Texture2D Run(Texture2D sceneColor)
        {
            graph.Clear();

            foreach (var pass in passes)
            {
                graph.AddPass(pass);
            }

            graph.Execute();

            return sceneColor;
        }
    }
}
