using DevoidEngine.Engine.Core;
using System.Numerics;

namespace DevoidEngine.Engine.Physics
{
    public class PhysicsSystem
    {
        private readonly IPhysicsBackend backend;

        public PhysicsSystem(IPhysicsBackend backend)
        {
            this.backend = backend;
            backend.Initialize();
        }

        public void Step(float frameDelta)
        {
            backend.Step(frameDelta);
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
