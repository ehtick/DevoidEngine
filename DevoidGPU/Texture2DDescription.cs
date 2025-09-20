using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public struct Tex2DDescription
    {
        public int Width;
        public int Height;
        public TextureFormat Format;

        public int MipLevels;
        public bool GenerateMipmaps;
        public bool IsRenderTarget;
        public bool IsDepthStencil;
        public bool IsMutable;


    }
}
