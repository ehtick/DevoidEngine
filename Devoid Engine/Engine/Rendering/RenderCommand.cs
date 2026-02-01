using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{
    public enum RendererStateCommnandType
    {
        Begin,
        End
    }

    public interface IRenderCommand
    {

    }

    public class Render3DStateCommand : IRenderCommand
    {
        public RendererStateCommnandType state;
    }

    public class SetViewInfoCommand3D : IRenderCommand
    {
        public Camera camera;
    }

    public class SetViewInfoCommand2D : IRenderCommand
    {
        public Camera camera;
    }

    public class DrawMeshCommand : IRenderCommand
    {

    }

    public class DrawMeshIndexed : IRenderCommand
    {
        public RenderInstance instance;
    }
}
