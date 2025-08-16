using DevoidEngine.Engine.Rendering;
using DevoidGPU;
using System;
using System.Numerics;

namespace DevoidEngine.Engine.Core
{
    public class Material
    {

        public BlendMode BlendMode { get; set; } = BlendMode.Opaque;
        public DepthTest DepthTest { get; set; } = DepthTest.LessEqual;
        public bool DepthWrite { get; set; } = true;
        public CullMode CullMode { get; set; } = CullMode.Back;
        public IUniformBuffer Buffer { get; set; }

        public Shader Shader { get; set; } = ShaderLibrary.GetShader("BASIC_SHADER");
    
        public virtual void Apply()
        {

        }

    }

    public class PBRMaterial : Material
    {
        public struct PBRData
        {
            public Vector4 Albedo;
            public Vector4 Emission;
            public float EmissionStr;
            public float Metallic;
            public float Roughness;

            public int UseDiffuseMap;
            public int UseNormalMap;
            public int UseRoughnessMap;
            public int UseEmissionMap;
            public int UseParallaxMap;
        }

        public PBRData MaterialData;

        // --- Backing fields
        private Vector4 _albedo = Vector4.Zero;
        private Vector4 _emission = Vector4.Zero;
        private float _emissionStr = 0f;
        private float _metallic = 0f;
        private float _roughness = 0f;

        // --- Properties only update CPU-side fields
        public Vector4 Albedo { get => _albedo; set { _albedo = value; Update(); } }
        public Vector4 Emission { get => _emission; set { _emission = value; Update(); } }
        public float EmissionStr { get => _emissionStr; set { _emissionStr = value; Update(); } }
        public float Metallic { get => _metallic; set { _metallic = value; Update(); } }
        public float Roughness { get => _roughness; set { _roughness = value; Update(); } }

        public PBRMaterial()
        {
            this.Buffer = Renderer.graphicsDevice.BufferFactory.CreateUniformBuffer<PBRData>();
            Update(); // keep data in sync at start
        }

        /// <summary>
        /// Updates the MaterialData struct from properties,
        /// and pushes it to the GPU buffer if one exists.
        /// </summary>
        private void Update()
        {
            MaterialData = new PBRData
            {
                Albedo = _albedo,
                Emission = _emission,
                EmissionStr = _emissionStr,
                Metallic = _metallic,
                Roughness = _roughness,
                // leave texture flags alone, set elsewhere
            };

            this.Buffer.SetData(ref  MaterialData);
        }

        public override void Apply()
        {
            base.Apply();
            this.Buffer.Bind(1, ShaderStage.Fragment);
            //Shader.Use();
        }
    }
}
