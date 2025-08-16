using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface IBufferFactory
    {
        IUniformBuffer CreateUniformBuffer<T>() where T : struct;

        IVertexBuffer CreateVertexBuffer(BufferUsage bufferUsage, VertexInfo vertexInfo, int vertexCount);
        IIndexBuffer CreateIndexBuffer();

        IFramebuffer CreateFramebuffer();
    }
}
