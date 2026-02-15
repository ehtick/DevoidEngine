using System.Numerics;

namespace DevoidEngine.Engine.Core
{
    public class RenderInstance
    {
        public Mesh Mesh;
        public int MaterialHandle;
        public Matrix4x4 WorldMatrix;
    }

    public static class SceneRenderSystem
    {
        public static List<RenderInstance> RenderInstances = new List<RenderInstance>();

        public static RenderInstance SubmitMesh(Mesh mesh, int MaterialHandle, Matrix4x4 WorldMatrix)
        {
            RenderInstances.Add(new RenderInstance()
            {
                Mesh = mesh,
                MaterialHandle = MaterialHandle,
                WorldMatrix = WorldMatrix
            });
            return RenderInstances.Last();
        }
    }
}
