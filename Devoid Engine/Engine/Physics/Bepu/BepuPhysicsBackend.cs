using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuUtilities.Memory;
using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Physics;
using SharpDX.DXGI;
using System.Numerics;

namespace DevoidEngine.Engine.Physics.Bepu
{
    internal class BepuPhysicsBackend : IPhysicsBackend
    {
        private Simulation simulation;
        private BufferPool bufferPool;

        private Dictionary<BodyHandle, GameObject> bodyToGameObject = new Dictionary<BodyHandle, GameObject>();


        public void Initialize()
        {
            bufferPool = new BufferPool();

            simulation = Simulation.Create(
                bufferPool,
                new BepuNarrowPhaseCallbacks(),
                new BepuPoseIntegratorCallbacks { Gravity = new Vector3(0, -9.81f, 0) },
                new SolveDescription(8, 1),
                new DefaultTimestepper()
            );
        }

        public void Step(float deltaTime)
        {
            simulation.Timestep(deltaTime);
        }

        public IPhysicsBody CreateBody(PhysicsBodyDescription desc, GameObject owner)
        {
            var shape = new Box(1, 1, 1);

            var shapeIndex = simulation.Shapes.Add(shape);

            var bodyDescription = BodyDescription.CreateDynamic(
                desc.Position,
                shape.ComputeInertia(desc.Mass),
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f));

            BodyHandle handle = simulation.Bodies.Add(bodyDescription);

            bodyToGameObject[handle] = owner;

            return new BepuPhysicsBody(handle, simulation);
        }


        public void RemoveBody(IPhysicsBody body)
        {
            if (body is BepuPhysicsBody b)
            {
                simulation.Bodies.Remove(b.Handle);
            }
        }

        public bool Raycast(Ray ray, float maxDistance, out RaycastHit hit)
        {
            hit = default;

            var handler = new RayHitHandler();

            simulation.RayCast(
                ray.Origin,
                ray.Direction,
                maxDistance,
                ref handler);

            if (!handler.Hit)
                return false;

            hit.Distance = handler.Distance;
            hit.Point = ray.GetPoint(handler.Distance);
            hit.Normal = handler.Normal;

            if (handler.Collidable.Mobility == CollidableMobility.Dynamic)
            {
                var bodyHandle = handler.Collidable.BodyHandle;

                if (bodyToGameObject.TryGetValue(bodyHandle, out var go))
                    hit.HitObject = go;
            }

            return true;
        }

    }

}