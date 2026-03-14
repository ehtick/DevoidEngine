using DevoidEngine.Engine.Core;
using DevoidEngine.Engine.Rendering.GPUResource;

namespace DevoidEngine.Engine.Rendering
{
    public class RenderGraph
    {
        Dictionary<int, RenderGraphResource> resources = new();
        Dictionary<string, RGResource> nameLookup = new();

        List<RenderGraphPass> passes = new();

        int resourceCounter = 0;

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

        public RGResource CreateResource(string name, RenderGraphPass producer)
        {
            var id = resourceCounter++;

            var res = new RenderGraphResource()
            {
                Name = name,
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

        public void RegisterProducer(RGResource res, RenderGraphPass pass)
        {
            resources[res.Id].Producer = pass;
        }

        public void AddPass(RenderGraphPass pass)
        {
            var builder = new RenderGraphBuilder(this, pass);

            pass.Setup(builder);

            passes.Add(pass);
        }
    }
}
