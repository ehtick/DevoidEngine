using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU.DX11
{
    public class DX11InputLayout : IInputLayout
    {
        public InputLayout NativeLayout { get; }

        public VertexInfo VertexInfo { get; }

        private readonly DeviceContext deviceContext;

        public DX11InputLayout(Device device, DeviceContext deviceContext, VertexInfo vertexInfo, DX11Shader shader)
        {
            this.deviceContext = deviceContext;

            VertexInfo = vertexInfo;

            NativeLayout = new InputLayout(
                device,
                shader.ByteCode,
                DXUtils.CreateInputElements(vertexInfo)
            );
        }


        public void Bind()
        {
            deviceContext.InputAssembler.InputLayout = NativeLayout;
        }


    }
}
