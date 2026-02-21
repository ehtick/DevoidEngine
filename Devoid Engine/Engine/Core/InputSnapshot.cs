using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public class InputSnapshot
    {
        public Vector2 MouseDelta;
        public Vector2 MouseScroll;
        public HashSet<Keys> Keys;
        public HashSet<MouseButton> Mouse;
    }
}
