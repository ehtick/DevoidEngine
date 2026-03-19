using System.Numerics;

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