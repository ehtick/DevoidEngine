using DevoidGPU;

namespace DevoidEngine.Engine.Rendering
{
    public static class InputLayoutManager
    {
        public static Dictionary<(VertexInfo, IShader), IInputLayout> inputLayoutCache = new Dictionary<(VertexInfo, IShader), IInputLayout>();
    }
}
