using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Audio
{
    internal interface IAudioBackend : IDisposable
    {
        void Initialize();
        void Update();
        void SetListener(Vector3 position, Vector3 forward, Vector3 up);
        AudioClipHandle Load(string path, bool stream = false);
        AudioPlayObject Play3D(AudioClipHandle clip, Vector3 pos, bool loop = false);
    }
}
