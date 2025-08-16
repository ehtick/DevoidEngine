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

        public IUniformBuffer CreateUniformBuffer<T>() where T : struct
        {
            return new DX11UniformBuffer(device, deviceContext, Marshal.SizeOf<T>());
        }

        public IIndexBuffer CreateIndexBuffer()
        {
            throw new NotImplementedException();
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
