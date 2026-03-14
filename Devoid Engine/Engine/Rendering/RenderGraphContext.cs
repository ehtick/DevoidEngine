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

        public Texture2D GetTexture(RGResource handle)
        {
            return resources[handle.Id].Texture;
        }

        public void SetTexture(RGResource handle, Texture2D tex)
        {
            resources[handle.Id].Texture = tex;
        }
    }
}
