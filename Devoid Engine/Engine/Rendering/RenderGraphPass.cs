using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public abstract class RenderGraphPass
    {
        public List<RGResource> Reads = new();
        public List<RGResource> Writes = new();

        public abstract void Setup(RenderGraphBuilder builder);

        public abstract void Execute(RenderGraphContext ctx);
    }
}
