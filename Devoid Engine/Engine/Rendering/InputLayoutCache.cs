using DevoidGPU;

namespace DevoidEngine.Engine.Rendering
{
    public class InputLayoutCache
    {
        private readonly Dictionary<(VertexInfo, IShader), IInputLayout> cache = new();

        public IInputLayout GetOrCreateLayout(VertexInfo vertexInfo, IShader program, IGraphicsDevice device)
        {
            var key = (vertexInfo, program);

            if (!cache.TryGetValue(key, out var layout))
            {
                layout = device.CreateInputLayout(vertexInfo, program);
                cache[key] = layout;
            }

            return layout;
        }


    }

}
