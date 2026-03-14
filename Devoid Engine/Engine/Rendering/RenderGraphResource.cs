using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering.GPUResource;
using DevoidGPU;
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

        public TextureDescription Description;

        public bool Imported;

        public bool Allocated;

        public RenderGraphPass Producer;
    }
}
