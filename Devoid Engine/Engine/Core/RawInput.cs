using System.Collections.Concurrent;

namespace DevoidEngine.Engine.Core
{
    public static class RawInputQueue
    {
        public static ConcurrentQueue<MouseMoveEvent> MouseMoves = new();
        public static ConcurrentQueue<MouseButtonEvent> MouseButtons = new();
        public static ConcurrentQueue<KeyboardEvent> KeyEvents = new();
        public static ConcurrentQueue<MouseWheelEvent> MouseWheels = new();
    }

}
