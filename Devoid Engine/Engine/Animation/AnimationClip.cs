namespace DevoidEngine.Engine.Animation
{
    public class AnimationClip
    {
        public float Length;
        public float TicksPerSecond;

        public List<TransformBinding> Bindings = new();
    }
}
