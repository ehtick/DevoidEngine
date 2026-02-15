namespace DevoidEngine.Engine.Core
{
    public static class RenderPipeline
    {
        public static event Action OnBeginCameraRender;
        public static event Action OnEndCameraRender;

        public static void BeginCameraRender()
        {
            OnBeginCameraRender?.Invoke();
        }

        public static void EndCameraRender()
        {
            OnEndCameraRender?.Invoke();
        }

    }
}
