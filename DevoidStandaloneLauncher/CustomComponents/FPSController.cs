using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Utilities;
using System.Numerics;
using System;

namespace DevoidEngine.Engine.Components
{
    public class FPSController : Component
    {
        public override string Type => nameof(FPSController);

        // ===============================
        // Settings
        // ===============================

        public float MoveSpeed = 6f;
        // FIX 1: Increased JumpForce so it actually lifts your 10kg Rigidbody
        public float JumpForce = 1000f;
        public float MouseSensitivity = 0.15f;
        public float MinPitch = -89f;
        public float MaxPitch = 89f;
        public float GroundCheckDistance = 0.2f; // Shortened since we are offsetting the raycast

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
        // Mouse Look
        // ===============================

        private void HandleMouseLook()
        {
            Vector2 mouseDelta = Input.MouseDelta;

            Console.WriteLine(mouseDelta);

            yaw += mouseDelta.X * MouseSensitivity;
            pitch -= mouseDelta.Y * MouseSensitivity;

            pitch = Math.Clamp(pitch, MinPitch, MaxPitch);

            // FIX 2: Apply Y-axis rotation to the Physics Body (rb), NOT the visual transform.
            // This stops the physics engine from overwriting your rotation and causing jitter.
            rb.Rotation = Quaternion.CreateFromAxisAngle(
                Vector3.UnitY,
                MathHelper.DegToRad(yaw)
            );

            // Rotate camera pivot (X axis) - this is fine to do via transform since it has no physics body
            if (cameraPivot != null)
            {
                cameraPivot.LocalRotation = Quaternion.CreateFromAxisAngle(
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

            Vector3 forward = Vector3.Normalize(Vector3.Transform(Vector3.UnitZ, rotation));
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));

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
            // FIX 3: Offset the raycast origin down by 1.4 units. 
            // If it starts directly at the center, it immediately hits the inside of the player's own physics capsule.
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