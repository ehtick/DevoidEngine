using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Rendering;
using DevoidEngine.Engine.Utilities;
using System.Diagnostics;
using System.Numerics;

namespace DevoidEngine.Engine.Core
{
    enum RenderResourceType
    {
        Screen,
        SceneDepth,
        GBufferNormal,
        GBufferPosition,
    }

    public class CameraRenderContext
    {
        public CameraData cameraData;
        public Framebuffer cameraTargetSurface;

        public List<RenderItem> renderItems3D = new();
        public List<RenderItem> renderItems2D = new();
        public List<RenderItem> renderItemsUI = new();

        public List<GPUPointLight> pointLights = new();
        public List<GPUSpotLight> spotLights = new();
        public List<GPUDirectionalLight> directionalLights = new();

        public void Clear()
        {
            renderItems3D.Clear(); renderItems2D.Clear(); renderItemsUI.Clear(); pointLights.Clear();
            spotLights.Clear(); directionalLights.Clear();
        }
    }

    public static class FramePipeline
    {
        //static Pool<CameraRenderContext> CameraContextPool = new Pool<CameraRenderContext>();

        static AsyncTripleBuffer<List<CameraRenderContext>> SwapBuffer;

        static FramePipeline()
        {
            SwapBuffer = new AsyncTripleBuffer<List<CameraRenderContext>>(new List<CameraRenderContext>(), new List<CameraRenderContext>(), new List<CameraRenderContext>());
        }

        public static void Reset()
        {
            //SwapBuffer.Front.Clear();
            //SwapBuffer.Back.Clear();
        }

        public static void ExecuteUpdateThread(float deltaTime)
        {
            if (!SceneManager.IsSceneLoaded()) return;

            var backList = SwapBuffer.AcquireBackBuffer(out int backIndex);
            backList.Clear();
            Scene scene = SceneManager.CurrentScene;
            var cameras = scene.GetComponentsOfType<CameraComponent3D>();


            for (int i = 0; i < cameras.Count; i++)
            {
                var cameraComponent = cameras[i];

                CameraRenderContext ctx = new CameraRenderContext();
                ctx.cameraData = cameraComponent.Camera.GetCameraData();
                ctx.cameraTargetSurface = cameraComponent.Camera.RenderTarget;

                foreach (var renderable in scene.Renderables)
                {
                    renderable.Collect(cameraComponent, ctx);
                }

                backList.Add(ctx);
            }

            SwapBuffer.Publish(backIndex);
        }

        public static void ExecuteRenderThread(float deltaTime, float alpha)
        {

            RenderThread.MainThreadStarted = true;
            RenderThread.Execute();

            var cameraContextList = SwapBuffer.GetFrontBuffer();


            for (int i = 0; i <  cameraContextList.Count; i++)
            {
                CameraRenderContext ctx = cameraContextList[i];
                for (int j = 0; j < ctx.renderItems3D.Count; j++)
                {
                    var renderItem = ctx.renderItems3D[j];

                    if (renderItem.useInterpolation)
                    {
                        var s = renderItem.TransformData;

                        Vector3 pos = Vector3.Lerp(s.PrevPos, s.CurrPos, alpha);
                        Quaternion rot = Quaternion.Slerp(s.PrevRot, s.CurrRot, alpha);
                        Vector3 scale = Vector3.Lerp(s.PrevScale, s.CurrScale, alpha);

                        renderItem.Model =
                            Matrix4x4.CreateScale(scale) *
                            Matrix4x4.CreateFromQuaternion(rot) *
                            Matrix4x4.CreateTranslation(pos);

                        // 🔴 THIS LINE IS MISSING
                        ctx.renderItems3D[j] = renderItem;
                    }
                }
            }

            // Frame level shader bindings go here

            int count = cameraContextList.Count;

            for (int i = 0; i < count; i++)
            {
                Debug.Assert(i < cameraContextList.Count);
                var ctx = cameraContextList[i];
                RenderBase.Render(ctx);
            }

            RenderThread.ExecuteFrameEnd();

            SwapBuffer.ReleasePreviousFront();
        }

        public static void Shutdown()
        {
            SwapBuffer.ReleasePreviousFront();
        }


    }
}