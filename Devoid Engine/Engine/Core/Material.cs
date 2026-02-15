using DevoidEngine.Engine.Rendering;
using DevoidGPU;
using System.Numerics;

namespace DevoidEngine.Engine.Core
{

    public class MaterialGPUData
    {
        public IUniformBuffer UniformBuffer;
        public bool Dirty;
    }

    public class Material
    {
        public RenderState RenderState;
        public Guid Id;

        public Shader Shader { get; set; } = ShaderLibrary.GetShader("BASIC_SHADER");
        public MaterialLayout MaterialLayout { get; set; }

        // PROPERTIES
        public Dictionary<string, int> PropertiesInt = new();
        public Dictionary<string, float> PropertiesFloat = new();
        public Dictionary<string, Vector2> PropertiesVec2 = new();
        public Dictionary<string, Vector3> PropertiesVec3 = new();
        public Dictionary<string, Vector4> PropertiesVec4 = new();
        public Dictionary<string, Matrix4x4> PropertiesMat4 = new();


        public Dictionary<string, Texture2D> Textures2D = new();

    }

    public class MaterialInstance
    {
        public Material BaseMaterial;
        public MaterialGPUData GPUData { get; set; }

        public Dictionary<string, int> PropertiesIntOverride = new();
        public Dictionary<string, float> PropertiesFloatOverride = new();
        public Dictionary<string, Vector2> PropertiesVec2Override = new();
        public Dictionary<string, Vector3> PropertiesVec3Override = new();
        public Dictionary<string, Vector4> PropertiesVec4Override = new();
        public Dictionary<string, Matrix4x4> PropertiesMat4Override = new();

        public Dictionary<string, Texture2D> Textures2DOverride = new();

        public MaterialInstance(Material material)
        {
            this.BaseMaterial = material;

            this.GPUData = new MaterialGPUData();
            this.GPUData.UniformBuffer = Renderer.graphicsDevice.BufferFactory.CreateUniformBuffer(material.MaterialLayout.bufferSize, BufferUsage.Dynamic);
            this.GPUData.Dirty = true;

            MaterialHelper.Update(this);

        }

        public void Set(string name, int value)
        {
            PropertiesIntOverride[name] = value;
            GPUData.Dirty = true;
        }
        public void Set(string name, float value)
        {
            PropertiesFloatOverride[name] = value;
            GPUData.Dirty = true;
        }
        public void Set(string name, Vector2 value)
        {
            PropertiesVec2Override[name] = value;
            GPUData.Dirty = true;
        }
        public void Set(string name, Vector3 value)
        {
            PropertiesVec3Override[name] = value;
            GPUData.Dirty = true;
        }
        public void Set(string name, Vector4 value)
        {
            PropertiesVec4Override[name] = value;
            GPUData.Dirty = true;
        }

        public void Set(string name, Matrix4x4 value)
        {
            PropertiesMat4Override[name] = value;
            GPUData.Dirty = true;
        }

        public void Apply()
        {
            MaterialHelper.Update(this);

            GPUData.UniformBuffer.Bind(2, ShaderStage.Fragment | ShaderStage.Vertex);
        }
    }
}
