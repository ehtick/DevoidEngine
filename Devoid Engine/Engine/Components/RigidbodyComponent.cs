using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class RigidBodyComponent : Component
    {
        public override string Type => nameof(RigidBodyComponent);

        public float Mass = 10f;
        public bool StartKinematic = false;

        public bool FreezeRotationX = false;
        public bool FreezeRotationY = false;
        public bool FreezeRotationZ = false;

        public PhysicsShapeDescription Shape = new PhysicsShapeDescription
        {
            Type = PhysicsShapeType.Box,
            Size = new Vector3(1, 1, 1)
        };

        public PhysicsMaterial Material = PhysicsMaterial.Default;

        internal IPhysicsBody InternalBody;

        public override void OnStart()
        {
            // Safety fallback if user forgot to define shape properly
            if (Shape.Type == PhysicsShapeType.Box &&
                Shape.Size == Vector3.Zero)
            {
                Shape.Size = new Vector3(1, 1, 1);
            }

            var desc = new PhysicsBodyDescription
            {
                Position = gameObject.transform.Position,
                Rotation = gameObject.transform.Rotation,
                Mass = Mass,
                IsKinematic = StartKinematic,
                Shape = Shape,
                Material = Material
            };

            InternalBody = gameObject.Scene.Physics.CreateBody(desc, gameObject);
        }

        public override void OnUpdate(float dt)
        {
            if (InternalBody == null)
                return;

            // If kinematic, push transform → physics
            if (InternalBody.IsKinematic)
            {
                InternalBody.Position = gameObject.transform.Position;
                InternalBody.Rotation = gameObject.transform.Rotation;
            }
        }

        public override void OnLateUpdate(float dt)
        {
            if (InternalBody == null)
                return;

            // If dynamic, pull physics → transform
            if (!InternalBody.IsKinematic)
            {
                gameObject.transform.Position = InternalBody.Position;

                Vector3 angVel = InternalBody.AngularVelocity;

                if (FreezeRotationX) angVel.X = 0f;
                if (FreezeRotationY) angVel.Y = 0f;
                if (FreezeRotationZ) angVel.Z = 0f;

                InternalBody.AngularVelocity = angVel;

                gameObject.transform.Rotation = InternalBody.Rotation;
            }

        }

        public override void OnDestroy()
        {
            InternalBody?.Remove();
            InternalBody = null;
        }

        // ===============================
        // Physics API
        // ===============================

        public void AddImpulse(Vector3 impulse)
        {
            InternalBody?.AddImpulse(impulse);
        }

        public void AddForce(Vector3 force)
        {
            InternalBody?.AddForce(force, gameObject.Scene.Physics.FixedDeltaTime);
        }

        public void AddTorque(Vector3 torque)
        {
            InternalBody?.AddTorque(torque);
        }

        public void SetLinearVelocity(Vector3 velocity)
        {
            if (InternalBody != null)
                InternalBody.LinearVelocity = velocity;
        }

        public void SetAngularVelocity(Vector3 velocity)
        {
            if (InternalBody != null)
                InternalBody.AngularVelocity = velocity;
        }
    }
}
