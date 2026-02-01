using DevoidEngine.Engine.Rendering;
using System;

namespace DevoidEngine.Engine.Core
{
    public class Graphics
    {


        public static void DrawMeshIndexed(RenderInstance renderInstance)
        {
            var cmd = EnginePipeline.DrawMeshIndexedPool.Get();
            cmd.instance = renderInstance;
            EnginePipeline.RenderCommands.Add(cmd);
        }

        public static void SetCamera(Camera camera)
        {
            var cmd = EnginePipeline.SetViewInfoPool.Get();
            cmd.camera = camera;
            EnginePipeline.RenderCommands.Add(cmd);
        }

        internal static void SetRendererState(RendererStateCommnandType commandType)
        {
            var cmd = EnginePipeline.Renderer3DStateCommand.Get();
            cmd.state = commandType;
            EnginePipeline.RenderCommands.Add(cmd);
        }
    }
}
