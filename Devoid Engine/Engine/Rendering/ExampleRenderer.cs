//using DevoidEngine.Engine.Core;
//using DevoidEngine.Engine.Utilities;
//using DevoidGPU;
//using DevoidGPU.DX11;
//using OpenTK.Graphics.ES11;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Numerics;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace DevoidEngine.Engine.Rendering
//{

//    public static class ExampleRenderer
//    {
//        static Vertex[] Quad;
//        public static IFramebuffer framebuffer;
//        static IVertexBuffer VertexBuffer;
//        static IInputLayout InputLayout;
//        static IUniformBuffer uniformBuffer;
//        static IUniformBuffer uniformBuffer2;
//        static Shader simpleShader;
//        static Camera camera;
//        static ObjectData objData;

//        public static void Initialize()
//        {
//            Quad = Primitives.GetQuadVertex();

//            VertexBuffer = Renderer.graphicsDevice.BufferFactory.CreateVertexBuffer(BufferUsage.Default, Vertex.VertexInfo, 6);
//            VertexBuffer.SetData<Vertex>(Quad);
            
//            simpleShader = new Shader("Engine/Content/Shaders/Testing/basic.vert.hlsl", "Engine/Content/Shaders/Testing/basic.frag.hlsl");


//            InputLayout = Renderer.graphicsDevice.CreateInputLayout(VertexBuffer.Layout, simpleShader.vShader);

//            framebuffer = Renderer.graphicsDevice.BufferFactory.CreateFramebuffer();

//            //ITexture2D texture2D = Renderer.graphicsDevice.TextureFactory.CreateTexture2D(550, 550, TextureFormat.RGBA16_Float, false, true);
//            //framebuffer.AddColorAttachment(texture2D);

//            int width = 550;
//            int height = 550;



//            Camera camera = new Camera();
//            float aspectRatio = width / (float)height;
//            //camera.UpdateView()
//            camera.UpdateProjectionMatrix(aspectRatio);


//            var camData = camera.GetCameraData();
//            objData = new ObjectData()
//            {
//                Model = Matrix4x4.CreateTranslation(new Vector3(0, 0, 11)) * Matrix4x4.CreateScale(1)
//            };

//            uniformBuffer = Renderer.graphicsDevice.BufferFactory.CreateUniformBuffer<CameraData>(BufferUsage.Dynamic);
//            uniformBuffer.SetData(ref camData);

//            uniformBuffer2 = Renderer.graphicsDevice.BufferFactory.CreateUniformBuffer<ObjectData>(BufferUsage.Dynamic);
//            uniformBuffer2.SetData(ref objData);

//        }

//        static int delta = 0;

//        public static void Render()
//        {
//            framebuffer.ClearColor(new System.Numerics.Vector4(0, 0, 0, 1));
//            framebuffer.Bind();
//            Renderer.graphicsDevice.SetViewport(0, 0, 500, 500);
//            Renderer.graphicsDevice.SetDepthState(DepthTest.Disabled, false);
//            Renderer.graphicsDevice.SetBlendState(BlendMode.Additive);

//            InputLayout.Bind();
//            simpleShader.Use();
//            VertexBuffer.Bind();
//            uniformBuffer.Bind(0, ShaderStage.Vertex);
//            uniformBuffer2.Bind(1, ShaderStage.Vertex);
//            (VertexBuffer as DX11VertexBuffer).Draw();


//            Renderer.graphicsDevice.MainSurface.Bind();


//            objData = new ObjectData()
//            {
//                Model = Matrix4x4.CreateTranslation(new Vector3(0, 0, 11 + (float)Math.Sin((Math.PI / 180) * delta++)))
//            };
//            uniformBuffer2.SetData(ref objData);
//        }

//    }
//}
