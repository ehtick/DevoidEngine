using DevoidEngine.Engine.Components;
using System.Numerics;
using System.Threading.Channels;

namespace DevoidEngine.Engine.Animation
{
    public class AnimationPlayer
    {
        public AnimationClip Clip;
        public float Time;
        public bool Loop = true;

        private readonly List<ChannelBinding<Vector3>> _vec3 = new();
        private readonly List<ChannelBinding<Quaternion>> _quat = new();
        private readonly List<ChannelBinding<float>> _float = new();

        // -------------------------
        // BIND (resolve once)
        // -------------------------
        public void Bind(
            Func<string, Action<Vector3>> vec3Resolver,
            Func<string, Action<Quaternion>> quatResolver,
            Func<string, Action<float>> floatResolver)
        {
            _vec3.Clear();
            _quat.Clear();
            _float.Clear();

            if (Clip == null)
                return;


            // Vector3 channels
            if (Clip.Vec3Channels != null)
            {
                foreach (var ch in Clip.Vec3Channels)
                {
                    var setter = vec3Resolver?.Invoke(ch.Path);
                    if (setter == null)
                        continue;

                    _vec3.Add(new ChannelBinding<Vector3>
                    {
                        Track = ch.Track,
                        Setter = setter
                    });
                }
            }

            // Quaternion channels
            if (Clip.QuatChannels != null)
            {
                foreach (var ch in Clip.QuatChannels)
                {
                    var setter = quatResolver?.Invoke(ch.Path);
                    if (setter == null)
                        continue;

                    _quat.Add(new ChannelBinding<Quaternion>
                    {
                        Track = ch.Track,
                        Setter = setter
                    });
                }
            }

            // Float channels
            if (Clip.FloatChannels != null)
            {
                foreach (var ch in Clip.FloatChannels)
                {
                    var setter = floatResolver?.Invoke(ch.Path);
                    if (setter == null)
                        continue;

                    _float.Add(new ChannelBinding<float>
                    {
                        Track = ch.Track,
                        Setter = setter
                    });
                }
            }

        }

        // -------------------------
        // UPDATE (hot path)
        // -------------------------
        public float FrameRate = 165f;
        public bool UseStepped = true;

        public void Update(float deltaTime)
        {
            if (Clip == null)
                return;

            Time += deltaTime;

            if (Loop)
            {
                if (Clip.Length > 0f)
                    Time %= Clip.Length;
            }
            else
            {
                if (Time > Clip.Length)
                    Time = Clip.Length;
            }

            float evalTime = Time;

            if (UseStepped)
            {
                evalTime = MathF.Floor(Time * FrameRate) / FrameRate;
            }

            // Evaluate Vector3
            for (int i = 0; i < _vec3.Count; i++)
            {
                var ch = _vec3[i];
                ch.Setter(ch.Track.Evaluate(evalTime));
            }

            // Quaternion
            for (int i = 0; i < _quat.Count; i++)
            {
                var ch = _quat[i];
                ch.Setter(ch.Track.Evaluate(evalTime));
            }

            // Float
            for (int i = 0; i < _float.Count; i++)
            {
                var ch = _float[i];
                ch.Setter(ch.Track.Evaluate(evalTime));
            }
        }

        // -------------------------
        // CONTROL
        // -------------------------
        public void Play(AnimationClip clip, bool loop = true)
        {
            Clip = clip;
            Loop = loop;
            Time = 0f;
        }

        public void Stop()
        {
            Time = 0f;
        }

        public void SetTime(float time)
        {
            Time = time;
        }
    }
}