using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU.DX11
{
    public class DX11BufferFactory : IBufferFactory
    {
        Device device;
        DeviceContext deviceContext;
        public DX11BufferFactory(Device device, DeviceContext deviceContext)
        {
            this.device = device;
            this.deviceContext = deviceContext;

        }

        public IUniformBuffer CreateUniformBuffer<T>(BufferUsage bufferUsage) where T : struct
        {
            return new DX11UniformBuffer(device, deviceContext, Marshal.SizeOf<T>(), bufferUsage);
        }

        public IUniformBuffer CreateUniformBuffer(int size, BufferUsage bufferUsage)
        {
            return new DX11UniformBuffer(device, deviceContext, size, bufferUsage);
        }

        public IShaderStorageBuffer<T> CreateShaderStorageBuffer<T>(int elementCount, BufferUsage usage, bool allowUAV = false) where T : struct
        {
            return new DX11ShaderStorageBuffer<T>(device, deviceContext, elementCount, usage, allowUAV);
        }

        public IIndexBuffer CreateIndexBuffer(int indexCount, BufferUsage usage, bool is16Bit = false)
        {
            return new DX11IndexBuffer(device, deviceContext, indexCount, usage, is16Bit);
        }

        public IVertexBuffer CreateVertexBuffer(BufferUsage bufferUsage, VertexInfo vertexInfo, int vertexCount)
        {
            DX11VertexBuffer dX11VertexBuffer = new DX11VertexBuffer(this.device, this.deviceContext, vertexInfo, vertexCount, bufferUsage);
            return dX11VertexBuffer;
        }

        public IFramebuffer CreateFramebuffer()
        {
            DX11Framebuffer framebuffer = new DX11Framebuffer(this.device, deviceContext);
            return framebuffer;
        }
    }
}
