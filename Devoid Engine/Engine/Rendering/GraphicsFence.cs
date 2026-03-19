namespace DevoidEngine.Engine.Rendering
{
    public sealed class GraphicsFence
    {
        private readonly ManualResetEventSlim _event = new(false);

        internal void Signal()
        {
            _event.Set();
        }

        public void Wait()
        {
            _event.Wait();
        }
    }
}
