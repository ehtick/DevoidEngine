using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public static class InputLayoutManager
    {
        public static Dictionary<(VertexInfo, IShader), IInputLayout> inputLayoutCache = new Dictionary<(VertexInfo, IShader), IInputLayout>();
    }
}
