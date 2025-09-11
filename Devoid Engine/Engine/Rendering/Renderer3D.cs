using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{



    public static class Renderer3D
    {
        public static IRenderPipeline ActiveRenderingPipeline;

        public static void Initialize(int width, int height)
        {
            ActiveRenderingPipeline.Initialize(width, height);
        }

        public static void BeginRender(Camera camera)
        {
            ActiveRenderingPipeline.BeginRender(camera);
        }

        public static void Render()
        {
            Console.WriteLine("Rendering!");
            ActiveRenderingPipeline.Render();
        }

        public static void EndRender()
        {
            ActiveRenderingPipeline.EndRender();
        }
    }
}
