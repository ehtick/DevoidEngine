using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public static class SynchronizationManager
    {
        public static Pool<DrawMeshIndexed> DrawMeshIndexedPool;

        public static List<IRenderCommand> RenderCommands;

        public static Dictionary<Camera, List<RenderInstance>> VisibleInstances;

        static SynchronizationManager()
        {
            RenderCommands = new List<IRenderCommand>();
            VisibleInstances = new Dictionary<Camera, List<RenderInstance>>();
        }

        public static void ExecuteUpdateThread(float deltaTime)
        {
            SceneManager.Update(deltaTime);

            List<RenderInstance> renderInstances = SceneRenderSystem.RenderInstances;

            // Frustum culling per camera
            PerformFrustumCull(renderInstances);



            SceneManager.Render(deltaTime);


        }

        static void PerformFrustumCull(List<RenderInstance> renderInstances)
        {
            VisibleInstances.Clear();
            foreach (var camera in SceneManager.MainScene.Cameras)
            {
                var cam = camera.Camera;
                var list = new List<RenderInstance>();
                foreach (var instance in renderInstances)
                {
                    if (cam.Frustum.Intersects(instance.Mesh.LocalBounds.Transform(instance.WorldMatrix)))
                        list.Add(instance);
                }
                VisibleInstances[cam] = list;
            }
        }

        public static void ExecuteRenderThread(float deltaTime)
        {

        }



    }
}
