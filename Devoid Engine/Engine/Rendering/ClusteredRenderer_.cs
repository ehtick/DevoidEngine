//using DevoidEngine.Engine.Core;
//using DevoidEngine.Engine.Utilities;
//using DevoidGPU;
//using System.Numerics;
//using System.Runtime.InteropServices;

//namespace DevoidEngine.Engine.Rendering
//{
//    public struct ObjectData
//    {
//        public Matrix4x4 Model;
//    }

//    [StructLayout(LayoutKind.Sequential, Pack = 16)]
//    unsafe struct Cluster
//    {
//        public Vector4 minPoint;
//        public Vector4 maxPoint;
//    }

//    [StructLayout(LayoutKind.Sequential, Pack = 4)]
//    unsafe struct LightGrid
//    {
//        public uint offset;
//        public uint count;
//    }

//    [StructLayout(LayoutKind.Sequential, Pack = 4)]
//    struct GlobalIndexCount
//    {
//        public uint globalLightIndexCount;
//    }

//    [StructLayout(LayoutKind.Sequential)]
//    struct ScreenViewData
//    {
//        public Matrix4x4 inverseProjectionMatrix; // 64 bytes
//        public uint tileCountX;
//        public uint tileCountY;
//        public uint sliceCountZ;
//        public uint tilePixelWidth;

//        public uint screenWidth;
//        public uint screenHeight;
//        public uint tilePixelHeight;

//        public float sliceScaling;
//        public float sliceBias;
//        public Vector3 _padding;
//    }



//    [StructLayout(LayoutKind.Sequential, Pack = 16)]
//    struct ClusteredRendererData
//    {
//        public Vector4 groupSize;
//    }

//    public class ClusteredRenderer : IRenderPipeline
//    {
//        ClusteredRendererData clusteredRendererData = new ClusteredRendererData()
//        {
//            groupSize = new Vector4(16, 9, 24, 1)
//        };

//        int numClusters;
//        uint numLightsPerTile = 100;

//        Cluster[] clusters;
//        LightGrid[] lightGrids;

//        uint[] globalLightIndexList;

//        BufferObject<Cluster> clusterBuffer;
//        BufferObject<LightGrid> lightGridBuffer;
//        BufferObject<uint> globalIndexCount;
//        BufferObject<uint> globalLightIndexListSSBO;
//        BufferObject<LightData> lightDataUBO;
//        BufferObject<ScreenViewData> screenViewDataBuffer;

//        UniformBuffer<CameraData> cameraBuffer;
//        UniformBuffer<ClusteredRendererData> clusteredRendererDataBuffer;

//        ComputeShader clusterCompute;
//        ComputeShader clusterLightCull;

//        RenderContext currentRenderContext;
//        List<IRenderPass> RenderPasses;

//        // TEMP //
//        UniformBuffer<ObjectData> objectData;
//        ObjectData objData;
//        IVertexBuffer vertexBuffer;
//        IInputLayout layout;
//        Shader clusteredPBR;
//        PBRMaterial material;
//        // END OF TEMP //


//        public Material GetDefaultMaterial()
//        {
//            throw new NotImplementedException();
//        }

//        public Framebuffer GetOutputFrameBuffer()
//        {
//            throw new NotImplementedException();
//        }

//        public void Initialize(int width, int height)
//        {
//            Console.WriteLine("Initializing Clustered Renderer");

//            RenderPasses = new List<IRenderPass>();

//            clusterCompute = new ComputeShader("Engine/Content/Shaders/Clustering/cluster.comp.hlsl");
//            clusterLightCull = new ComputeShader("Engine/Content/Shaders/Clustering/cluster_light_cull.comp.hlsl");


//            numClusters = (int)(clusteredRendererData.groupSize.X * clusteredRendererData.groupSize.Y * clusteredRendererData.groupSize.Z);

//            clusters = new Cluster[numClusters];
//            lightGrids = new LightGrid[numClusters * 100];
//            globalLightIndexList = new uint[numLightsPerTile * numClusters];


//            clusterBuffer = new BufferObject<Cluster>(clusters.Length, BufferUsage.Default, true);
//            lightGridBuffer = new BufferObject<LightGrid>(lightGrids.Length, BufferUsage.Default, true);
//            globalIndexCount = new BufferObject<uint>(1, BufferUsage.Default, true);
//            globalLightIndexListSSBO = new BufferObject<uint>(globalLightIndexList.Length, BufferUsage.Default, true);

//            screenViewDataBuffer = new BufferObject<ScreenViewData>(1, BufferUsage.Dynamic, false);


//            clusteredRendererDataBuffer = new UniformBuffer<ClusteredRendererData>();
//            clusteredRendererDataBuffer.SetData(ref clusteredRendererData);

//            cameraBuffer = new UniformBuffer<CameraData>();

//            LightManager.Initialize();

//            // TEMP //

//            objData = new ObjectData()
//            {
//                Model = Matrix4x4.CreateTranslation(11, 0, 0) * Matrix4x4.CreateScale(1)
//            };

//            objectData = new UniformBuffer<ObjectData>();
//            objectData.SetData(ref objData);

//            Vertex[] box = Primitives.GetCubeVertex();
//            vertexBuffer = Renderer.graphicsDevice.BufferFactory.CreateVertexBuffer(BufferUsage.Default, Vertex.VertexInfo, box.Length);
//            vertexBuffer.SetData(box);

//            clusteredPBR = new Shader("Engine/Content/Shaders/PBR/clustered_pbr");

//            layout = Renderer.graphicsDevice.CreateInputLayout(Vertex.VertexInfo, clusteredPBR.vShader);


//        }

//        void ComputeClusters()
//        {
//            clusterCompute.Use();

//            clusterBuffer.BindMutable((int)RenderGraph.SSBOBindIndex.ClusterBuffer);
//            screenViewDataBuffer.Bind((int)RenderGraph.SSBOBindIndex.ScreenInfoBuffer, ShaderStage.Compute);

//            cameraBuffer.Bind(0, ShaderStage.Compute);
//            clusteredRendererDataBuffer.Bind(1, ShaderStage.Compute);

//            clusterCompute.Dispatch((int)clusteredRendererData.groupSize.X, (int)clusteredRendererData.groupSize.Y, (int)clusteredRendererData.groupSize.Z);
//        }

//        public void CreateScreenViewBuffer(Camera camera)
//        {
//            uint sizeX = (uint)Math.Ceiling(Renderer.Width / (float)clusteredRendererData.groupSize.X);

//            // This is generated here since the camera does not get assigned at the initialize stage.
//            ScreenViewData screenViewData = new ScreenViewData();
//            screenViewData.screenWidth = (uint)Screen.Size.X;
//            screenViewData.screenHeight = (uint)Screen.Size.Y;
//            screenViewData.sliceScaling = (float)clusteredRendererData.groupSize.Z / (float)Math.Log(camera.FarClip / camera.NearClip, 2);
//            screenViewData.sliceBias = -(clusteredRendererData.groupSize.Z * (float)Math.Log(camera.NearClip, 2) / (float)Math.Log(camera.FarClip / camera.NearClip, 2));
//            screenViewData.tileCountX = (uint)clusteredRendererData.groupSize.X;
//            screenViewData.tileCountY = (uint)clusteredRendererData.groupSize.Y;
//            screenViewData.sliceCountZ = (uint)clusteredRendererData.groupSize.Z;
//            screenViewData.tilePixelWidth = (uint)sizeX;
//            screenViewData.tilePixelHeight = (uint)Math.Ceiling(Renderer.Height / (float)clusteredRendererData.groupSize.Y);
//            Matrix4x4.Invert(camera.GetProjectionMatrix(), out screenViewData.inverseProjectionMatrix);
//            screenViewData.inverseProjectionMatrix = Matrix4x4.Transpose(screenViewData.inverseProjectionMatrix);
//            screenViewDataBuffer.SetData(new ScreenViewData[] { screenViewData }, 1);
//            screenViewDataBuffer.Bind((int)RenderGraph.SSBOBindIndex.ScreenInfoBuffer);
//        }
//        public void CullLights()
//        {
//            lightGridBuffer.UnBind((int)RenderGraph.SSBOBindIndex.LightGridBuffer);
//            globalLightIndexListSSBO.UnBind((int)RenderGraph.SSBOBindIndex.GlobalLightIndexList);

//            clusterLightCull.Use();

//            LightManager.pointLightBuffer.Bind(0, ShaderStage.Compute);
//            LightManager.spotLightBuffer.Bind(1, ShaderStage.Compute);

//            cameraBuffer.Bind(0, ShaderStage.Compute);
//            LightManager.countsBuffer.Bind(1, ShaderStage.Compute);
//            clusteredRendererDataBuffer.Bind(2, ShaderStage.Compute);


//            lightGridBuffer.BindMutable((int)RenderGraph.SSBOBindIndex.LightGridBuffer);
            
//            // Already bound while ComputeCluster();
//            //clusterBuffer.BindMutable((int)RenderGraph.SSBOBindIndex.ClusterBuffer);
//            globalIndexCount.BindMutable((int)RenderGraph.SSBOBindIndex.GlobalLightIndexCount);
//            globalLightIndexListSSBO.BindMutable((int)RenderGraph.SSBOBindIndex.GlobalLightIndexList);

            
//            clusterLightCull.Dispatch(1, 1, 6);
//            clusterLightCull.Wait();

//            lightGridBuffer.UnBindMutable((int)RenderGraph.SSBOBindIndex.LightGridBuffer);
//            globalLightIndexListSSBO.UnBindMutable((int)RenderGraph.SSBOBindIndex.GlobalLightIndexList);
//        }

//        public void BeginRender(Camera camera)
//        {
//            LightManager.Update();

//            currentRenderContext = new RenderContext();
//            currentRenderContext.Camera = camera;
            

//            CameraData cameraInfo = camera.GetCameraData();
//            cameraBuffer.SetData(ref cameraInfo);

//            camera.RenderTarget.Bind();
//            camera.RenderTarget.Clear();
//            Renderer.graphicsDevice.SetViewport(0, 0, Renderer.Width, Renderer.Height);
//            Renderer.graphicsDevice.SetDepthState(DepthTest.LessEqual, true);


//            CreateScreenViewBuffer(camera);
//            ComputeClusters();
//            CullLights();
//        }

//        public void Render(List<RenderInstance> renderInstances)
//        {
//            for (int i = 0; i <  renderInstances.Count; i++)
//            {
//                Mesh mesh = renderInstances[i].Mesh;
//                Matrix4x4 WorldMatrix = renderInstances[i].WorldMatrix;

//                IInputLayout inputLayout = Renderer3D.GetInputLayout(mesh, clusteredPBR);

//                objData = new ObjectData()
//                {
//                    Model = WorldMatrix,
//                };
//                objectData.SetData(ref objData);

//                inputLayout.Bind();
//                mesh.VertexBuffer.Bind();

//                MaterialManager.MaterialA.Apply();

//                LightManager.pointLightBuffer.Bind(0, ShaderStage.Fragment);
//                LightManager.spotLightBuffer.Bind(1, ShaderStage.Fragment);
//                LightManager.countsBuffer.Bind(1, ShaderStage.Fragment);

//                cameraBuffer.Bind(0, ShaderStage.Vertex | ShaderStage.Fragment);
//                objectData.Bind(1, ShaderStage.Vertex);

//                lightGridBuffer.Bind((int)RenderGraph.SSBOBindIndex.LightGridBuffer, ShaderStage.Fragment);
//                globalLightIndexListSSBO.Bind((int)RenderGraph.SSBOBindIndex.GlobalLightIndexList, ShaderStage.Fragment);

//                Renderer.graphicsDevice.Draw(vertexBuffer.VertexCount, 0);
//            }

//        }

//        public void EndRender()
//        {
//            Renderer.graphicsDevice.MainSurface.Bind();
//        }

//        public void Resize(int width, int height)
//        {

//        }
//    }
//}
