namespace DevoidEngine.Engine.Utilities
{
    public class AsyncDoubleBuffer<T> where T : class
    {
        private T _front; // render reads this
        private T _back;  // update writes this

        public T Front => Volatile.Read(ref _front);

        public T Back => _back; // only update touches this

        public AsyncDoubleBuffer(T initialFront, T initialBack)
        {
            _front = initialFront;
            _back = initialBack;
        }

        // Called ONLY from update thread
        public void Publish()
        {
            // Ensure writes to Back are visible before swap
            Thread.MemoryBarrier();

            // Atomically swap front and back
            var oldFront = Interlocked.Exchange(ref _front, _back);

            // Now oldFront becomes new back
            _back = oldFront;
        }
    }

}
