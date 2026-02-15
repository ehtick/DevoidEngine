using DevoidEngine.Engine.Components;

namespace DevoidEngine.Engine.Core
{
    public interface IRenderComponent
    {
        public void Collect(CameraComponent3D camera, CameraRenderContext viewData);
    }
}
