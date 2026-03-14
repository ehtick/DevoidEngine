using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering.GPUResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public class RenderGraphResource
    {
        public string Name;

        public Texture2D Texture;

        public bool Imported;

        public RenderGraphPass Producer;
    }
}
