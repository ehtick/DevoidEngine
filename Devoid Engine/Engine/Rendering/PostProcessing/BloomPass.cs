using DevoidEngine.Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.Rendering.PostProcessing
{
    struct BloomMip
    {
        public Vector2 size;
        public Texture2D texture;
    }

    struct BloomMipShaderData
    {
        public Vector2 mipSize;
        public int mipLevel;
        public float filterRadius;
    }

    internal class BloomPass : RenderGraphPass
    {
        public int BloomMipCount { get; set; } = 8;
        public float BloomFilterRadius { get; set; } = 0f;

        Shader BloomUpsampleShader = new Shader("Engine/Content/Shaders/Screen/bloom_base.vert.hlsl", "Engine/Content/Shaders/Screen/bloom_upsample.frag.hlsl");
        Shader BloomDownsampleShader = new Shader("Engine/Content/Shaders/Screen/bloom_base.vert.hlsl", "Engine/Content/Shaders/Screen/bloom_downsample.frag.hlsl");

        Framebuffer bloomFrameBuffer;
        UniformBuffer mipShaderDataBuffer;

        Vector2 screenSize;
        List<BloomMip> bloomMipList;

        public BloomPass(int width, int height)
        {
            screenSize = new Vector2(width, height);
            bloomMipList = new List<BloomMip>();

            Vector2 currentMipSize = screenSize; 

            for (int i = 0; i < BloomMipCount; i++)
            {
                BloomMip bloomMip = new BloomMip();
                currentMipSize *= 0.5f;
                if (currentMipSize.X <= 1 || currentMipSize.Y <= 1)
                {
                    continue;
                }

                bloomMip.size = currentMipSize;

                Texture2D mipTexture = new Texture2D(new DevoidGPU.TextureDescription()
                {
                    Format = DevoidGPU.TextureFormat.RGBA16_Float,
                    Width = (int)currentMipSize.X,
                    Height = (int)currentMipSize.Y,
                    GenerateMipmaps = false,
                    IsRenderTarget = true,
                    MipLevels = 1,
                    Type = DevoidGPU.TextureType.Texture2D
                });

                mipTexture.SetFilter(DevoidGPU.TextureFilter.Linear, DevoidGPU.TextureFilter.Linear);
                mipTexture.SetWrapMode(DevoidGPU.TextureWrapMode.ClampToEdge, DevoidGPU.TextureWrapMode.ClampToEdge);

                bloomMip.texture = mipTexture;
                bloomMipList.Add(bloomMip);
            }

            bloomFrameBuffer = new Framebuffer();

            mipShaderDataBuffer = new UniformBuffer(Marshal.SizeOf<BloomMipShaderData>(), DevoidGPU.BufferUsage.Dynamic);

        }
        public override void Setup()
        {
            Read("SceneColor");
            Write("BloomOutput");
        }

        public override void Execute(RenderGraphContext ctx)
        {
            (int,int,int,int) ViewportSize = Renderer.graphicsDevice.GetViewport();

            mipShaderDataBuffer.Bind(2);
            bloomFrameBuffer.Bind();
            RenderDownsamples(ctx.GetTexture("SceneColor"));
            RenderUpsamples();

            Renderer.graphicsDevice.SetViewport(ViewportSize.Item1, ViewportSize.Item2, ViewportSize.Item3, ViewportSize.Item4);
        }

        public override void Resize(int width, int height)
        {
        }

        void RenderUpsamples()
        {
            BloomUpsampleShader.Use();

            Renderer.graphicsDevice.SetBlendState(DevoidGPU.BlendMode.Additive);

            for (int i = bloomMipList.Count - 1; i > 0; i--)
            {
                BloomMip sourceMip = bloomMipList[i];
                BloomMip destMip = bloomMipList[i - 1];
                bloomMipList.Last().texture.UnBind(0);

                Renderer.graphicsDevice.SetViewport(
                    0,
                    0,
                    (int)destMip.size.X,
                    (int)destMip.size.Y);

                Console.WriteLine("CalledSetRenderTex");
                bloomFrameBuffer.SetRenderTexture(destMip.texture, 0);
                Console.WriteLine("CalledSetRenderTexEnd");

                sourceMip.texture.Bind(0);

                BloomMipShaderData mipData = new BloomMipShaderData()
                {
                    mipLevel = i,
                    mipSize = sourceMip.size,
                    filterRadius = BloomFilterRadius
                };

                mipShaderDataBuffer.SetData(mipData);

                RenderAPI.RenderFullScreen(BloomUpsampleShader);
            }
            bloomMipList.Last().texture.UnBind(0);
        }

        void RenderDownsamples(Texture2D inputTexture)
        {
            BloomDownsampleShader.Use();

            for (int i = 0; i < bloomMipList.Count; i++)
            {
                BloomMip bloomMip = bloomMipList[i];

                Renderer.graphicsDevice.SetViewport(0, 0, (int)bloomMip.size.X, (int)bloomMip.size.Y);

                bloomFrameBuffer.SetRenderTexture(bloomMip.texture, 0);
                bloomFrameBuffer.Clear();

                Texture2D sourceTexture;
                Vector2 sourceSize;

                if (i == 0)
                {
                    sourceTexture = inputTexture;
                    sourceSize = screenSize;
                }
                else
                {
                    sourceTexture = bloomMipList[i - 1].texture;
                    sourceSize = bloomMipList[i - 1].size;
                }

                sourceTexture.Bind(0);

                BloomMipShaderData bloomMipData = new BloomMipShaderData()
                {
                    mipLevel = i,
                    mipSize = sourceSize,
                    filterRadius = BloomFilterRadius
                };

                mipShaderDataBuffer.SetData(bloomMipData);

                RenderAPI.RenderFullScreen(BloomDownsampleShader);
            }
        }
    }
}
