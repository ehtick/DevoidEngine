using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Trees;
using DevoidEngine.Engine.Core;
using OpenTK.Windowing.Common.Input;
using System.Numerics;

namespace DevoidEngine.Engine.Physics.Bepu
{

    internal struct RayHitHandler : IRayHitHandler
    {
        public bool Hit;
        public float Distance;
        public Vector3 Normal;
        public CollidableReference Collidable;

        public bool AllowTest(CollidableReference collidable) => true;

        public bool AllowTest(CollidableReference collidable, int childIndex) => true;

        public void OnRayHit(
            in RayData ray,
            ref float maximumT,
            float t,
            in Vector3 normal,
            CollidableReference collidable,
            int childIndex)
        {
            Hit = true;
            Distance = t;
            Normal = normal;
            Collidable = collidable;

            maximumT = t; // keep closest hit
        }
    }


}
