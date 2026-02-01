using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public enum ShaderStage
    {
        Vertex = 1 << 0,
        Fragment = 1 << 1,
        Geometry = 1 << 2,
        Compute = 1 << 3,
        All = Vertex | Fragment | Geometry | Compute
    }

    public interface IUniformBuffer
    {
        void SetData<T>(ref T data) where T : struct;
        void SetData(ReadOnlySpan<byte> data);
        void Bind(int slot, ShaderStage stage);
    }
}
