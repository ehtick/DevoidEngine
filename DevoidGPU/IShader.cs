using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public enum ShaderType
    {
        Vertex,
        Fragment,
        Geometry,
        Compute,
        TessControl,
        TessEval
    }
    public interface IShader
    {
        ShaderType Type { get; }
        string Name { get; }

        void Compile(string source, string entryPoint);

    }
}
