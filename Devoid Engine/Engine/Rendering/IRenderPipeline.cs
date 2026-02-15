using DevoidEngine.Engine.Core;

namespace DevoidEngine.Engine.Rendering
{
    public interface IRenderPipeline
    {
        void Initialize(int width, int height);
        void BeginRender(Camera camera);
        void Render(List<RenderInstance> renderInstances);
        void EndRender();

        void Resize(int width, int height);

        Framebuffer GetOutputFrameBuffer();

        MaterialInstance GetDefaultMaterial();
    }
}
