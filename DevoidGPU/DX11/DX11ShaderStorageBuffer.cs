using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU.DX11
{
    public class DX11ShaderStorageBuffer<T> : IShaderStorageBuffer<T> where T : struct
    {

        public BufferUsage Usage { get; private set; }

        private readonly Device device;
        private readonly DeviceContext deviceContext;
        private readonly SharpDX.Direct3D11.Buffer buffer;

        public ShaderResourceView ShaderResourceView { get; private set; }
        public UnorderedAccessView UnorderedAccessView { get; private set; }

        public DX11ShaderStorageBuffer(Device device, DeviceContext context, int elementCount, BufferUsage usage, bool allowUAV)
        {
            this.device = device;
            this.deviceContext = context;

            int stride = SharpDX.Utilities.SizeOf<T>();
            int sizeInBytes = stride * elementCount;

            //int alignedSize = (sizeInBytes + 15) / 16 * 16;

            var desc = new BufferDescription()
            {
                Usage = usage == BufferUsage.Dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
                SizeInBytes = sizeInBytes,
                BindFlags = BindFlags.ShaderResource | (allowUAV && usage != BufferUsage.Dynamic ? BindFlags.UnorderedAccess : 0),
                OptionFlags = ResourceOptionFlags.BufferStructured,
                CpuAccessFlags = usage == BufferUsage.Dynamic ? CpuAccessFlags.Write : CpuAccessFlags.None,
                StructureByteStride = stride,
            };

            buffer = new SharpDX.Direct3D11.Buffer(device, desc);
            Usage = usage;

            ShaderResourceView = new ShaderResourceView(device, buffer);

            if (allowUAV)
            {
                UnorderedAccessView = new UnorderedAccessView(device, buffer);
            }
        }


        public void Bind(int slot, ShaderStage stages)
        {
            if ((stages & ShaderStage.Vertex) != 0)
                deviceContext.VertexShader.SetShaderResource(slot, ShaderResourceView);
            if ((stages & ShaderStage.Fragment) != 0)
                deviceContext.PixelShader.SetShaderResource(slot, ShaderResourceView);
            if ((stages & ShaderStage.Geometry) != 0)
                deviceContext.GeometryShader.SetShaderResource(slot, ShaderResourceView);
            if ((stages & ShaderStage.Compute) != 0)
                deviceContext.ComputeShader.SetShaderResource(slot, ShaderResourceView);
        }

        public void UnBind(int slot, ShaderStage stages = ShaderStage.All)
        {
            if ((stages & ShaderStage.Vertex) != 0)
                deviceContext.VertexShader.SetShaderResource(slot, null);
            if ((stages & ShaderStage.Fragment) != 0)
                deviceContext.PixelShader.SetShaderResource(slot, null);
            if ((stages & ShaderStage.Geometry) != 0)
                deviceContext.GeometryShader.SetShaderResource(slot, null);
            if ((stages & ShaderStage.Compute) != 0)
                deviceContext.ComputeShader.SetShaderResource(slot, null);
        }


        public void BindMutable(int slot)
        {
            deviceContext.ComputeShader.SetUnorderedAccessView(slot, UnorderedAccessView);
        }

        public void UnBindMutable(int slot)
        {
            deviceContext.ComputeShader.SetUnorderedAccessView(slot, null);
        }

        public void SetData(T[] data)
        {
            if (Usage == BufferUsage.Dynamic)
            {
                DataStream stream;
                deviceContext.MapSubresource(buffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out stream);
                stream.WriteRange(data);
                deviceContext.UnmapSubresource(buffer, 0);
                stream.Dispose();
            } else
            {
                deviceContext.UpdateSubresource(data, buffer);
            }
        }

        public void UpdatePartial(T[] data, int startIndex, int elementCount, int elementOffset = 0)
        {
            int stride = SharpDX.Utilities.SizeOf<T>();
            int byteOffset = elementOffset * stride;       // where in the GPU buffer to start
            int byteSize = elementCount * stride;        // how much to write

            if (Usage == BufferUsage.Dynamic)
            {
                DataStream stream;
                deviceContext.MapSubresource(buffer, 0, MapMode.WriteNoOverwrite, MapFlags.None, out stream);

                stream.Position = byteOffset;
                stream.WriteRange(data, startIndex, elementCount);

                deviceContext.UnmapSubresource(buffer, 0);
                stream.Dispose();
            }
            else
            {
                var handle = System.Runtime.InteropServices.GCHandle.Alloc(data, System.Runtime.InteropServices.GCHandleType.Pinned);
                try
                {
                    IntPtr ptr = System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(data, startIndex);

                    deviceContext.UpdateSubresource(
                        new DataBox(ptr, 0, 0),
                        buffer,
                        0,
                        new ResourceRegion()
                        {
                            Left = byteOffset,
                            Right = byteOffset + byteSize,
                            Top = 0,
                            Front = 0,
                            Bottom = 1,
                            Back = 1
                        });
                }
                finally
                {
                    handle.Free();
                }
            }
        }




    }
}
