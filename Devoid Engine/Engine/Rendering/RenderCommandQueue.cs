using System.Collections.Concurrent;

namespace DevoidEngine.Engine.Rendering
{
    public class RenderCommandQueue
    {
        private static readonly ConcurrentQueue<Action> actions = new();

        public static void Queue(Action action) => actions.Enqueue(action);

        public static void Execute()
        {
            while (actions.TryDequeue(out var action))
                action();
        }


    }
}
