using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface IPresentSurface : IDisposable
    {
        int Width { get; }
        int Height { get; }
        bool VSync { get; set; }
        void Resize(int width, int height);
        void Bind();
        void Present();
        void ClearColor(System.Numerics.Vector4 Color);

    }
}
