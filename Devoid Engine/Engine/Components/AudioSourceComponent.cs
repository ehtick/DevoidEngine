using DevoidEngine.Audio;
using SoLoud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Components
{
    public class AudioSourceComponent : Component
    {
        public override string Type => nameof(AudioSourceComponent);

        string audioPath = "D:/Programming/Devoid Engine/Devoid Engine/Engine/Content/Audio/game1.wav";
        AudioPlayObject audioPlayer;
        public override void OnStart()
        {
            AudioClipHandle audioClip = gameObject.Scene.Audio.Load(audioPath);

            audioPlayer = gameObject.Scene.Audio.Play3D(audioClip, gameObject.transform.Position, true);
        }

        public override void OnUpdate(float dt)
        {
            Vector3 position = gameObject.Scene.GetMainCamera().gameObject.transform.Position;
            Vector3 forward = gameObject.Scene.GetMainCamera().gameObject.transform.Forward;
            Vector3 up = gameObject.Scene.GetMainCamera().gameObject.transform.Up;
            gameObject.Scene.Audio.SetListener(position, forward, up);

            audioPlayer.Position = gameObject.transform.Position;
        }

        public override void OnDestroy()
        {
            Console.WriteLine("Destroyed?");

            var stackTrace = new StackTrace(true); // true = include file info

            foreach (var frame in stackTrace.GetFrames())
            {
                var method = frame.GetMethod();
                var file = frame.GetFileName();
                var line = frame.GetFileLineNumber();

                Console.WriteLine($"{method.DeclaringType}.{method.Name} in {file}:line {line}");
            }
        }
    }
}
