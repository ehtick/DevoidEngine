using System.Collections.Concurrent;

namespace DevoidEngine.Engine.Core
{
    public static class UpdateThreadDispatcher
    {
        private static readonly ConcurrentQueue<Action> actions = new();

        private static readonly object _lock = new();
        private static readonly Dictionary<string, Action> _latestActions = new();

        public static void QueueLatest(string key, Action action)
        {
            lock (_lock)
            {
                _latestActions[key] = action; // overwrites previous for same key
            }
        }

        public static void Queue(Action action) => actions.Enqueue(action);

        public static void ExecutePending()
        {
            // Copy the latest actions safely
            Dictionary<string, Action> actionsCopy;
            lock (_lock)
            {
                actionsCopy = new Dictionary<string, Action>(_latestActions);
                _latestActions.Clear();
            }

            // Execute queued actions
            while (actions.TryDequeue(out var action))
                action();

            // Execute latest actions
            foreach (var latestAction in actionsCopy.Values)
                latestAction();
        }
    }

    public static class RenderThreadDispatcher
    {
        private static readonly ConcurrentQueue<Action> actions = new();

        private static readonly object _lock = new();
        private static readonly Dictionary<string, Action> _latestActions = new();

        public static void QueueLatest(string key, Action action)
        {
            lock (_lock)
            {
                _latestActions[key] = action; // overwrites previous for same key
            }
        }

        public static void Queue(Action action) => actions.Enqueue(action);

        public static void ExecutePending()
        {
            // Copy the latest actions safely
            Dictionary<string, Action> actionsCopy;
            lock (_lock)
            {
                actionsCopy = new Dictionary<string, Action>(_latestActions);
                _latestActions.Clear();
            }

            // Execute queued actions
            while (actions.TryDequeue(out var action))
                action();

            // Execute latest actions
            foreach (var latestAction in actionsCopy.Values)
                latestAction();
        }

    }
}