using DevoidEngine.Engine.Core;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    struct PerFrameData
    {

    }

    public static class Renderer
    {
        public static IGraphicsDevice graphicsDevice;

        public static int Width;
        public static int Height;

        public static void Initialize(IGraphicsDevice gd, int width, int height)
        {
            graphicsDevice = gd;

            Renderer3D.ActiveRenderingPipeline = new ClusteredRenderer();

            Renderer3D.Initialize(width, height);
        }

        public static void Resize(int width, int height)
        {
            Width = width;
            Height = height;


        }

    }
}
