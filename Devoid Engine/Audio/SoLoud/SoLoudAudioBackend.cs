using SoLoud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Audio.SoLoud
{
    internal class SoLoudAudioBackend : IAudioBackend
    {
        internal Dictionary<uint, Wav> _audioObjectMapping;
        internal uint _nextAudioId = 0;

        internal List<AudioPlayObject> _audioPlayObjects;

        internal Soloud Soloud;

        public void Initialize()
        {
            Soloud = new Soloud();
            var result = Soloud.init();
            if (result != 0)
                throw new Exception("SoLoud init failed");

            _audioObjectMapping = new Dictionary<uint, Wav>();
            _audioPlayObjects = new List<AudioPlayObject>();
        }

        public void Update()
        {
            for (int i = _audioPlayObjects.Count - 1; i >= 0; i--)
            {
                var obj = _audioPlayObjects[i];

                // Remove if finished
                if (Soloud.isValidVoiceHandle(obj.Handle.Id) == 0)
                {
                    _audioPlayObjects[i].OnFinished.Invoke();
                    _audioPlayObjects.RemoveAt(i);
                    continue;
                }

                // Apply volume
                Soloud.setVolume(obj.Handle.Id, obj.Volume);

                // Apply position
                if (obj.Is3D)
                {
                    Soloud.set3dSourcePosition(
                        obj.Handle.Id,
                        obj.Position.X,
                        obj.Position.Y,
                        obj.Position.Z
                    );
                }
            }

            Soloud.update3dAudio();
        }

        public void SetListener(Vector3 position, Vector3 forward, Vector3 up)
        {
            Soloud.set3dListenerPosition(position.X, position.Y, position.Z);
            Soloud.set3dListenerAt(forward.X, forward.Y, forward.Z);
            Soloud.set3dListenerUp(up.X, up.Y, up.Z);
        }

        public AudioClipHandle Load(string path, bool stream)
        {
            if (stream)
            {
                throw new NotImplementedException("[Audio/SoLoudBackend]: Audio streaming not supported yet.");
            }

            AudioClipHandle audioHandle = new AudioClipHandle(++_nextAudioId);
            Wav audio = new Wav();
            int result = audio.load(path);
            if (result != 0)
                throw new Exception("Audio Load Failed");


            _audioObjectMapping[audioHandle.Id] = audio;
            return audioHandle;
        }

        public AudioPlayObject Play3D(AudioClipHandle clip, Vector3 pos, bool loop = false)
        {
            if (!_audioObjectMapping.TryGetValue(clip.Id, out var wav))
                return null;

            uint voice = Soloud.play3d(wav, pos.X, pos.Y, pos.Z);


            Soloud.set3dSourceMinMaxDistance(voice, 1.0f, 5.0f);
            Soloud.set3dSourceAttenuation(voice, 2, 1.0f);
            Soloud.set3dSourceDopplerFactor(voice, 1.0f);

            Soloud.setLooping(voice, loop ? 1 : 0);
            var playObject = new AudioPlayObject
            {
                Handle = new AudioPlayHandle(voice),
                Clip = clip,
                Position = pos,
                Volume = 1.0f,
                Is3D = true
            };

            _audioPlayObjects.Add(playObject);

            return playObject;
        }

        public void Dispose()
        {
            _audioObjectMapping?.Clear();

            Soloud?.deinit();
            Soloud = null;
        }
    }
}
