using DevoidEngine.Engine.Components;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Utilities;
using DevoidGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        public string AudioPath = "D:/Programming/Devoid Engine/Devoid Engine/Engine/Content/Audio/game1.wav";

        public override void OnStart()
        {
            startPos = gameObject.transform.Position;
            startRot = gameObject.transform.EulerAngles;

            audioSource = gameObject.AddComponent<AudioSourceComponent3D>();
            audioSource.PlayOnStart = false;
            audioSource.MaxDistance = 10;
            audioSource.MinDistance = 0;
            audioSource.Volume = 0.2f;

            screenArrow = gameObject.AddComponent<ScreenArrowComponent>();
            screenArrow.ArrowTexture = Helper.LoadImageAsTex(
                "D:/Programming/Devoid Engine/Devoid Engine/Engine/Content/Textures/E_Key_Dark.png",
                TextureFilter.Linear
            );
            screenArrow.TargetObject = gameObject;
            screenArrow.StartIndicator();
        }

        public override void OnUpdate(float dt)
        {
            bobTimer += dt;

            var cam = gameObject.Scene.GetMainCamera()?.gameObject;
            if (cam == null) return;

            float distance = Vector3.Distance(
                cam.transform.Position,
                gameObject.transform.Position
            );
            
            if (interacted && distance > ResetDistance)
            {
                ResetState();
            }

            if (!interacted)
            {
                UpdateFloating(dt);

                if (distance <= InteractionDistance && DevoidEngine.InputSystem.Input.GetActionDown("Interact"))
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
            gameObject.transform.Position = startPos;
            gameObject.transform.EulerAngles = startRot;

            // Restart indicator
            screenArrow.StartIndicator();

            // Optional: stop audio if still playing
            audioSource.Stop();
        }

        private void UpdateFloating(float dt)
        {
            float yOffset = MathF.Sin(bobTimer * BobSpeed) * BobHeight;

            gameObject.transform.Position = new Vector3(
                startPos.X,
                startPos.Y + yOffset,
                startPos.Z
            );
        }

        private void UpdateRotation(float dt)
        {
            var rot = gameObject.transform.EulerAngles;

            rot.Y += rotationSpeed * dt;

            gameObject.transform.EulerAngles = new Vector3(
                startRot.X,
                rot.Y,
                startRot.Z
            );
        }

        private void TryInteract()
        {
            var cam = gameObject.Scene.GetMainCamera()?.gameObject;
            if (cam == null) return;

            float distance = Vector3.Distance(
                cam.transform.Position,
                gameObject.transform.Position
            );

            if (distance > InteractionDistance) return;

            if (Input.GetKeyDown(Keys.E))
            {
                Interact();
            }
        }

        private void Interact()
        {
            if (interacted) return;

            interacted = true;

            screenArrow.StopIndicator();

            audioSource.AudioPath = AudioPath;
            audioSource.Play();
        }
    }
}
