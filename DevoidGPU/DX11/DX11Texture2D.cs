using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Device = SharpDX.Direct3D11.Device;

namespace DevoidGPU.DX11
{
    internal class DX11Texture2D : ITexture2D
    {
        public TextureType Type => TextureType.Texture2D;
        public int Width { get; set; }

        public int Height { get; set; }

        public int Depth { get; set; }

        public bool IsRenderTarget { get; }

        public bool IsDepthStencil { get; }

        public Texture2D Texture { get; set; }
        public RenderTargetView RenderTargetView { get; set; }
        public DepthStencilView DepthStencilView { get; set; }

        private readonly Device device;
        private readonly DeviceContext deviceContext;
        private Format format;

        public DX11Texture2D(Device device, DeviceContext deviceContext, bool isRT = true, bool isDS = false)
        {
            this.device = device;
            this.deviceContext = deviceContext;

            IsRenderTarget = isRT;
            IsDepthStencil = isDS;
        }

        public void Create(int width, int height, Format format)
        {
            this.format = format;
            this.Width = width;
            this.Height = height;
            var desc = new Texture2DDescription
            {
                
                Width = width,
                Height = height,
                MipLevels = 1,
                ArraySize = 1,
                Format = format,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource
                          | (IsRenderTarget ? BindFlags.RenderTarget : 0)
                          | (IsDepthStencil ? BindFlags.DepthStencil : 0),
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            this.Texture = new Texture2D(device, desc);

            RenderTargetView = IsRenderTarget ? new RenderTargetView(device, Texture) : null;
            DepthStencilView = IsDepthStencil ? new DepthStencilView(device, Texture) : null;
        }

        public void SetData(byte[] data)
        {
            int rowPitch = DX11TextureFormat.RowPitch(DX11TextureFormat.DXGIFormatToTextureFormat(format), Width); // Format is stored in DX11Texture2D
            var handle = System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            var box = new DataBox(handle, rowPitch, 0);
            deviceContext.UpdateSubresource(box, Texture, 0);
        }
    }
}
