namespace DevoidEngine.Engine.Utilities
{

    public class DoubleBuffer<T> where T : class
    {
        private T _front;   // buffer currently being read by render
        private T _spare;   // buffer most recently filled by update

        public T Front => Volatile.Read(ref _front); // always safe to read

        public DoubleBuffer(T initialFront, T initialSpare)
        {
            _front = initialFront;
            _spare = initialSpare;
        }

        /// <summary>
        /// Called by the update thread after filling a buffer.
        /// Atomically replaces the spare buffer with the newly filled one.
        /// Returns the previous spare buffer so the update thread can cycle it.
        /// </summary>
        public T UpdateSwap(T justFilledBuffer)
        {
            return Interlocked.Exchange(ref _spare, justFilledBuffer);
        }

        /// <summary>
        /// Called by the render thread when it finishes reading Front.
        /// Atomically swaps Front with the current Spare buffer.
        /// </summary>
        public void RenderSwap()
        {
            _front = Interlocked.Exchange(ref _spare, _front);
        }
    }







}
