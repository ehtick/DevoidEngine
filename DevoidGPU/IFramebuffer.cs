using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface IFramebuffer
    {
        List<ITexture2D> ColorAttachments { get; }
        ITexture2D DepthAttachment { get; }

        void Bind();
        int Width { get; }
        int Height { get; }

        void AddColorAttachment(ITexture2D texture, int index = 0);
        void AddDepthAttachment(ITexture2D texture);

        void ClearColor(Vector4 color);
        void ClearDepth(float depth = 1.0f);

    }
}
