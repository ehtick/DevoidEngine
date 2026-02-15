using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;

namespace DevoidEngine.Engine.Core
{


    public class CameraRenderContext
    {
        public Camera camera;

        public List<RenderItem> renderItems3D = new();
        public List<RenderItem> renderItems2D = new();
        public List<RenderItem> renderItemsUI = new();

        public void Clear() { renderItems3D.Clear(); renderItems2D.Clear(); renderItemsUI.Clear(); }
    }

    public static class FramePipeline
    {
        static Pool<CameraRenderContext> CameraContextPool = new Pool<CameraRenderContext>();

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
                ctx.camera = cameraComponent.Camera;

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

            // Frame level shader bindings go here

            for (int i = 0; i < cameraContextList.Count; i++)
            {
                CameraRenderContext ctx = cameraContextList[i];
                RenderBase.Render(ctx);
            }

            for (int i = 0; i < cameraContextList.Count; i++)
            {
                var ctx = cameraContextList[i];
                ctx.Clear();
                CameraContextPool.Return(ref ctx);
            }

            cameraContextList.Clear();

        }


    }
}
