namespace DevoidEngine.Engine.Core
{
    public enum ShaderPropertyType
    {
        Int,
        Float,
        Vector2,
        Vector3,
        Vector4,
        Matrix4
    }

    public sealed class ShaderPropertyInfo
    {
        public string Name;
        public ShaderPropertyType Type;
        public int Offset;
    }


    public class MaterialLayout
    {
        public int bufferSize = 0;
        public List<ShaderPropertyInfo> Properties = new List<ShaderPropertyInfo>();
    }
}
