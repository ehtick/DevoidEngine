using DevoidEngine.Engine.Core;

namespace DevoidEngine.Engine.Components
{
    public class GhostFadeComponent : Component
    {
        public override string Type => nameof(GhostFadeComponent);

        public float Life = 0.2f;
        private float timer;

        public override void OnUpdate(float dt)
        {
            timer += dt;

            if (timer >= Life)
            {
                gameObject.Scene.Destroy(gameObject);
            }
        }
    }
}