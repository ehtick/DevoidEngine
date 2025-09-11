using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public class ClusteredRenderer : IRenderPipeline
    {

        public Material GetDefaultMaterial()
        {
            throw new NotImplementedException();
        }

        public Framebuffer GetOutputFrameBuffer()
        {
            throw new NotImplementedException();
        }

        public void Initialize(int width, int height)
        {
            Console.WriteLine("Initializing Clustered Renderer");
        }

        public void BeginRender(Camera camera)
        {
            Console.WriteLine("Rendering");
        }

        public void Render()
        {

        }

        public void EndRender()
        {

        }

        public void Resize(int width, int height)
        {
            throw new NotImplementedException();
        }
    }
}
