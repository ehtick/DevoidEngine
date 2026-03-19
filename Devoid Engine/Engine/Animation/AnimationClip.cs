using System.Numerics;

namespace DevoidEngine.Engine.Animation
{
    public class AnimationClip
    {
        public float Length;
        public float TicksPerSecond = 1f;

        public List<AnimationChannel<Vector3>> Vec3Channels = new();
        public List<AnimationChannel<Quaternion>> QuatChannels = new();
        public List<AnimationChannel<float>> FloatChannels = new();
    }
}