using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface ISampler
    {
        SamplerDescription Description { get; }
        void Bind(int slot = 0);
    }
}
