using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Utilities;
using System;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class FPSController : Component
    {
        public override string Type => nameof(FPSController);

        public float MoveSpeed = 6f;
        public float JumpForce = 1000f;
        public float MouseSensitivity = 0.15f;
        public float MinPitch = -89f;
        public float MaxPitch = 89f;
        public float GroundCheckDistance = 0.2f;

        private RigidBodyComponent rb;
        private Transform cameraPivot;

        private float yaw = 0f;
        private float pitch = 0f;

        private Vector2 storedMoveInput = Vector2.Zero;
        private bool jumpRequested = false;

        public override void OnStart()
        {
            rb = gameObject.GetComponent<RigidBodyComponent>();
            if (rb == null) return;

            // Physics should NOT rotate the body
            rb.FreezeRotationX = true;
            rb.FreezeRotationY = true;
            rb.FreezeRotationZ = true;

            if (gameObject.children.Count > 0)
                cameraPivot = gameObject.children[0].transform;
        }

        // VARIABLE RATE (INPUT SAMPLING ONLY)
        public override void OnUpdate(float dt)
        {
            if (rb == null) return;

            // Pull move input snapshot
            storedMoveInput = Input.MoveAxis;

            // Integrate mouse delta here (Update thread) so it's not lost by fixed-step timing
            Vector2 mouseDelta = Input.MouseDelta;
            if (mouseDelta.Y != 0)
            {
                Console.WriteLine($"[Input] DeltaX: {mouseDelta.X}");
            }

            yaw += mouseDelta.X * MouseSensitivity;
            pitch -= mouseDelta.Y * MouseSensitivity;
            pitch = Math.Clamp(pitch, MinPitch, MaxPitch);

            if (Input.JumpPressed)
                jumpRequested = true;
        }

        // Update-thread visual update so camera pivot is smooth every frame
        public override void OnLateUpdate(float dt)
        {
            if (cameraPivot != null)
            {
                cameraPivot.LocalRotation =
                    Quaternion.CreateFromAxisAngle(
                        Vector3.UnitX,
                        MathHelper.DegToRad(pitch)
                    );
            }
        }

        // FIXED RATE (ALL TRANSFORM + PHYSICS)
        public override void OnFixedUpdate(float fixedDt)
        {
            if (rb == null) return;

            ApplyRotation();     // apply yaw to physics body (keeps physics deterministic)
            ApplyMovement();     // physics-safe movement

            jumpRequested = false;
        }

        private void ApplyRotation()
        {
            // Horizontal rotation (yaw) applied to physics body
            Quaternion bodyRotation =
                Quaternion.CreateFromAxisAngle(
                    Vector3.UnitY,
                    MathHelper.DegToRad(yaw)
                );

            rb.Rotation = bodyRotation;

            // NOTE: cameraPivot.LocalRotation is intentionally updated in OnLateUpdate (update thread)
            // so visuals are updated every frame and don't wait for the fixed/physics tick.
        }

        private void ApplyMovement()
        {
            Quaternion rotation = rb.Rotation;

            Vector3 forward =
                Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, rotation));

            Vector3 right =
                Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));

            forward.Y = 0;
            right.Y = 0;

            Vector3 moveDir = forward * storedMoveInput.Y + right * storedMoveInput.X;

            if (moveDir.LengthSquared() > 0)
                moveDir = Vector3.Normalize(moveDir);

            Vector3 velocity = rb.LinearVelocity;

            float verticalVelocity = velocity.Y;

            velocity = moveDir * MoveSpeed;
            velocity.Y = verticalVelocity;

            rb.LinearVelocity = velocity;

            if (jumpRequested && IsGrounded())
            {
                rb.AddImpulse(Vector3.UnitY * JumpForce);
            }
        }

        private bool IsGrounded()
        {
            Vector3 origin = gameObject.transform.Position + new Vector3(0, -1.4f, 0);
            Vector3 direction = -Vector3.UnitY;

            return gameObject.Scene.Physics.Raycast(
                new Ray(origin, direction),
                GroundCheckDistance,
                out RaycastHit hit
            );
        }
    }
}
