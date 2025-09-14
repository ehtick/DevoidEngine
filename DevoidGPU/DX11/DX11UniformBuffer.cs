using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU.DX11
{
    public class DX11UniformBuffer : IUniformBuffer
    {
        public BufferUsage Usage { get; private set; }

        private readonly Device device;
        private readonly DeviceContext deviceContext;
        private readonly SharpDX.Direct3D11.Buffer buffer;

        public DX11UniformBuffer(Device device, DeviceContext deviceContext, int sizeInBytes, BufferUsage bufferUsage)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            this.deviceContext = deviceContext ?? throw new ArgumentNullException(nameof(deviceContext));

            int alignedSize = (sizeInBytes + 15) / 16 * 16;

            var desc = new BufferDescription
            {
                Usage = DX11StateMapper.ToDXResourceUsage(bufferUsage),
                SizeInBytes = alignedSize,
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = bufferUsage == BufferUsage.Dynamic ? CpuAccessFlags.Write : CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            buffer = new SharpDX.Direct3D11.Buffer(device, desc);


            this.Usage = bufferUsage;
        }

        public void SetData<T>(ref T data) where T : struct
        {
            if (this.Usage == BufferUsage.Dynamic)
            {
                DataBox dataBox = deviceContext.MapSubresource(
                    buffer,
                    0,
                    MapMode.WriteDiscard, // Use WriteDiscard or WriteNoOverwrite
                    MapFlags.None
                );

                // Copy the data
                Utilities.Write(dataBox.DataPointer, ref data);

                deviceContext.UnmapSubresource(buffer, 0);
            }
            else
            {
                deviceContext.UpdateSubresource(ref data, buffer);
            }
        }
        public void Bind(int slot, ShaderStage stages)
        {
            if ((stages & ShaderStage.Vertex) != 0)
                deviceContext.VertexShader.SetConstantBuffer(slot, buffer);
            if ((stages & ShaderStage.Fragment) != 0)
                deviceContext.PixelShader.SetConstantBuffer(slot, buffer);
            if ((stages & ShaderStage.Geometry) != 0)
                deviceContext.GeometryShader.SetConstantBuffer(slot, buffer);
            if ((stages & ShaderStage.Compute) != 0)
                deviceContext.ComputeShader.SetConstantBuffer(slot, buffer);
        }

        public void Dispose()
        {
            buffer?.Dispose();
        }
    }
}
