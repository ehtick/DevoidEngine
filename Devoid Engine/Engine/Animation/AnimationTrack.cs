namespace DevoidEngine.Engine.Animation
{
    public class AnimationTrack<T>
    {
        public List<Keyframe<T>> Keyframes = new();
        private readonly Func<T, T, float, T> _interpolation;

        public AnimationTrack(Func<T, T, float, T> interp)
        {
            _interpolation = interp;
        }


        // Note: Damn [^1] i never knew this existed, this is extremely cool
        // list[list.count - 1] -> ^1, very cool
        public T Evaluate(float time)
        {
            if (Keyframes.Count == 0)
                return default;

            if (time <= Keyframes[0].Time)
                return Keyframes[0].Value;

            if (time >= Keyframes[^1].Time)
                return Keyframes[^1].Value;

            for (int i = 0; i < Keyframes.Count - 1; i++)
            {
                var a = Keyframes[i];
                var b = Keyframes[i + 1];

                if (time >= a.Time && time <= b.Time)
                {
                    float t = (time - a.Time) / (b.Time - a.Time);
                    return _interpolation(a.Value, b.Value, t);
                }
            }

            return Keyframes[^1].Value;
        }
    }
}