namespace DevoidEngine.Engine.Components
{
    public class PortalComponent : Component
    {
        public override string Type => nameof(PortalComponent);

        public override void OnStart()
        {
            gameObject.Scene.addGameObject("Camera1");
        }
    }
}
