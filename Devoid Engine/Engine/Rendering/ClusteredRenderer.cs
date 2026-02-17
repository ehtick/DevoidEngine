//using DevoidEngine.Engine.Core;
//using DevoidEngine.Engine.Utilities;
//using System.Numerics;
//using System.Runtime.InteropServices;

//namespace DevoidEngine.Engine.Rendering
//{
//    [StructLayout(LayoutKind.Sequential, Pack = 16)]
//    public unsafe struct Cluster
//    {
//        public Vector4 minPoint;
//        public Vector4 maxPoint;

//        public uint count;
//        public fixed uint lightIndices[100];
//    }

//    public class ClusteredRenderer : IRenderPipeline
//    {
//        const uint MAX_CLUSTER_LIGHTS = 100;
//        struct RendererData
//        {
//            public Vector4 groupSize;
//        }


//        Vector4 ClusterGroupSize = new Vector4(16, 9, 24, 1);
//        // 16 clusters on width, 9 clusters on height, 24 on depth.


//        RendererData data;

//        Cluster[] clusters;
//        int numClusters;

//        // Shaders + Compute

//        ComputeShader ClusterFormationShader = new ComputeShader("Engine/Content/Shaders/Clustering/formClusters.comp.hlsl");

//        // Buffers

//        UniformBuffer cameraDataBuffer;
//        BufferObject<Cluster> clusterBuffer;
//        UniformBuffer rendererDataBuffer;

//        public void Initialize(int width, int height)
//        {
//            Console.WriteLine("Initializing Clustered Renderer");

//            data = new RendererData() { groupSize = ClusterGroupSize };
//            numClusters = (int)(ClusterGroupSize.X * ClusterGroupSize.Y * ClusterGroupSize.Z);

//            clusters = new Cluster[numClusters];


//            clusterBuffer = new BufferObject<Cluster>(clusters.Length, DevoidGPU.BufferUsage.Default, true);

//            cameraDataBuffer = new UniformBuffer(Marshal.SizeOf<CameraData>(), DevoidGPU.BufferUsage.Default);

//            rendererDataBuffer = new UniformBuffer(Marshal.SizeOf<RendererData>(), DevoidGPU.BufferUsage.Default);
//            rendererDataBuffer.SetData(data);

//            //ClusteredRendererHelper.Initialize();
//        }

//        void GenerateClusters()
//        {
//            cameraDataBuffer.Bind(0, DevoidGPU.ShaderStage.Compute);
//            rendererDataBuffer.Bind(1, DevoidGPU.ShaderStage.Compute);

//            clusterBuffer.BindMutable(1);



//            ClusterFormationShader.Use();
//            ClusterFormationShader.Dispatch((int)ClusterGroupSize.X, (int)ClusterGroupSize.Y, (int)ClusterGroupSize.Z);
//            ClusterFormationShader.Wait();
//        }

//        void AssignClusterLights()
//        {

//        }

//        public MaterialInstance GetDefaultMaterial()
//        {
//            return null;
//        }

//        public Framebuffer GetOutputFrameBuffer()
//        {
//            return null;
//        }

//        public void BeginRender(Camera camera)
//        {
//            CameraData cameraData = camera.GetCameraData();
//            cameraDataBuffer.SetData(cameraData);

//            GenerateClusters();
//            AssignClusterLights();
//            //ClusteredRendererHelper.VisualizeClusters(clusterBuffer, cameraDataBuffer);
//        }

//        public void EndRender()
//        {
//        }

//        public void Render(List<RenderInstance> renderInstances)
//        {
//        }

//        public void Resize(int width, int height)
//        {
//        }
//    }
//}
