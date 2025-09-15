using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public class Graphics
    {
        public static void DrawMeshIndexed(Mesh mesh, int material, Matrix4x4 worldMatrix)
        {
            var cmd = EnginePipeline.DrawMeshIndexedPool.Get();
            cmd.Mesh = mesh;
            cmd.MaterialHandle = material;
            cmd.WorldMatrix = worldMatrix;
            EnginePipeline.RenderCommands.Add(cmd);
        }

        public static void SetCamera(Camera camera)
        {
            var cmd = EnginePipeline.SetViewInfoPool.Get();
            cmd.camera = camera;
            EnginePipeline.RenderCommands.Add(cmd);
        }
    }
}
