using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface IShaderProgram
    {
        void AttachShader(IShader shader);
        void Link();
        void Bind();
    }
}
