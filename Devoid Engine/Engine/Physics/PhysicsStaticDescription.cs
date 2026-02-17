using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Physics
{
    public struct PhysicsStaticDescription
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public PhysicsShapeDescription Shape;
    }
}
