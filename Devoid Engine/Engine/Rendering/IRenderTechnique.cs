using DevoidEngine.Engine.Core;

namespace DevoidEngine.Engine.Rendering
{
    public interface IRenderTechnique
    {
        void Initialize(int width, int height);
        void Resize(int width, int height);
        Texture2D Render(CameraRenderContext ctx);
        void Dispose();
    }
}
