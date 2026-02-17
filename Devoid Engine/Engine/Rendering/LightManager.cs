//using DevoidEngine.Engine.Utilities;
//using DevoidGPU;
//using System.Numerics;
//using System.Runtime.InteropServices;

//namespace DevoidEngine.Engine.Rendering
//{
//    public static class LightManager
//    {
//        static List<PointLight> pointLights;
//        static List<int> emptyPointLights;

//        static List<SpotLight> spotLights;
//        static List<int> emptySpotLights;

//        static DirectionalLight directionalLight;

//        public static BufferObject<GPUPointLight> pointLightBuffer;
//        public static BufferObject<GPUSpotLight> spotLightBuffer;
//        public static BufferObject<GPUDirectionalLight> dirLightBuffer;

//        public static UniformBuffer countsBuffer;

//        public static void Initialize()
//        {
//            pointLights = new List<PointLight>();
//            emptyPointLights = new List<int>();

//            spotLights = new List<SpotLight>();
//            emptySpotLights = new List<int>();

//            directionalLight = new DirectionalLight();

//            pointLightBuffer = new BufferObject<GPUPointLight>(RenderGraph.MAX_POINT_LIGHTS, BufferUsage.Dynamic, false);
//            spotLightBuffer = new BufferObject<GPUSpotLight>(RenderGraph.MAX_SPOT_LIGHTS, BufferUsage.Dynamic, false);
//            dirLightBuffer = new BufferObject<GPUDirectionalLight>(RenderGraph.MAX_DIR_LIGHTS, BufferUsage.Dynamic, false);

//            countsBuffer = new UniformBuffer(Marshal.SizeOf<LightCounts>(), BufferUsage.Dynamic);
//        }

//        public static void Update()
//        {
//            // Update counts every frame
//            LightCounts counts = new LightCounts
//            {
//                numPointLights = (uint)pointLights.Count,
//                numSpotLights = (uint)spotLights.Count,
//                numDirectionalLights = (uint)(directionalLight.enabled ? 1 : 0),
//                _padding = 0
//            };
//            countsBuffer.SetData(counts);

//            for (int i = 0; i < pointLights.Count; i++)
//            {
//                if (pointLights[i].needsUpdate)
//                {
//                    pointLightBuffer.SetData(new[] { pointLights[i].internalLight }, 1, i);
//                    pointLights[i].needsUpdate = false;
//                }
//            }
//        }

//        public static void Bind()
//        {
//            pointLightBuffer.Bind((int)RenderGraph.SSBOBindIndex.PointLightBuffer, ShaderStage.Compute);
//            spotLightBuffer.Bind((int)RenderGraph.SSBOBindIndex.SpotLightBuffer, ShaderStage.Compute);
//            dirLightBuffer.Bind((int)RenderGraph.SSBOBindIndex.DirLightBuffer, ShaderStage.Compute);

//            countsBuffer.Bind(0, ShaderStage.Compute); // bound as constant buffer
//        }

//        // ================= Point Lights ==================
//        public static unsafe PointLight AddPointLight(Vector3 position, Vector3 color, float intensity, float radius)
//        {
//            if (pointLights.Count >= RenderGraph.MAX_POINT_LIGHTS)
//                throw new Exception("Max point lights reached.");

//            GPUPointLight gpu = new GPUPointLight
//            {
//                position = new Vector4(position, 1),
//                color = new Vector4(color, intensity),
//                range = new Vector4(radius, 0, 0.7f, 1.8f)
//            };

//            PointLight light = new PointLight { internalLight = gpu, enabled = true };

//            int index;
//            if (emptyPointLights.Count > 0)
//            {
//                index = emptyPointLights[0];
//                emptyPointLights.RemoveAt(0);
//                pointLights[index] = light;
//            }
//            else
//            {
//                index = pointLights.Count;
//                pointLights.Add(light);
//            }

//            pointLightBuffer.SetData(new[] { gpu }, 1, index);
//            return light;
//        }

//        public static unsafe void RemovePointLight(PointLight light)
//        {
//            int index = pointLights.IndexOf(light);
//            if (index < 0) return;

//            light.enabled = false;
//            GPUPointLight disabled = new GPUPointLight(); // zero it out
//            pointLightBuffer.SetData(new[] { disabled }, 1, index);
//            emptyPointLights.Add(index);
//        }

//        // ================= Spot Lights ==================
//        public static unsafe SpotLight AddSpotLight(Vector3 position, Vector3 color, Vector3 direction, float intensity, float radius, float innerCutoff, float outerCutoff)
//        {
//            if (spotLights.Count >= RenderGraph.MAX_SPOT_LIGHTS)
//                throw new Exception("Max spot lights reached.");

//            GPUSpotLight gpu = new GPUSpotLight
//            {
//                position = new Vector4(position, 1),
//                color = new Vector4(color, intensity),
//                direction = new Vector4(direction, radius),
//                innerCutoff = innerCutoff,
//                outerCutoff = outerCutoff
//            };

//            SpotLight light = new SpotLight { internalLight = gpu, enabled = true };

//            int index;
//            if (emptySpotLights.Count > 0)
//            {
//                index = emptySpotLights[0];
//                emptySpotLights.RemoveAt(0);
//                spotLights[index] = light;
//            }
//            else
//            {
//                index = spotLights.Count;
//                spotLights.Add(light);
//            }

//            spotLightBuffer.SetData(new[] { gpu }, 1, index);
//            return light;
//        }

//        public static unsafe void RemoveSpotLight(SpotLight light)
//        {
//            int index = spotLights.IndexOf(light);
//            if (index < 0) return;

//            light.enabled = false;
//            GPUSpotLight disabled = new GPUSpotLight();
//            spotLightBuffer.SetData(new[] { disabled }, 1, index);
//            emptySpotLights.Add(index);
//        }

//        // ================= Directional Light ==================
//        public static DirectionalLight AddDirectionalLight(Vector3 direction, Vector3 color, float intensity)
//        {
//            GPUDirectionalLight gpu = new GPUDirectionalLight
//            {
//                Direction = new Vector4(direction, 1),
//                Color = new Vector4(color, intensity)
//            };

//            directionalLight = new DirectionalLight { internalLight = gpu, enabled = true };

//            dirLightBuffer.SetData(new[] { gpu }, 1, 0);
//            return directionalLight;
//        }

//        public static unsafe void RemoveDirectionalLight()
//        {
//            directionalLight.enabled = false;
//            GPUDirectionalLight disabled = new GPUDirectionalLight();
//            dirLightBuffer.SetData(new[] { disabled }, 1, 0);
//        }
//    }

//    [StructLayout(LayoutKind.Sequential)]
//    public struct LightCounts
//    {
//        public uint numPointLights;
//        public uint numSpotLights;
//        public uint numDirectionalLights;
//        public uint _padding;
//    }
//}
