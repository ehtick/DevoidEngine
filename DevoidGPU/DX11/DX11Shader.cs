using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU.DX11
{
    public class DX11Shader : IShader
    {
        public ShaderType Type { get; private set; }

        public string Name { get; private set; }

        public VertexShader VertexShader { get; private set; }
        public PixelShader PixelShader { get; private set; }
        public GeometryShader GeometryShader { get; private set; }
        public ComputeShader ComputeShader { get; private set; }

        public byte[] ByteCode { get; private set; }
        private readonly Device device;

        public DX11Shader(Device device, ShaderType type, string name = null)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            this.Type = type;
            this.Name = name ?? "UnnamedShader";
        }


        public void Compile(string source, string entryPoint)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Shader source cannot be empty.", nameof(source));

            if (string.IsNullOrWhiteSpace(entryPoint))
                throw new ArgumentException("Entry point cannot be empty.", nameof(entryPoint));

            string profile = GetProfileForType(Type);

            ShaderFlags flags = ShaderFlags.None;
#if DEBUG
            flags |= ShaderFlags.Debug | ShaderFlags.SkipOptimization;
#endif

            var result = ShaderBytecode.Compile(source, entryPoint, profile, flags);
            if (result.HasErrors)
                throw new Exception($"Shader compile error ({Name}): {result.Message}");

            ByteCode = result.Bytecode;

            CreateShaderFromBytecode();
        }

        public void LoadPrecompiled(byte[] bytecode)
        {
            using (var shaderBytecode = new SharpDX.D3DCompiler.ShaderBytecode(bytecode))
            {
                ByteCode = shaderBytecode;
                CreateShaderFromBytecode();
            }
        }

        private void CreateShaderFromBytecode()
        {
            switch (Type)
            {
                case ShaderType.Vertex:
                    VertexShader = new VertexShader(device, ByteCode);
                    break;
                case ShaderType.Fragment:
                    PixelShader = new PixelShader(device, ByteCode);
                    break;
                case ShaderType.Geometry:
                    GeometryShader = new GeometryShader(device, ByteCode);
                    break;
                case ShaderType.Compute:
                    ComputeShader = new ComputeShader(device, ByteCode);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported shader type: {Type}");
            }
        }

        private static string GetProfileForType(ShaderType type)
        {
            return type switch
            {
                ShaderType.Vertex => "vs_5_0",
                ShaderType.Fragment => "ps_5_0",
                ShaderType.Geometry => "gs_5_0",
                ShaderType.Compute => "cs_5_0",
                _ => throw new NotSupportedException($"No profile for shader type: {type}")
            };
        }

        public void Dispose()
        {
            VertexShader?.Dispose();
            PixelShader?.Dispose();
            GeometryShader?.Dispose();
            ComputeShader?.Dispose();
        }
    }
}
