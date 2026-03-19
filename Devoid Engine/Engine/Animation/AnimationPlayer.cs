using DevoidEngine.Engine.Components;

namespace DevoidEngine.Engine.Animation
{
    public class AnimationPlayer
    {
        public AnimationClip Clip;
        public float Time;
        public bool Loop = true;

        private Dictionary<string, Transform> _map;

        public void Initialize(Dictionary<string, Transform> nodeMap)
        {
            _map = nodeMap;
        }

        public void Update(float dt)
        {
            if (Clip == null) return;

            Time += dt * Clip.TicksPerSecond;

            if (Loop)
                Time %= Clip.Length;

            foreach (var b in Clip.Bindings)
            {
                var t = _map[b.NodeName];

                if (b.Position != null)
                    t.LocalPosition = b.Position.Evaluate(Time);

                if (b.Rotation != null)
                    t.LocalRotation = b.Rotation.Evaluate(Time);

                if (b.Scale != null)
                    t.LocalScale = b.Scale.Evaluate(Time);
            }
        }
    }
}
