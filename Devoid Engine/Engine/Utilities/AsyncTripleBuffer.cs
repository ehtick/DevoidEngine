using System.Threading;

namespace DevoidEngine.Engine.Utilities
{
    public class AsyncTripleBuffer<T> where T : class
    {
        private readonly T[] _buffers = new T[3];

        // 0 = Free
        // 1 = Writing (update thread)
        // 2 = Reading (render thread)
        private readonly int[] _states = new int[3];

        // index of current front buffer (read by render)
        private int _frontIndex = 0;

        // previous front that must be released AFTER render
        private volatile int _previousFront = -1;

        public AsyncTripleBuffer(T a, T b, T c)
        {
            _buffers[0] = a;
            _buffers[1] = b;
            _buffers[2] = c;

            // initial state:
            // buffer 0 = front (reading)
            // others free
            _states[0] = 2;
            _states[1] = 0;
            _states[2] = 0;
        }

        // =============================
        // UPDATE THREAD
        // =============================

        public T AcquireBackBuffer(out int index)
        {
            var spinner = new SpinWait();

            while (true)
            {
                for (int i = 0; i < 3; i++)
                {
                    // try to grab a FREE buffer
                    if (Interlocked.CompareExchange(ref _states[i], 1, 0) == 0)
                    {
                        index = i;
                        return _buffers[i];
                    }
                }

                // no free buffer → wait briefly
                spinner.SpinOnce();
            }
        }

        public void Publish(int backIndex)
        {
            // swap front buffer
            int oldFront = Interlocked.Exchange(ref _frontIndex, backIndex);

            // mark new front as being read
            Volatile.Write(ref _states[backIndex], 2);

            // defer releasing old front until render is done
            _previousFront = oldFront;
        }

        // =============================
        // RENDER THREAD
        // =============================

        public T GetFrontBuffer()
        {
            int index = Volatile.Read(ref _frontIndex);
            return _buffers[index];
        }

        public void ReleasePreviousFront()
        {
            int prev = _previousFront;

            if (prev != -1)
            {
                // mark it as free again
                Volatile.Write(ref _states[prev], 0);
                _previousFront = -1;
            }
        }
    }
}