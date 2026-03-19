using System.Numerics;

namespace DevoidEngine.Engine.Animation
{
    public class TransformBinding
    {
        public string NodeName;

        public AnimationTrack<Vector3> Position;
        public AnimationTrack<Quaternion> Rotation;
        public AnimationTrack<Vector3> Scale;
    }
}
