using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public class RenderGraphContext
    {
        Dictionary<int, RenderGraphResource> resources;

        public RenderGraphContext(Dictionary<int, RenderGraphResource> res)
        {
            resources = res;
        }

        public Texture GetTexture(RGResource handle)
        {
            return resources[handle.Id].Texture;
        }
    }
}
