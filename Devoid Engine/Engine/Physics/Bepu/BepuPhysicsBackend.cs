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
        private Dictionary<StaticHandle, GameObject> staticToGameObject = new Dictionary<StaticHandle, GameObject>();


        public void Initialize()
        {
            bufferPool = new BufferPool();

            simulation = Simulation.Create(
                bufferPool,
                new BepuNarrowPhaseCallbacks(),
                new BepuPoseIntegratorCallbacks { Gravity = new Vector3(0, 9.81f, 0) },
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
            TypedIndex shapeIndex = CreateShape(
                desc.Shape,
                desc.Mass,
                out BodyInertia inertia);

            var pose = new RigidPose(desc.Position, desc.Rotation);

            var bodyDescription = BodyDescription.CreateDynamic(
                pose,
                inertia,
                new CollidableDescription(shapeIndex, 0.1f),
                new BodyActivityDescription(0.01f)
            );

            BodyHandle handle = simulation.Bodies.Add(bodyDescription);

            bodyToGameObject[handle] = owner;

            return new BepuPhysicsBody(handle, simulation);
        }




        public void CreateStatic(PhysicsStaticDescription desc, GameObject owner)
        {
            // 1️⃣ Create shape (no inertia needed for statics)
            TypedIndex shapeIndex = CreateShapeStatic(desc.Shape);

            // 2️⃣ Create pose
            var pose = new RigidPose(desc.Position, desc.Rotation);

            // 3️⃣ Create static description
            var staticDescription = new StaticDescription(
                pose,
                shapeIndex
            );

            // 4️⃣ Add to simulation
            StaticHandle handle = simulation.Statics.Add(staticDescription);

            // 5️⃣ Store mapping
            staticToGameObject[handle] = owner;
        }




        private TypedIndex CreateShape(
            PhysicsShapeDescription shapeDesc,
            float mass,
            out BodyInertia inertia)
        {
            switch (shapeDesc.Type)
            {
                case PhysicsShapeType.Box:
                    {
                        var shape = new Box(
                            shapeDesc.Size.X,
                            shapeDesc.Size.Y,
                            shapeDesc.Size.Z);

                        inertia = shape.ComputeInertia(mass);
                        return simulation.Shapes.Add(shape);
                    }

                case PhysicsShapeType.Sphere:
                    {
                        var shape = new Sphere(shapeDesc.Radius);

                        inertia = shape.ComputeInertia(mass);
                        return simulation.Shapes.Add(shape);
                    }

                case PhysicsShapeType.Capsule:
                    {
                        var shape = new Capsule(
                            shapeDesc.Radius,
                            shapeDesc.Height);

                        inertia = shape.ComputeInertia(mass);
                        return simulation.Shapes.Add(shape);
                    }

                case PhysicsShapeType.ConvexHull:
                    {
                        var shape = new ConvexHull(
                            shapeDesc.Vertices,
                            bufferPool,
                            out _);

                        inertia = shape.ComputeInertia(mass);
                        return simulation.Shapes.Add(shape);
                    }

                //case PhysicsShapeType.Mesh:
                //    {
                //        var shape = new Mesh(
                //            shapeDesc.Vertices,
                //            shapeDesc.Indices,
                //            bufferPool);

                //        // Mesh should NOT be dynamic
                //        inertia = default;
                //        return simulation.Shapes.Add(shape);
                //    }

                default:
                    throw new NotSupportedException("Shape not supported.");
            }
        }

        private TypedIndex CreateShapeStatic(PhysicsShapeDescription shapeDesc)
        {
            switch (shapeDesc.Type)
            {
                case PhysicsShapeType.Box:
                    {
                        var shape = new Box(
                            shapeDesc.Size.X,
                            shapeDesc.Size.Y,
                            shapeDesc.Size.Z);

                        return simulation.Shapes.Add(shape);
                    }

                case PhysicsShapeType.Sphere:
                    {
                        var shape = new Sphere(shapeDesc.Radius);
                        return simulation.Shapes.Add(shape);
                    }

                case PhysicsShapeType.Capsule:
                    {
                        var shape = new Capsule(
                            shapeDesc.Radius,
                            shapeDesc.Height);

                        return simulation.Shapes.Add(shape);
                    }

                case PhysicsShapeType.ConvexHull:
                    {
                        var shape = new ConvexHull(
                            shapeDesc.Vertices,
                            bufferPool,
                            out _);

                        return simulation.Shapes.Add(shape);
                    }

                //case PhysicsShapeType.Mesh:
                //    {
                //        var shape = new Mesh(
                //            shapeDesc.Vertices,
                //            shapeDesc.Indices,
                //            bufferPool);

                //        return simulation.Shapes.Add(shape);
                //    }

                default:
                    throw new NotSupportedException("Unsupported static shape type");
            }
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
            else if (handler.Collidable.Mobility == CollidableMobility.Static)
            {
                var staticHandle = handler.Collidable.StaticHandle;

                if (staticToGameObject.TryGetValue(staticHandle, out var go))
                    hit.HitObject = go;
            }


            return true;
        }

    }

}