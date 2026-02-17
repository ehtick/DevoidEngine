using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using DevoidEngine.Engine.Utilities;
using System.Numerics;

namespace DevoidEngine.Engine.Components
{
    public class RigidBodyComponent : Component
    {
        public override string Type => nameof(RigidBodyComponent);

        public float Mass = 1f;
        public bool StartKinematic = false;

        internal IPhysicsBody InternalBody;

        private bool initialized;

        public override void OnStart()
        {
            var desc = new PhysicsBodyDescription
            {
                Position = gameObject.transform.Position,
                Rotation = TransformMath.EulerToQuaternion(gameObject.transform.Rotation),
                Mass = Mass,
                IsKinematic = StartKinematic
            };

            InternalBody = gameObject.Scene.Physics.CreateBody(desc, gameObject);
        }

        public override void OnUpdate(float dt)
        {
            if (InternalBody == null)
                return;

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

            if (!InternalBody.IsKinematic)
            {
                gameObject.transform.Position = InternalBody.Position;

                gameObject.transform.Rotation =
                    TransformMath.QuaternionToEuler(InternalBody.Rotation);
            }
        }



        public void AddImpulse(Vector3 impulse)
        {
            InternalBody?.AddImpulse(impulse);
        }

        public void AddForce(Vector3 force)
        {
            InternalBody?.AddForce(force, gameObject.Scene.Physics.FixedDeltaTime);
        }
    }
}
