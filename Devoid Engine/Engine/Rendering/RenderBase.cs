using DevoidEngine.Engine.Core;
using DevoidGPU;
using System.Numerics;

namespace DevoidEngine.Engine.Rendering
{
    public static class RenderBase
    {
        struct MeshRenderData
        {
            public Matrix4x4 ModelMatrix;
            public Matrix4x4 ModelMatrixInv;
        }

        struct CameraRenderData
        {

        }

        static IRenderTechnique ActiveRenderTechnique;

        // Renderer objects
        static MeshRenderData _meshRenderData;
        static UniformBuffer _meshRenderDataBuffer;

        public static void SetupCamera()
        {
            // Per camera data goes here
        }


        public unsafe static void Initialize(int width, int height)
        {
            _meshRenderData = new MeshRenderData();
            _meshRenderDataBuffer = new UniformBuffer(sizeof(MeshRenderData));


            ActiveRenderTechnique?.Initialize(width, height);
        }

        public static void Resize(int width, int height)
        {
            ActiveRenderTechnique?.Resize(width, height);
        }

        public static void Render(CameraRenderContext ctx)
        {
            if (ActiveRenderTechnique == null)
                Console.WriteLine("[Renderer]: Render technique was not set. No Object rendered.");

            SetupCamera();

            ActiveRenderTechnique?.Render(ctx);
        }

        public static void Draw(RenderItem item)
        {
            // 1. Bind material (shader + textures + uniforms)
            item.Material.Apply();

            item.Mesh.VertexBuffer.Bind();
            item.Mesh.IndexBuffer?.Bind();

            IInputLayout layout = Renderer.GetInputLayout(item.Mesh, item.Material.BaseMaterial.Shader);

            if (item.Mesh.IndexBuffer != null)
            {
                Renderer.graphicsDevice.DrawIndexed(item.Mesh.IndexBuffer.IndexCount, 0, 0);
            }
            else
            {
                Renderer.graphicsDevice.Draw(item.Mesh.VertexBuffer.VertexCount, 0);
            }
        }


    }
}
