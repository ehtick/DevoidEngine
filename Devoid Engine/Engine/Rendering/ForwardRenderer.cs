using DevoidEngine.Engine.Core;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering
{

    class ForwardRenderer : IRenderPipeline
    {

        MaterialInstance BasicForward;

        RenderMesh3DData MeshRenderData;
        CameraData CameraData;

        IUniformBuffer CameraDataBuffer;
        IUniformBuffer MeshDataBuffer;

        public void Initialize(int width, int height)
        {
            Console.WriteLine("Initializing Forward Renderer at " + width + " " + height);

            CameraDataBuffer = Renderer.graphicsDevice.BufferFactory.CreateUniformBuffer(Marshal.SizeOf<CameraData>(), BufferUsage.Dynamic);
            MeshDataBuffer = Renderer.graphicsDevice.BufferFactory.CreateUniformBuffer(Marshal.SizeOf<RenderMesh3DData>(), BufferUsage.Dynamic);

            SetupMaterial();
        }

        private void SetupMaterial()
        {
            Material pbrMaterial = new Material();
            pbrMaterial.Shader = ShaderLibrary.GetShader("PBR/ForwardPBR");

            
            pbrMaterial.MaterialLayout = new MaterialLayout()
            {
                bufferSize = 32,
                Properties =
                {
                    new ShaderPropertyInfo() {Name = "Albedo", Offset = 0, Type = ShaderPropertyType.Vector4 },
                }
            };

            pbrMaterial.PropertiesVec4["Albedo"] = new System.Numerics.Vector4(1, 0.69f, 1, 1);

            this.BasicForward = new MaterialInstance(MaterialManager.RegisterMaterial(pbrMaterial));
        }

        public MaterialInstance GetDefaultMaterial()
        {
            return BasicForward;
        }

        public Framebuffer GetOutputFrameBuffer()
        {
            throw new NotImplementedException();
        }

        public void BeginRender(Camera camera)
        {
            CameraData cameraData = camera.GetCameraData();

            CameraDataBuffer.SetData(ref cameraData);

            CameraDataBuffer.Bind(0, ShaderStage.Vertex | ShaderStage.Fragment);

            camera.RenderTarget.Bind();
            camera.RenderTarget.Clear();
            //Renderer.graphicsDevice.SetViewport(0, 0, Renderer.Width, Renderer.Height);

            Renderer.graphicsDevice.SetBlendState(BlendMode.Opaque);
            Renderer.graphicsDevice.SetDepthState(DepthTest.LessEqual, true);
            Renderer.graphicsDevice.SetRasterizerState(CullMode.None);
            Renderer.graphicsDevice.SetPrimitiveType(PrimitiveType.Triangles);
        }

        public void EndRender()
        {
        }

        public void Render(List<RenderInstance> renderInstances)
        {
            MeshDataBuffer.Bind(1, ShaderStage.Vertex | ShaderStage.Fragment);

            for (int i = 0; i < renderInstances.Count; i++)
            {
                RenderInstance instance = renderInstances[i];

                MeshRenderData.WorldMatrix = instance.WorldMatrix;
                Matrix4x4.Invert(instance.WorldMatrix, out MeshRenderData.invWorldMatrix);

                MeshDataBuffer.SetData(ref MeshRenderData);

                BasicForward.Apply();

                IInputLayout inputLayout = Renderer.GetInputLayout(instance.Mesh, BasicForward.BaseMaterial.Shader);

                inputLayout.Bind();

                instance.Mesh.VertexBuffer.Bind();

                Renderer.graphicsDevice.Draw(instance.Mesh.VertexBuffer.VertexCount, 0);

            }
        }

        public void Resize(int width, int height)
        {
        }
    }
}
