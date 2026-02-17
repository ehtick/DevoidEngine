using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class RigidBodyComponent : Component
    {
        public override string Type => nameof(RigidBodyComponent);

        // ===============================
        // Basic Settings
        // ===============================

        public float Mass = 1f;
        public bool StartKinematic = false;

        // ===============================
        // Shape Definition
        // ===============================

        public PhysicsShapeDescription Shape = new PhysicsShapeDescription
        {
            Type = PhysicsShapeType.Box,
            Size = new Vector3(1, 1, 1)
        };

        // ===============================
        // Physics Material (Per Object)
        // ===============================

        public PhysicsMaterial Material = PhysicsMaterial.Default;

        // ===============================
        // Internal
        // ===============================

        internal IPhysicsBody InternalBody;

        // ===============================
        // Lifecycle
        // ===============================

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
                Rotation = TransformMath.EulerToQuaternion(gameObject.transform.Rotation),
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
                InternalBody.Rotation =
                    TransformMath.EulerToQuaternion(gameObject.transform.Rotation);
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
                gameObject.transform.Rotation =
                    TransformMath.QuaternionToEuler(InternalBody.Rotation);
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
