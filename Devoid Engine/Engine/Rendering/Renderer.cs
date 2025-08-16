using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public static class Renderer
    {
        public static IGraphicsDevice graphicsDevice;

        public static void Initialize(IGraphicsDevice gd)
        {
            graphicsDevice = gd;
        }

    }
}
