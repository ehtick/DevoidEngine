using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Utilities
{
    class AsyncTripleBuffer<T> where T : class
    {
        private T _front;   // render reads
        private T _back;    // update writes
        private T _spare;   // free buffer

        public T Front => Volatile.Read(ref _front);
        public T Back => _back;

        public AsyncTripleBuffer(T a, T b, T c)
        {
            _front = a;
            _back = b;
            _spare = c;
        }

        public void Publish()
        {
            // Ensure writes to back are visible
            Thread.MemoryBarrier();

            // Atomically swap front and back
            var oldFront = Interlocked.Exchange(ref _front, _back);

            // Rotate buffers
            _back = _spare;
            _spare = oldFront;
        }
    }
}
