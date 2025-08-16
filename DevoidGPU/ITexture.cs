using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public enum TextureType
    {
        Texture2D,
        Texture3D,
        TextureCube,
    }

    public interface ITexture
    {
        TextureType Type { get; }
        int Width { get; }
        int Height { get; }
        int Depth { get; }

        public bool IsRenderTarget { get; }
        public bool IsDepthStencil { get; }
    }

    public interface ITexture2D : ITexture
    {
        void SetData(byte[] data);
    }
    public interface ITexture3D : ITexture { }
    public interface ITextureCube : ITexture { }
}
