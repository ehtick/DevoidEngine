using DevoidEngine.Engine.Core;
using System.Numerics;

namespace DevoidEngine.Engine.Physics
{
    public class PhysicsSystem
    {
        private readonly IPhysicsBackend backend;

        private float accumulator = 0f;

        public float FixedDeltaTime { get; } = 1f / 60f;

        public PhysicsSystem(IPhysicsBackend backend)
        {
            this.backend = backend;
            backend.Initialize();
        }

        public void Step(float frameDelta)
        {
            accumulator += frameDelta;

            while (accumulator >= FixedDeltaTime)
            {
                backend.Step(FixedDeltaTime);
                accumulator -= FixedDeltaTime;
            }
        }

        public bool Raycast(Ray ray, float maxDistance, out RaycastHit hit)
        {
            return backend.Raycast(ray, maxDistance, out hit);
        }

        public IPhysicsBody CreateBody(PhysicsBodyDescription desc, GameObject owner)
        {
            return backend.CreateBody(desc, owner);
        }

        public void CreateStatic(PhysicsStaticDescription desc, GameObject owner)
        {
            backend.CreateStatic(desc, owner);
        }

        public void RemoveBody(IPhysicsBody body)
        {
            backend.RemoveBody(body);
        }
    }
}
