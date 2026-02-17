using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Physics
{
    [Flags]
    public enum PhysicsLayer
    {
        Default = 1 << 0,
        Player = 1 << 1,
        Enemy = 1 << 2,
        Environment = 1 << 3
    }

}
