using System;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    internal class SurveillanceCameraComponent : Component
    {
        public override string Type => nameof(SurveillanceCameraComponent);

        public float SweepAngle = 45f;   // degrees left/right
        public float Speed = 0.75f;       // oscillation speed

        float time = 0f;
        float baseYaw = 0f;

        public override void OnStart()
        {
            baseYaw = gameObject.transform.EulerAngles.Y;
        }

        public override void OnUpdate(float dt)
        {
            time += dt * Speed;

            float yawOffset = MathF.Sin(time) * SweepAngle;

            Vector3 euler = gameObject.transform.EulerAngles;
            euler.Y = baseYaw + yawOffset;

            gameObject.transform.EulerAngles = euler;
        }
    }
}