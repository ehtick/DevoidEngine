using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
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
    public static class EnginePipeline
    {
        static List<CameraRenderContext> FrameState = new List<CameraRenderContext>();

        public static void ExecuteUpdateThread(float deltaTime)
        {
            FrameState.Clear();
            if (!SceneManager.IsSceneLoaded()) return;

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

                FrameState.Add(ctx);
            }
        }

        public static void ExecuteRenderThread(float deltaTime, float alpha)
        {

            RenderThread.MainThreadStarted = true;
            RenderThread.Execute();


            for (int i = 0; i < FrameState.Count; i++)
            {
                CameraRenderContext ctx = FrameState[i];
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

            int count = FrameState.Count;

            for (int i = 0; i < count; i++)
            {
                Debug.Assert(i < FrameState.Count);
                var ctx = FrameState[i];
                RenderBase.Render(ctx);
            }

            RenderThread.ExecuteFrameEnd();
        }

    }
}
