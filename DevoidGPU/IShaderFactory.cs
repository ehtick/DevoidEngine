using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface IShaderFactory
    {
        IShader CreateShader(ShaderType shaderType, string source, string entrypoint);
        IShaderProgram CreateShaderProgram();

        IComputeShader CreateComputeShader(string source, string entrypoint);

    }
}
