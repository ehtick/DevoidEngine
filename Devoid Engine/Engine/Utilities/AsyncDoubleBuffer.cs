namespace DevoidEngine.Engine.Utilities
{
    public class AsyncDoubleBuffer<T> where T : class
    {
        private T _front; // render reads this
        private T _back;  // update writes this
        private int _hasNewFrame = 0;

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
            Thread.MemoryBarrier();

            var oldFront = Interlocked.Exchange(ref _front, _back);
            _back = oldFront;

            Volatile.Write(ref _hasNewFrame, 1);
        }

        public T Consume()
        {
            if (Interlocked.Exchange(ref _hasNewFrame, 0) == 1)
            {
                return Volatile.Read(ref _front);
            }

            return _front; // reuse previous frame safely
        }
    }

}