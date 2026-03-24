using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Utilities;
using DevoidGPU;
using System.Numerics;

namespace DevoidStandaloneLauncher.CustomComponents
{
    public class InfoBubbleComponent : Component
    {
        public override string Type => nameof(InfoBubbleComponent);

        private ScreenArrowComponent screenArrow;
        private AudioSourceComponent3D audioSource;
        private AreaComponent playerTriggerArea;

        private bool interacted = false;

        private Vector3 startPos;
        private Vector3 startRot;

        private float bobTimer = 0f;
        private float rotationSpeed = 90; // 90° per second

        // Configurable (important later)
        public float InteractionDistance = 5f;
        public float ResetDistance = 15f;
        public float BobHeight = 0.5f;
        public float BobSpeed = 2f;
        public string AudioPath = "Engine/Content/Audio/game1.wav";

        public override void OnStart()
        {
            gameObject.Transform.Interpolated = false;
            startPos = gameObject.Transform.Position;
            startRot = gameObject.Transform.EulerAngles;

            audioSource = gameObject.AddComponent<AudioSourceComponent3D>();
            audioSource.PlayOnStart = false;
            audioSource.MaxDistance = 10;
            audioSource.MinDistance = 0;
            audioSource.Volume = 0.2f;

            screenArrow = gameObject.AddComponent<ScreenArrowComponent>();
            screenArrow.ArrowTexture = Helper.LoadImageAsTex(
                "Engine/Content/Textures/E_Key_Dark.png",
                TextureFilter.Linear
            );
            screenArrow.TargetObject = gameObject;
            screenArrow.StartIndicator();
        }

        public override void OnFixedUpdate(float dt)
        {
            if (DevoidEngine.Engine.InputSystem.Input.GetActionDown("MCLICK"))
            {
                Console.WriteLine("Hello");
            }

            bobTimer += dt;

            var cam = gameObject.Scene.GetMainCamera()?.gameObject;
            if (cam == null) return;

            float distance = Vector3.Distance(
                cam.Transform.Position,
                gameObject.Transform.Position
            );

            if (interacted && distance > ResetDistance)
            {
                ResetState();
            }

            if (!interacted)
            {
                UpdateFloating(dt);

                if (distance <= InteractionDistance && DevoidEngine.Engine.InputSystem.Input.GetActionDown("Interact"))
                {
                    Interact();
                }
            }
            else
            {
                UpdateRotation(dt);
            }
        }

        private void ResetState()
        {
            interacted = false;

            // Reset transform
            gameObject.Transform.Position = startPos;
            gameObject.Transform.EulerAngles = startRot;

            // Restart indicator
            screenArrow.StartIndicator();

            // Optional: stop audio if still playing
            audioSource.Stop();
        }

        private void UpdateFloating(float dt)
        {
            float yOffset = MathF.Sin(bobTimer * BobSpeed) * BobHeight;

            gameObject.Transform.Position = new Vector3(
                startPos.X,
                startPos.Y + yOffset,
                startPos.Z
            );
        }

        private void UpdateRotation(float dt)
        {
            var rot = gameObject.Transform.EulerAngles;

            rot.Y += rotationSpeed * dt;

            gameObject.Transform.EulerAngles = new Vector3(
                startRot.X,
                rot.Y,
                startRot.Z
            );

            Console.WriteLine(rot);
        }

        private void TryInteract()
        {
            var cam = gameObject.Scene.GetMainCamera()?.gameObject;
            if (cam == null) return;

            float distance = Vector3.Distance(
                cam.Transform.Position,
                gameObject.Transform.Position
            );

            if (distance > InteractionDistance) return;

            if (DevoidEngine.Engine.Core.Input.GetKeyDown(DevoidEngine.Engine.Core.Keys.E))
            {
                Interact();
            }
        }

        private void Interact()
        {
            if (interacted) return;

            interacted = true;

            screenArrow.StopIndicator();

            audioSource.AudioPath = Path.GetFullPath(AudioPath);
            audioSource.Play();
        }
    }
}