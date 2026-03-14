using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering.GPUResource;
using DevoidGPU;

namespace DevoidEngine.Engine.Rendering
{
    public class RenderGraph
    {
        List<RenderGraphPass> passes = new();
        List<RenderGraphPass> compiledPasses = new();

        Dictionary<string, RenderGraphPass> producers = new();

        bool dirty = true;

        public void AddPass(RenderGraphPass pass)
        {
            pass.Setup();
            passes.Add(pass);
            dirty = true;
        }

        public void RemovePass(RenderGraphPass pass)
        {
            passes.Remove(pass);
            dirty = true;
        }

        public void Clear()
        {
            passes.Clear();
            compiledPasses.Clear();
            producers.Clear();
            dirty = true;
        }

        public void Compile()
        {
            producers.Clear();

            foreach (var pass in passes)
            {
                foreach (var write in pass.Writes)
                {
                    producers[write] = pass;
                }
            }

            compiledPasses = ResolvePassOrder();
            dirty = false;
        }

        public void Execute()
        {
            if (dirty)
                Compile();

            var ctx = new RenderGraphContext();

            foreach (var pass in compiledPasses)
                pass.Execute(ctx);
        }

        List<RenderGraphPass> ResolvePassOrder()
        {
            Dictionary<RenderGraphPass, List<RenderGraphPass>> edges = new();
            Dictionary<RenderGraphPass, int> incoming = new();

            foreach (var pass in passes)
            {
                edges[pass] = new();
                incoming[pass] = 0;
            }

            foreach (var pass in passes)
            {
                foreach (var read in pass.Reads)
                {
                    if (!producers.TryGetValue(read, out var producer))
                        continue;

                    if (producer == pass)
                        continue;

                    edges[producer].Add(pass);
                    incoming[pass]++;
                }
            }

            Queue<RenderGraphPass> ready = new();

            foreach (var pass in passes)
                if (incoming[pass] == 0)
                    ready.Enqueue(pass);

            List<RenderGraphPass> result = new();

            while (ready.Count > 0)
            {
                var p = ready.Dequeue();
                result.Add(p);

                foreach (var dep in edges[p])
                {
                    incoming[dep]--;

                    if (incoming[dep] == 0)
                        ready.Enqueue(dep);
                }
            }

            if (result.Count != passes.Count)
                throw new Exception("RenderGraph contains a cycle.");

            return result;
        }
    }
}