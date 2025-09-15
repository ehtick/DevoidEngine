using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public static class EnginePipeline
    {
        public static Pool<DrawMeshIndexed> DrawMeshIndexedPool;
        public static Pool<SetViewInfoCommand3D> SetViewInfoPool;

        public static Dictionary<Camera, List<RenderInstance>> VisibleInstances;


        public static List<IRenderCommand> RenderCommands;
        public static DoubleBuffer<List<IRenderCommand>> InstanceBuffers;

        static EnginePipeline()
        {
            DrawMeshIndexedPool = new Pool<DrawMeshIndexed>();
            SetViewInfoPool = new Pool<SetViewInfoCommand3D>();

            InstanceBuffers = new DoubleBuffer<List<IRenderCommand>>(new List<IRenderCommand>(), new List<IRenderCommand>());

            RenderCommands = new List<IRenderCommand>();
            VisibleInstances = new Dictionary<Camera, List<RenderInstance>>();
        }

        public static void ExecuteUpdateThread(float deltaTime)
        {
            RenderCommands.Clear();

            SceneManager.Update(deltaTime);

            List<RenderInstance> renderInstances = SceneRenderSystem.RenderInstances;


            SceneManager.Render(deltaTime);

            // Frustum culling per camera
            PerformFrustumCull(renderInstances);
            
            foreach (var cameraRenderInstances in VisibleInstances)
            {
                Camera camera = cameraRenderInstances.Key;
                List<RenderInstance> visibleInstances = cameraRenderInstances.Value;


                Graphics.SetCamera(camera);

                RenderPipeline.BeginCameraRender();

                foreach (var renderInstance in visibleInstances)
                {
                    Graphics.DrawMeshIndexed(renderInstance.Mesh, renderInstance.MaterialHandle, renderInstance.WorldMatrix);
                }

                RenderPipeline.EndCameraRender();
            }

            RenderCommands = InstanceBuffers.UpdateSwap(RenderCommands);
        }

        public static void ExecuteRenderThread(float deltaTime)
        {
            List<IRenderCommand> commandList = InstanceBuffers.Front;

            RenderAPI.ProcessCommands(commandList);

            commandList.Clear();

            InstanceBuffers.RenderSwap();
        }

        static void PerformFrustumCull(List<RenderInstance> renderInstances)
        {
            VisibleInstances.Clear();
            foreach (var camera in SceneManager.MainScene.Cameras)
            {
                var cam = camera.Camera;
                var list = new List<RenderInstance>();
                for (int i = 0; i < renderInstances.Count; i++)
                {
                    var instance = renderInstances[i];
                    if (cam.Frustum.Intersects(instance.Mesh.LocalBounds.Transform(instance.WorldMatrix)))
                        list.Add(instance);
                }
                if (list.Count > 0)
                {
                    VisibleInstances[cam] = list;
                }

            }
        }



    }
}
