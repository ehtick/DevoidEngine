using SharpDX;
using SharpDX.Direct3D;
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
    internal class DX11Texture2D : ITexture2D, IDisposable
    {
        private IntPtr handle;

        public TextureType Type => TextureType.Texture2D;
        public int Width { get; set; }

        public int Height { get; set; }

        public bool IsRenderTarget { get; }

        public bool IsDepthStencil { get; }

        public Texture2D Texture { get; set; }
        public RenderTargetView RenderTargetView { get; set; }
        public DepthStencilView DepthStencilView { get; set; }
        public ShaderResourceView ShaderResourceView { get; private set; }
        public UnorderedAccessView UnorderedAccessView { get; private set; }

        public bool AllowUnorderedView { get; set; }

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

            var textureFormat = format;
            if (IsDepthStencil && format == Format.D24_UNorm_S8_UInt)
                textureFormat = Format.R24G8_Typeless;

            var desc = new Texture2DDescription
            {
                
                Width = width,
                Height = height,
                MipLevels = 0,
                ArraySize = 1,
                Format = textureFormat,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = (IsDepthStencil ? BindFlags.DepthStencil | BindFlags.ShaderResource : BindFlags.ShaderResource)
                   | (IsRenderTarget ? BindFlags.RenderTarget : 0)
                   | (AllowUnorderedView ? BindFlags.UnorderedAccess : 0),
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            this.Texture = new Texture2D(device, desc);

            RenderTargetView = IsRenderTarget ? new RenderTargetView(device, Texture) : null;
            if (IsDepthStencil)
            {
                var dsvDesc = new DepthStencilViewDescription
                {
                    Format = Format.D24_UNorm_S8_UInt,
                    Dimension = DepthStencilViewDimension.Texture2D,
                    Flags = DepthStencilViewFlags.None,
                    Texture2D = new DepthStencilViewDescription.Texture2DResource { MipSlice = 0 }
                };
                DepthStencilView = new DepthStencilView(device, Texture, dsvDesc);

                var srvDesc = new ShaderResourceViewDescription
                {
                    Format = Format.R24_UNorm_X8_Typeless,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource
                    {
                        MostDetailedMip = 0,
                        MipLevels = 1
                    }
                };
                ShaderResourceView = new ShaderResourceView(device, Texture, srvDesc);
            }
            else
            {
                ShaderResourceView = new ShaderResourceView(device, Texture);
            }

            if (AllowUnorderedView)
            {
                UnorderedAccessView = new UnorderedAccessView(device, this.Texture);
            }

            handle = TextureManager.Register(this);
        }

        public void SetData(byte[] data)
        {
            int rowPitch = DX11TextureFormat.RowPitch(DX11TextureFormat.DXGIFormatToTextureFormat(format), Width); // Format is stored in DX11Texture2D
            var handle = System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            var box = new DataBox(handle, rowPitch, 0);
            deviceContext.UpdateSubresource(box, Texture, 0);
        }

        public void GenerateMipmaps()
        {
            deviceContext.GenerateMips(ShaderResourceView);
        }

        public IntPtr GetHandle() => handle;

        public void Dispose()
        {
            DepthStencilView?.Dispose();
            RenderTargetView?.Dispose();
            ShaderResourceView?.Dispose();
            Texture?.Dispose();

            DepthStencilView = null;
            RenderTargetView = null;
            ShaderResourceView = null;
            Texture = null;
            handle = IntPtr.Zero;
            TextureManager.Unregister(handle);
        }

        public void Bind(int slot)
        {
            deviceContext.PixelShader.SetShaderResource(slot, ShaderResourceView);
        }

        public void BindMutable(int slot)
        {
            deviceContext.ComputeShader.SetUnorderedAccessView(slot, UnorderedAccessView);
        }

        
    }
}
