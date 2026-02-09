using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.UI;
using DevoidEngine.Engine.Utilities;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DevoidEngine.Engine.Rendering
{
    struct UIRenderData
    {
        public Matrix4x4 model;
        public Vector4 id;
    }

    public static class UIRenderer
    {
        static Shader basicShader = new Shader("Engine/Content/Shaders/UI/basic");
        static Shader basicTextShader = new Shader("Engine/Content/Shaders/UI/basic.vert.hlsl", "Engine/Content/Shaders/UI/sdf_text.frag.hlsl");

        public static Framebuffer RenderOutput;
        public static CameraData ScreenData;
        static UIRenderData UIRenderData;

        static IUniformBuffer ScreendataBuffer;
        static IUniformBuffer UIRenderBuffer;

        static Mesh Quad;

        public static void Initialize(int width, int height)
        {
            Console.WriteLine("Initializing UI Renderer at " + width + " " + height);

            RenderOutput = new();
            RenderOutput.AttachRenderTexture(new Texture2D(new DevoidGPU.Tex2DDescription()
            {
                Width = width,
                Height = height,
                Format = DevoidGPU.TextureFormat.RGBA16_Float,
                GenerateMipmaps = false,
                IsDepthStencil = false,
                IsRenderTarget = true,
                IsMutable = false,
            }));

            RenderOutput.AttachDepthTexture(new Texture2D(new DevoidGPU.Tex2DDescription()
            {
                Width = width,
                Height = height,
                Format = DevoidGPU.TextureFormat.Depth24_Stencil8,
                GenerateMipmaps = false,
                IsDepthStencil = true,
                IsRenderTarget = false,
                IsMutable = false,
            }));

            Matrix4x4 ortho = Matrix4x4.CreateOrthographicOffCenter(
                0f, Screen.Size.X,
                0f, Screen.Size.Y,
                -1f,
                1f
            );


            ScreenData = new CameraData()
            {
                FarClip = 1f,
                NearClip = -1f,
                Projection = ortho,
                Position = new Vector3( 0f, 0f, 0f ),
                ScreenSize = Screen.Size,
                View = Matrix4x4.Identity
            };

            UIRenderData = new UIRenderData()
            {
                model = Matrix4x4.Identity,
            };

            Matrix4x4.Invert(ortho, out ScreenData.InverseProjection);

            ScreendataBuffer = Renderer.graphicsDevice.BufferFactory.CreateUniformBuffer(Marshal.SizeOf<CameraData>(), BufferUsage.Dynamic);
            UIRenderBuffer = Renderer.graphicsDevice.BufferFactory.CreateUniformBuffer(Marshal.SizeOf<UIRenderData>(), BufferUsage.Dynamic);

            Quad = new Mesh();
            Quad.SetVertices(Primitives.GetQuadVertex());

            ScreendataBuffer.SetData(ref ScreenData);

        }

        public static void BeginRender()
        {
            RenderOutput.Bind();
            RenderOutput.Clear();

            Renderer.graphicsDevice.SetRasterizerState(CullMode.None);
            Renderer.graphicsDevice.SetPrimitiveType(PrimitiveType.Triangles);
            Renderer.graphicsDevice.SetViewport(0, 0, Renderer.Width, Renderer.Height);

            UIRenderBuffer.Bind(1, ShaderStage.Vertex | ShaderStage.Fragment);
            ScreendataBuffer.Bind(0, ShaderStage.Vertex);

        }
        public static void DrawRect(UITransform transform, int count)
        {

            UIRenderData = new UIRenderData()
            {
                model =
                    Matrix4x4.CreateScale(transform.size.X, transform.size.Y, 1.0f) *
                    Matrix4x4.CreateTranslation(transform.position.X, transform.position.Y, 0.0f),
            };
            UIRenderData.id.X = count;
            UIRenderData.id.Y = 0;
            UIRenderBuffer.SetData(ref UIRenderData);

            IInputLayout layout = Renderer3D.GetInputLayout(Quad, basicShader);

            layout.Bind();
            Quad.VertexBuffer.Bind();

            basicShader.Use();
            Renderer.graphicsDevice.Draw(Quad.VertexBuffer.VertexCount, 0);
        }

        public static void DrawTexRect(UITransform transform, Texture2D tex, int count)
        {

            UIRenderData = new UIRenderData()
            {
                model =
                    Matrix4x4.CreateScale(transform.size.X, transform.size.Y, 1.0f) *
                    Matrix4x4.CreateTranslation(transform.position.X, transform.position.Y, 0.0f),
            };
            UIRenderData.id.X = count;
            UIRenderData.id.Y = 1;
            UIRenderBuffer.SetData(ref UIRenderData);

            IInputLayout layout = Renderer3D.GetInputLayout(Quad, basicShader);

            layout.Bind();
            Quad.VertexBuffer.Bind();

            basicShader.Use();

            tex.Bind(0);
            tex.BindSampler(0);

            Renderer.graphicsDevice.Draw(Quad.VertexBuffer.VertexCount, 0);
        }

        public static void DrawText(UITransform transform, Mesh mesh, Texture2D atlas)
        {
            UIRenderData = new UIRenderData()
            {
                model =
                    Matrix4x4.CreateScale(transform.size.X, transform.size.Y, 1.0f) *
                    Matrix4x4.CreateTranslation(transform.position.X, transform.position.Y, 0.0f),
            };
            UIRenderBuffer.SetData(ref UIRenderData);

            IInputLayout layout = Renderer3D.GetInputLayout(mesh, basicTextShader);

            layout.Bind();
            mesh.VertexBuffer.Bind();
            mesh.IndexBuffer.Bind();

            basicTextShader.Use();

            atlas.Bind(0);
            atlas.BindSampler(0);

            Renderer.graphicsDevice.DrawIndexed(mesh.IndexBuffer.IndexCount, 0, 0);
        }

        public static void EndRender()
        {
        }

        public static void Resize(int width, int height)
        {
            Matrix4x4 ortho = Matrix4x4.CreateOrthographicOffCenter(
                0f, width,
                0f, height,
                -1f,
                1f
            );

            ScreenData = new CameraData()
            {
                FarClip = 1f,
                NearClip = -1f,
                Projection = ortho,
                Position = new Vector3(0f, 0f, 0f),
                ScreenSize = new Vector2(Renderer.Width, Renderer.Height),
                View = Matrix4x4.Identity
            };

            Matrix4x4.Invert(ortho, out ScreenData.InverseProjection);
            ScreendataBuffer.SetData(ref ScreenData);

            RenderOutput.Resize(width, height);
        }
    }
}
