using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU.DX11
{
    public class DX11GraphicsDevice : IGraphicsDevice
    {

        public IPresentSurface MainSurface { get; private set; }

        public IBufferFactory BufferFactory { get; private set; }
        public IShaderFactory ShaderFactory { get; private set; }
        public ITextureFactory TextureFactory { get; private set; }

        private SharpDX.Direct3D11.Device device;
        private DeviceContext deviceContext;
        private SwapChain swapChain;

        public void Initialize(nint hwnd, PresentationParameters parameters)
        {
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 2,
                ModeDescription = new ModeDescription()
                {
                    Width = parameters.BackBufferWidth,
                    Height = parameters.BackBufferHeight,
                    RefreshRate = new Rational(parameters.RefreshRate, 1),
                    Format = DX11TextureFormat.ToDXGIFormat(parameters.ColorFormat)
                },
                IsWindowed = parameters.Windowed,
                Usage = Usage.RenderTargetOutput,
                SwapEffect = SwapEffect.Discard,

                SampleDescription = new SampleDescription(1, 0),

                OutputHandle = hwnd
            };

            SharpDX.Direct3D11.Device.CreateWithSwapChain(
                SharpDX.Direct3D.DriverType.Hardware,
                DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug,
                swapChainDesc,
                out device,
                out swapChain
            );


            deviceContext = device.ImmediateContext;

            using (var backBuffer = swapChain.GetBackBuffer<Texture2D>(0))
            {
                var rtv = new RenderTargetView(device, backBuffer);
                MainSurface = new DX11PresentSurface(device, deviceContext, swapChain, rtv);
            }

            BufferFactory = new DX11BufferFactory(device, deviceContext);
            ShaderFactory = new DX11ShaderFactory(device, deviceContext);
            TextureFactory = new DX11TextureFactory(device, deviceContext);
        }

        public void DrawInstanced(int indexCount, int startIndexLocation, int baseVertexLocation)
        {
            deviceContext.DrawIndexed(baseVertexLocation, indexCount, startIndexLocation);
        }

        public void Draw(int vertexCount, int startVertex)
        {
            deviceContext.Draw(vertexCount, startVertex);
        }

        public void SetViewport(int x, int y, int width, int height)
        {
            deviceContext.Rasterizer.SetViewport(x, y, width, height);
        }

        public IInputLayout CreateInputLayout(VertexInfo vertexInfo, IShader vertexShader)
        {
            if (vertexShader is not DX11Shader dxShader)
                throw new ArgumentException("Vertex shader must be DX11Shader", nameof(vertexShader));

            return new DX11InputLayout(device, deviceContext, vertexInfo, dxShader);
        }


        public void SetBlendState()
        {
            throw new NotImplementedException();
        }

        public void SetScissorState()
        {
            throw new NotImplementedException();
        }

        public void SetAlphaBlendState()
        {
            throw new NotImplementedException();
        }

        public void SetRasterizerState()
        {
            throw new NotImplementedException();
        }
    }
}
