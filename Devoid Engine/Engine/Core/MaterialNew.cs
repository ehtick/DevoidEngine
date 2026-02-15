using DevoidGPU;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DevoidEngine.Engine.Core
{
    public class MaterialNew
    {
        public Shader Shader { get; }
        public int MaterialBufferSize => materialBufferSize;
        public int MaterialBufferBindSlot => materialBufferBindSlot;


        private readonly Dictionary<string, ShaderVariableInfo> variables;
        private readonly Dictionary<string, TextureBindingInfo> textureBindings;

        private readonly Dictionary<string, Texture> textures;

        private byte[] defaultBuffer;

        private int materialBufferBindSlot;
        private int materialBufferSize;

        public MaterialNew(Shader shader)
        {
            Shader = shader ?? throw new ArgumentNullException(nameof(shader));

            variables = new Dictionary<string, ShaderVariableInfo>();
            textureBindings = new Dictionary<string, TextureBindingInfo>();
            textures = new Dictionary<string, Texture>();

            var reflection = shader.fShader.ReflectionData;

            foreach (var cb in reflection.UniformBuffers)
            {
                if (!string.Equals(cb.Name, "Material", StringComparison.OrdinalIgnoreCase))
                    continue;

                materialBufferSize = cb.Size;
                materialBufferBindSlot = cb.BindSlot;

                defaultBuffer = new byte[cb.Size];

                foreach (var variable in cb.Variables)
                {
                    variables[variable.Name] = variable;
                }

                break;
            }

            foreach (var tb in reflection.TextureBindings)
            {
                if (!tb.Name.StartsWith("MAT_", StringComparison.Ordinal))
                    continue;

                if (tb.ResourceType != ShaderResourceType.Texture2D)
                    continue;

                textureBindings[tb.Name] = tb;

                textures[tb.Name] = Texture2D.WhiteTexture;
            }
        }

        public bool TryGetVariable(string name, out ShaderVariableInfo info)
            => variables.TryGetValue(name, out info);

        public bool HasTextureBinding(string name)
            => textureBindings.ContainsKey(name);

        public TextureBindingInfo GetTextureBinding(string name)
            => textureBindings[name];

        public Texture GetDefaultTexture(string name)
            => textures[name];

        public ReadOnlySpan<byte> GetDefaultMaterialBuffer()
            => defaultBuffer;

        public IEnumerable<string> GetTextureNames()
            => textureBindings.Keys;

        public void SetTexture(string name, Texture texture)
        {
            if (!textureBindings.ContainsKey(name))
                throw new Exception($"Texture '{name}' not found in material layout.");

            textures[name] = texture ?? Texture2D.WhiteTexture;
        }

        #region SETTERS
        public void SetFloat(string name, float value)
        {
            Write(name, value);
        }

        public void SetVector2(string name, Vector2 value)
        {
            Write(name, value);
        }

        public void SetVector3(string name, Vector3 value)
        {
            Write(name, value);
        }

        public void SetVector4(string name, Vector4 value)
        {
            Write(name, value);
        }

        public void SetMatrix4x4(string name, Matrix4x4 value)
        {
            Write(name, value);
        }
        #endregion

        private void Write<T>(string name, T value) where T : struct
        {
            if (!variables.TryGetValue(name, out var varInfo))
            {
                Console.WriteLine($"Variable '{name}' not found in material.");
                return;
            }

            var span = defaultBuffer.AsSpan(varInfo.Offset);

            MemoryMarshal.Write(span, ref value);
        }

    }

    public class MaterialInstanceNew
    {
        public MaterialNew BaseMaterial { get; }

        private Dictionary<string, Texture> textureOverrides;

        private byte[] cpuBuffer;
        private UniformBuffer gpuBuffer;
        private bool isDirty;

        public MaterialInstanceNew(MaterialNew material)
        {
            BaseMaterial = material;

            cpuBuffer = new byte[BaseMaterial.MaterialBufferSize];
            BaseMaterial.GetDefaultMaterialBuffer().CopyTo(cpuBuffer);

            gpuBuffer = new UniformBuffer(BaseMaterial.MaterialBufferSize, BufferUsage.Dynamic);

            textureOverrides = new Dictionary<string, Texture>();
            isDirty = true;

        }

        private void UpdateBuffer()
        {
            if (!isDirty) return;

            gpuBuffer.SetData(cpuBuffer);

            isDirty = false;
        }

        public void Bind()
        {
            UpdateBuffer();

            gpuBuffer.Bind(BaseMaterial.MaterialBufferBindSlot);
            BaseMaterial.Shader.Use();

            foreach (var texName in BaseMaterial.GetTextureNames())
            {
                var binding = BaseMaterial.GetTextureBinding(texName);

                Texture texture =
                    textureOverrides.TryGetValue(texName, out var overrideTex)
                        ? overrideTex
                        : BaseMaterial.GetDefaultTexture(texName);

                texture.Bind(binding.BindSlot);
            }
        }

        public void SetFloat(string name, float value)
            => Write(name, value);

        public void SetVector2(string name, Vector2 value)
            => Write(name, value);

        public void SetVector3(string name, Vector3 value)
            => Write(name, value);

        public void SetVector4(string name, Vector4 value)
            => Write(name, value);

        public void SetMatrix4x4(string name, Matrix4x4 value)
            => Write(name, value);

        private void Write<T>(string name, T value) where T : struct
        {
            if (!BaseMaterial.TryGetVariable(name, out var varInfo))
            {
                Console.WriteLine($"Variable '{name}' not found in material.");
                return;
            }

            var span = cpuBuffer.AsSpan(varInfo.Offset);
            MemoryMarshal.Write(span, ref value);

            isDirty = true;
        }

        public void SetTexture(string name, Texture texture)
        {
            if (!BaseMaterial.HasTextureBinding(name))
                throw new Exception($"Texture '{name}' not found in material layout.");

            textureOverrides[name] = texture ?? Texture2D.WhiteTexture;
        }

        public void ClearTextureOverride(string name)
        {
            textureOverrides.Remove(name);
        }
    }
}
