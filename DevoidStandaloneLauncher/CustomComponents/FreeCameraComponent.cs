using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Utilities;
using System;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class FreeCameraComponent : Component
    {
        public override string Type => nameof(FreeCameraComponent);

        // ===== SETTINGS =====
        public float MoveSpeed = 8f;
        public float BoostMultiplier = 3f;
        public float MouseSensitivity = 4f;
        public float MinPitch = -89f;
        public float MaxPitch = 89f;

        // ===== INTERNAL =====
        private float yaw;
        private float pitch;

        public override void OnStart()
        {
            // Initialize yaw/pitch from current rotation
            Vector3 euler = gameObject.transform.EulerAngles;
            yaw = euler.Y;
            pitch = euler.X;
        }

        public override void OnUpdate(float dt)
        {
            HandleMouseLook(dt);
            HandleMovement(dt);
        }


        float time = 0;
        // =========================================
        // Mouse Look
        // =========================================
        private void HandleMouseLook(float dt)
        {
            time += dt;
            //yaw += MathF.Sin((float)time * 20f) * 0.5f;
            yaw -= Input.MouseDelta.X * dt * MouseSensitivity;
            pitch += Input.MouseDelta.Y * dt * MouseSensitivity;
            //pitch = 0;
            //pitch = MathF.Sin((float)time * 20f) * 0.5f;

            Quaternion rotation =
                Quaternion.CreateFromYawPitchRoll(
                    MathHelper.DegToRad(yaw),
                    MathHelper.DegToRad(pitch),
                    0f);

            gameObject.transform.Rotation = rotation;
        }


        // =========================================
        // Movement
        // =========================================
        private void HandleMovement(float dt)
        {
            Vector3 forward =
                Vector3.Normalize(
                    Vector3.Transform(Vector3.UnitZ, gameObject.transform.Rotation));

            Vector3 right =
                Vector3.Normalize(
                    Vector3.Cross(forward, Vector3.UnitY));

            Vector3 up = Vector3.UnitY;

            Vector3 move = Vector3.Zero;

            if (Input.GetKey(Keys.W)) move += forward;
            if (Input.GetKey(Keys.S)) move -= forward;
            if (Input.GetKey(Keys.D)) move += right;
            if (Input.GetKey(Keys.A)) move -= right;
            if (Input.GetKey(Keys.E)) move += up;
            if (Input.GetKey(Keys.Q)) move -= up;

            if (move.LengthSquared() > 0)
                move = Vector3.Normalize(move);

            float speed = MoveSpeed;

            if (Input.GetKey(Keys.LeftShift))
                speed *= BoostMultiplier;

            gameObject.transform.Position += move * speed * dt;
        }
    }
}
