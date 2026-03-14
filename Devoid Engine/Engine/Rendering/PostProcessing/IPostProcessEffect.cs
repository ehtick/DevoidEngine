using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering.PostProcessing
{
    public interface IPostProcessEffect
    {
        void Initialize(int width, int height);

        void Build(RenderGraph graph, RGResource input, out RGResource output);

        void Resize(int width, int height);
    }
}
