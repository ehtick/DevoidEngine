using DevoidGPU;
using System;

namespace DevoidEngine.Engine.Core
{
    public class Material
    {

        public BlendMode BlendMode { get; set; } = BlendMode.Opaque;
        public DepthTest DepthTest { get; set; } = DepthTest.LessEqual;
        public bool DepthWrite { get; set; } = true;
        public CullMode CullMode { get; set; } = CullMode.Back;


    }
}
