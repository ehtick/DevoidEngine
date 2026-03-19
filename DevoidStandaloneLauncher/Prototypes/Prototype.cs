namespace DevoidStandaloneLauncher.Prototypes
{
    internal class Prototype
    {
        public PrototypeLoader loader;
        public virtual void OnInit() { }
        public virtual void OnUpdate(float delta) { }
        public virtual void OnRender(float delta) { }
        public virtual void OnLateRender() { }
        public virtual void Resize(int width, int height) { }
    }
}