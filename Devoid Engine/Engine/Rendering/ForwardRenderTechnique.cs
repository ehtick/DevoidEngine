using DevoidEngine.Engine.Core;

namespace DevoidEngine.Engine.Rendering
{
    public class ForwardRenderTechnique : IRenderTechnique
    {
        Framebuffer finalOutputBuffer;

        public void Dispose()
        {

        }

        public void Initialize(int width, int height)
        {

            finalOutputBuffer = new Framebuffer();

            finalOutputBuffer.AttachRenderTexture(new Texture2D(new DevoidGPU.Tex2DDescription()
            {
                Width = width,
                Height = height,
                Format = DevoidGPU.TextureFormat.RGBA16_Float,
                GenerateMipmaps = false,
                IsDepthStencil = false,
                IsRenderTarget = true,
                IsMutable = false,
            }));
            finalOutputBuffer.AttachDepthTexture(new Texture2D(new DevoidGPU.Tex2DDescription()
            {
                Width = width,
                Height = height,
                Format = DevoidGPU.TextureFormat.Depth24_Stencil8,
                GenerateMipmaps = false,
                IsDepthStencil = true,
                IsRenderTarget = false,
                IsMutable = false
            }));

            UIRenderer.CreateCameraData(width, height);
        }

        public Texture2D Render(CameraRenderContext ctx)
        {
            finalOutputBuffer.Bind();
            finalOutputBuffer.Clear();

            RenderBase.SetupCamera(ctx.camera.GetCameraData());
            RenderBase.Execute(ctx.renderItems3D, RenderState.DefaultRenderState);

            //RenderBase.SetupCamera(UIRenderer.ScreenData);
            //RenderBase.Execute(ctx.renderItemsUI, RenderState.DefaultRenderState);


            return finalOutputBuffer.GetRenderTexture(0);
        }

        public void Resize(int width, int height)
        {
            finalOutputBuffer.Resize(width, height);
            UIRenderer.CreateCameraData(width, height);
        }
    }
}
