using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{


    public class CameraRenderContext
    {
        public List<RenderItem> renderItems3D = new();
        public List<RenderItem> renderItems2D = new();
        public List<RenderItem> renderItemsUI = new();

        public void Clear() { renderItems3D.Clear(); renderItems2D.Clear(); renderItemsUI.Clear(); }
    }

    public static class FramePipeline
    {
        static Pool<CameraRenderContext> CameraContextPool = new Pool<CameraRenderContext>();

        static List<CameraRenderContext> camRenderContexts = new List<CameraRenderContext>();
        static AsyncDoubleBuffer<List<CameraRenderContext>> SwapBuffer;

        static FramePipeline()
        {
            SwapBuffer = new AsyncDoubleBuffer<List<CameraRenderContext>>(new List<CameraRenderContext>(), new List<CameraRenderContext>());
        }

        public static void ExecuteUpdateThread(float deltaTime)
        {
            if (!SceneManager.IsSceneLoaded()) { return; }

            var backList = SwapBuffer.Back;
            backList.Clear();

            Scene scene = SceneManager.MainScene;

            foreach (var cameraComponent in scene.GetComponentsOfType<CameraComponent3D>())
            {

                CameraRenderContext ctx = CameraContextPool.Get(); 
                
                foreach (var renderable in scene.Renderables)
                {
                    renderable.Collect(cameraComponent, ctx);
                }

                backList.Add(ctx);
            }

            SwapBuffer.Publish();
        }

        public static void ExecuteRenderThread(float deltaTime)
        {
            List<CameraRenderContext> cameraContextList = SwapBuffer.Front;

            UIRenderer.BeginRender();

            Console.WriteLine(cameraContextList.Count);


            foreach (var item in cameraContextList)
            {
                foreach (var x in item.renderItemsUI)
                {
                    UIRenderer.DrawRect(x.Model, (int)x.Material.PropertiesVec4Override["Configuration"].X);
                }
            }

            UIRenderer.EndRender();

            for (int i = 0; i < cameraContextList.Count; i++)
            {
                var ctx = cameraContextList[i];
                ctx.Clear();
                CameraContextPool.Return(ref ctx);
            }

            cameraContextList.Clear();

            //SwapBuffer.RenderSwap();
        }


    }
}
