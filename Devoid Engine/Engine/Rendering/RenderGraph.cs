using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering.GPUResource;
using DevoidGPU;

namespace DevoidEngine.Engine.Rendering
{
    public class RenderGraph
    {
        Dictionary<int, RenderGraphResource> resources = new();
        Dictionary<string, RGResource> nameLookup = new();

        List<RenderGraphPass> passes = new();

        List<RenderGraphPass> compiledPasses = new();

        int resourceCounter = 0;

        bool compiled = false;

        public RGResource ImportTexture(string name, Texture2D texture)
        {
            var id = resourceCounter++;

            var res = new RenderGraphResource()
            {
                Name = name,
                Texture = texture,
                Imported = true
            };

            resources[id] = res;

            var handle = new RGResource(id);
            nameLookup[name] = handle;

            return handle;
        }

        public void UpdateImported(RGResource handle, Texture2D texture)
        {
            resources[handle.Id].Texture = texture;
        }

        public RGResource CreateResource(string name, TextureDescription desc, RenderGraphPass producer)
        {
            var id = resourceCounter++;

            var res = new RenderGraphResource()
            {
                Name = name,
                Description = desc,
                Imported = false,
                Producer = producer
            };

            resources[id] = res;

            var handle = new RGResource(id);
            nameLookup[name] = handle;

            return handle;
        }

        public RGResource GetResource(string name)
        {
            return nameLookup[name];
        }

        public Texture2D GetTexture(RGResource handle)
        {
            return resources[handle.Id].Texture;
        }

        public void RegisterProducer(RGResource res, RenderGraphPass pass)
        {
            resources[res.Id].Producer = pass;
        }

        public void AddPass(RenderGraphPass pass)
        {
            var builder = new RenderGraphBuilder(this, pass);

            pass.Setup(builder);

            passes.Add(pass);

            compiled = false;
        }

        public void Clear()
        {
            passes.Clear();
            compiledPasses.Clear();
            compiled = false;
        }

        public void Compile()
        {
            compiledPasses = ResolvePassOrder();
            compiled = true;
        }

        public void Execute()
        {
            if (!compiled)
                throw new Exception("RenderGraph must be compiled before execution.");

            AllocateResources();

            var ctx = new RenderGraphContext(resources);

            foreach (var pass in compiledPasses)
                pass.Execute(ctx);
        }

        void AllocateResources()
        {
            foreach (var res in resources.Values)
            {
                if (res.Imported)
                    continue;

                if (!res.Allocated)
                {
                    res.Texture = new Texture2D(res.Description);
                    res.Allocated = true;
                }
            }
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
                    var producer = resources[read.Id].Producer;

                    if (producer == null || producer == pass)
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
