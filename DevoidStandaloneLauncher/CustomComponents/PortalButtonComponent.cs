using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Utilities;
using System;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class PortalButtonComponent : Component
    {
        public override string Type => nameof(PortalButtonComponent);

        public float PressDepth = 0.35f;
        public float PressSpeed = 4f;
        public float RequiredMass = 1f;

        public bool IsPressed { get; private set; }

        public event Action OnPressed;
        public event Action OnReleased;

        private int overlappingBodies = 0;
        private Vector3 originalPosition;
        private float currentOffset = 0f;

        private AreaComponent area;

        public override void OnStart()
        {
            originalPosition = gameObject.transform.LocalPosition;
        }

        public void SetTriggerArea(AreaComponent area)
        {
            this.area = area;
            area.OnEnter += HandleEnter;
            area.OnExit += HandleExit;
        }

        public override void OnUpdate(float dt)
        {
            float target = IsPressed ? -PressDepth : 0f;

            currentOffset = MathHelper.Lerp(currentOffset, target, dt * PressSpeed);

            area.gameObject.transform.Position = originalPosition + new Vector3(0, 0.6f, 0);

            // Optional animation
            gameObject.transform.LocalPosition =
                originalPosition + new Vector3(0, currentOffset, 0);
        }

        private void HandleEnter(GameObject other)
        {
            if (other == gameObject) { return; }
            var rb = other.GetComponent<RigidBodyComponent>();
            if (rb == null) return;

            if (rb.Mass >= RequiredMass)
            {
                overlappingBodies++;

                if (!IsPressed)
                {
                    IsPressed = true;
                    OnPressed?.Invoke();
                }
            }
        }

        private void HandleExit(GameObject other)
        {
            if (other == gameObject) { return; }
            var rb = other.GetComponent<RigidBodyComponent>();
            if (rb == null) return;

            overlappingBodies--;

            if (overlappingBodies <= 0)
            {
                overlappingBodies = 0;

                if (IsPressed)
                {
                    IsPressed = false;
                    OnReleased?.Invoke();
                }
            }
        }
    }
}