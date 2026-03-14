using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public class RenderGraphBuilder
    {
        RenderGraph graph;
        RenderGraphPass pass;

        internal RenderGraphBuilder(RenderGraph g, RenderGraphPass p)
        {
            graph = g;
            pass = p;
        }

        public RGResource Read(string name)
        {
            var res = graph.GetResource(name);
            pass.Reads.Add(res);
            return res;
        }

        public RGResource Write(string name)
        {
            var res = graph.GetResource(name);
            pass.Writes.Add(res);
            graph.RegisterProducer(res, pass);
            return res;
        }

        public RGResource Read(RGResource res)
        {
            pass.Reads.Add(res);
            return res;
        }

        public RGResource Write(RGResource res)
        {
            pass.Writes.Add(res);
            graph.RegisterProducer(res, pass);
            return res;
        }

        public RGResource Create(string name, TextureDescription desc)
        {
            return graph.CreateResource(name, desc, pass);
        }
    }
}
