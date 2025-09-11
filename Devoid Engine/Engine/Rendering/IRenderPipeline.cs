using DevoidEngine.Engine.Core;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public interface IRenderPipeline
    {
        void Initialize(int width, int height);
        void BeginRender(Camera camera);
        void Render();
        void EndRender();

        void Resize(int width, int height);

        Framebuffer GetOutputFrameBuffer();

        Material GetDefaultMaterial();
    }
}
