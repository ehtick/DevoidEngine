using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public struct RGResource
    {
        public int Id;

        public RGResource(int id)
        {
            Id = id;
        }

        public bool IsValid => Id >= 0;
    }
}
