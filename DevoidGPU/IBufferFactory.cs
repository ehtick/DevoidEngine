using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface IBufferFactory
    {
        IUniformBuffer CreateUniformBuffer<T>(BufferUsage bufferUsage) where T : struct;
        IUniformBuffer CreateUniformBuffer(int size, BufferUsage bufferUsage);
        IShaderStorageBuffer<T> CreateShaderStorageBuffer<T>(int elementCount, BufferUsage usage, bool allowUAV = false) where T : struct;

        IVertexBuffer CreateVertexBuffer(BufferUsage bufferUsage, VertexInfo vertexInfo, int vertexCount);
        IIndexBuffer CreateIndexBuffer(int indexCount, BufferUsage usage, bool is16Bit = false);

        IFramebuffer CreateFramebuffer();
    }
}
