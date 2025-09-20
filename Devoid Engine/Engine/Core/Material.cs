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

        public Guid Id;

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

        private Texture2D diffuseTexture;
        private Texture2D normalTexture;
        private Texture2D roughnessTexture;
        private Texture2D emissionTexture;

        public bool isDiffuseSet;
        public bool isNormalSet;
        public bool isSpecularSet;
        public bool isRoughnessSet;
        public bool isEmissionSet;

        // --- Properties only update CPU-side fields
        public Vector4 Albedo { get => _albedo; set { _albedo = value; Update(); } }
        public Vector4 Emission { get => _emission; set { _emission = value; Update(); } }
        public float EmissionStr { get => _emissionStr; set { _emissionStr = value; Update(); } }
        public float Metallic { get => _metallic; set { _metallic = value; Update(); } }
        public float Roughness { get => _roughness; set { _roughness = value; Update(); } }

        public Texture2D DiffuseTexture
        {
            get => diffuseTexture;
            set
            {
                diffuseTexture = value;
                isDiffuseSet = true;
                Update();
            }
        }

        public Texture2D NormalTexture
        {
            get => normalTexture;
            set
            {
                normalTexture = value;
                isNormalSet = true;
                Update();
            }
        }

        public Texture2D RoughnessTexture
        {
            get => roughnessTexture;
            set
            {
                roughnessTexture = value;
                isRoughnessSet = true;
                Update();
            }
        }

        public Texture2D EmissionTexture
        {
            get => emissionTexture;
            set
            {
                emissionTexture = value;
                isEmissionSet = true;
                Update();
            }
        }

        public PBRMaterial()
        {
            this.Shader = ShaderLibrary.GetShader("PBR/ClusteredPBR");
            this.Buffer = Renderer.graphicsDevice.BufferFactory.CreateUniformBuffer<PBRData>(BufferUsage.Dynamic);
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
                UseDiffuseMap = isDiffuseSet ? 1 : 0,
                UseNormalMap = isNormalSet ? 1 : 0,
                UseRoughnessMap = isRoughnessSet ? 1 : 0,
                UseEmissionMap = isEmissionSet ? 1 : 0,
                UseParallaxMap = 0
                // leave texture flags alone, set elsewhere
            };

            RenderThreadDispatcher.QueueLatest("UPDATE_MATERIAL_DATA_" + GetHashCode(), () =>
            {
                this.Buffer.SetData(ref MaterialData);
            });
        }

        bool x = false;

        public override void Apply()
        {
            base.Apply();

            this.Buffer.Bind(2, ShaderStage.Fragment);
            Shader.Use();

            if (isDiffuseSet)
            {
                diffuseTexture.BindSampler(0);
                diffuseTexture.Bind(10, ShaderStage.Fragment);

            }
            else
            {
                Texture2D.DefaultSampler.Bind(0);
                Texture2D.WhiteTexture.Bind(10);
            }

            if (isNormalSet)
            {
                normalTexture.BindSampler(1);
                normalTexture.Bind(11);
            }
            else
            {
                Texture2D.DefaultSampler.Bind(1);
                Texture2D.WhiteTexture.Bind(11);
            }

            if (isRoughnessSet)
            {
                roughnessTexture.BindSampler(2);
                roughnessTexture.Bind(12);
            }
            else
            {
                //Texture.White2DTex.Bind();
            }

            if (isEmissionSet)
            {
                emissionTexture.BindSampler(3);
                emissionTexture.Bind(13);
            }
            else
            {
                //Texture.White2DTex.Bind();
            }


        }
    }
}
