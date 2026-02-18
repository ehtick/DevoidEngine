using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class FPSController : Component
    {
        public override string Type => nameof(FPSController);

        // ===============================
        // Settings
        // ===============================

        public float MoveSpeed = 6f;
        public float JumpForce = 1000f;
        public float MouseSensitivity = 0.15f;
        public float MinPitch = -89f;
        public float MaxPitch = 89f;
        public float GroundCheckDistance = 1.6f;

        // ===============================
        // Internals
        // ===============================

        private RigidBodyComponent rb;
        private Transform cameraPivot;

        private float yaw = 0f;
        private float pitch = 0f;

        // ===============================
        // Setup
        // ===============================

        public override void OnStart()
        {
            rb = gameObject.GetComponent<RigidBodyComponent>();
            if (rb == null)
                return;

            // Fully freeze rigidbody rotation
            rb.FreezeRotationX = true;
            rb.FreezeRotationY = true;
            rb.FreezeRotationZ = true;

            if (gameObject.children.Count > 0)
                cameraPivot = gameObject.children[0].transform;
        }

        // ===============================
        // Update
        // ===============================

        public override void OnUpdate(float dt)
        {
            if (rb == null)
                return;

            HandleMouseLook();
            HandleMovement();
        }

        // ===============================
        // Mouse Look (Visual Only)
        // ===============================

        private void HandleMouseLook()
        {
            Vector2 mouseDelta = Input.MouseDelta;

            yaw += mouseDelta.X * MouseSensitivity;
            pitch -= mouseDelta.Y * MouseSensitivity;

            pitch = Math.Clamp(pitch, MinPitch, MaxPitch);

            // Rotate player visually (Y axis)
            gameObject.transform.LocalRotation =
                Quaternion.CreateFromAxisAngle(
                    Vector3.UnitY,
                    MathHelper.DegToRad(yaw)
                );

            // Rotate camera pivot (X axis)
            if (cameraPivot != null)
            {
                cameraPivot.LocalRotation =
                    Quaternion.CreateFromAxisAngle(
                        Vector3.UnitX,
                        MathHelper.DegToRad(pitch)
                    );
            }
        }

        // ===============================
        // Movement (Velocity Hybrid)
        // ===============================

        private void HandleMovement()
        {
            Vector2 input = Input.MoveAxis;

            Quaternion rotation = gameObject.transform.Rotation;

            Vector3 forward =
                Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, rotation));

            Vector3 right =
                Vector3.Normalize(Vector3.Transform(Vector3.UnitX, rotation));

            forward.Y = 0;
            right.Y = 0;

            Vector3 moveDir = forward * input.Y + right * input.X;

            if (moveDir.LengthSquared() > 0)
                moveDir = Vector3.Normalize(moveDir);

            Vector3 velocity = rb.LinearVelocity;

            // Preserve vertical velocity from physics
            float verticalVelocity = velocity.Y;

            // Overwrite horizontal velocity only
            velocity = moveDir * MoveSpeed;
            velocity.Y = verticalVelocity;

            rb.LinearVelocity = velocity;

            // Jump (impulse only when grounded)
            if (Input.JumpPressed && IsGrounded())
            {
                Console.WriteLine("Added Impulse");
                rb.AddImpulse(Vector3.UnitY * JumpForce);
            }
        }

        // ===============================
        // Ground Check
        // ===============================

        private bool IsGrounded()
        {
            Vector3 origin = gameObject.transform.Position;
            Vector3 direction = -Vector3.UnitY;

            return gameObject.Scene.Physics.Raycast(
                new Ray(origin, direction),
                GroundCheckDistance,
                out RaycastHit hit
            );
        }
    }
}
