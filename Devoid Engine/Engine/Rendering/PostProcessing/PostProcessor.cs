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
        List<RenderGraphPass> passes = new();

        bool dirty = true;

        public void AddPass(RenderGraphPass pass)
        {
            passes.Add(pass);
            graph.AddPass(pass);
            dirty = true;
        }

        public void RemovePass(RenderGraphPass pass)
        {
            passes.Remove(pass);
            graph.RemovePass(pass);
            dirty = true;
        }

        public Texture2D Run(Texture2D input)
        {
            if (dirty)
            {
                graph.Compile();
                dirty = false;
            }

            graph.Execute();

            return input;
        }
    }
}
