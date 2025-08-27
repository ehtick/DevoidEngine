using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface IIndexBuffer
    {
        int IndexCount { get; }
        BufferUsage Usage { get; }

        void SetData(int[] indices, int startIndex = 0);
        void UpdatePartial(int[] indices, int startIndex, int indexCount);
        void UpdatePartial(nint indices, int startIndex, int indexCount);
        void Bind();
        void Dispose();
    }
}
