//using DevoidEngine.Engine.Core;
//using DevoidEngine.Engine.Utilities;
//using DevoidGPU;

//namespace DevoidEngine.Engine.Rendering
//{
//    public static class ClusteredRendererHelper
//    {
//        static Mesh cubeMesh;
//        static MaterialInstance BasicForward;

//        private static void SetupMaterial()
//        {
//            MaterialNew_C pbrMaterial = new MaterialNew_C();
//            pbrMaterial.Shader = new Shader("Engine/Content/Shaders/Clustering/clusterVisualize");


//            pbrMaterial.MaterialLayout = new MaterialLayout()
//            {
//                bufferSize = 32,
//                Properties =
//                {
//                    new ShaderPropertyInfo() {Name = "cubeID", Offset = 0, Type = ShaderPropertyType.Int },
//                }
//            };

//            BasicForward = new MaterialInstance(MaterialManager.RegisterMaterial(pbrMaterial));
//        }

//        public static void Initialize()
//        {
//            cubeMesh = new Mesh();
//            cubeMesh.SetVertices(Primitives.GetCubeVertex());

//            SetupMaterial();
//        }

//        public static void VisualizeClusters(BufferObject<Cluster> clusterBuffer, UniformBuffer cameraBuffer)
//        {
//            Renderer.graphicsDevice.UnbindAllShaderResources();

//            IInputLayout LAYOUT = Renderer.GetInputLayout(cubeMesh, BasicForward.BaseMaterial.Shader);
//            LAYOUT.Bind();

//            cubeMesh.VertexBuffer.Bind();

//            clusterBuffer.Bind(1, ShaderStage.Vertex);
//            cameraBuffer.Bind(0, ShaderStage.Vertex);


//            for (int i = 0; i < 3456; i++)
//            {
//                BasicForward.Set("cubeID", i);
//                BasicForward.Apply();

//                Renderer.graphicsDevice.Draw(cubeMesh.VertexBuffer.VertexCount, 0);
//            }
//        }

//    }
//}
