using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Core
{
    public static class UpdateThreadDispatcher
    {
        private static readonly ConcurrentQueue<Action> actions = new();

        public static void Queue(Action action) => actions.Enqueue(action);

        public static void ExecutePending()
        {
            while (actions.TryDequeue(out var action))
                action();
        }
    }

    public static class RenderThreadDispatcher
    {
        private static readonly ConcurrentQueue<Action> actions = new();

        public static void Queue(Action action) => actions.Enqueue(action);

        public static void ExecutePending()
        {
            while (actions.TryDequeue(out var action))
                action();
        }
    }
}