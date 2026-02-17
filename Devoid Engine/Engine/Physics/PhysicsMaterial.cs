using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Physics
{
    public struct PhysicsMaterial
    {
        public float Friction;
        public float Restitution; // bounciness
        public float LinearDamping;
        public float AngularDamping;

        public static PhysicsMaterial Default => new PhysicsMaterial
        {
            Friction = 0.5f,
            Restitution = 0.1f,
            LinearDamping = 1f,
            AngularDamping = 3f
        };
    }

}
