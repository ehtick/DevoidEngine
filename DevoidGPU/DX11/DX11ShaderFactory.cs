using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU.DX11
{
    public class DX11ShaderFactory : IShaderFactory
    {
        Device device;
        DeviceContext deviceContext;
        public DX11ShaderFactory(Device device, DeviceContext deviceContext)
        {
            this.device = device;
            this.deviceContext = deviceContext;
        }

        public IShader CreateShader(ShaderType shaderType, string source, string entrypoint)
        {
            DX11Shader shader = new DX11Shader(device, shaderType);
            shader.Compile(source, entrypoint);
            return shader;
        }

        public IShaderProgram CreateShaderProgram()
        {
            return new DX11ShaderProgram(deviceContext);
        }
    }
}
