using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface ITextureFactory
    {
        ITexture2D CreateTexture2D(int width, int height, TextureFormat format, bool allowUAV = false, bool isDepth = false, bool isRenderTarget = false);
        ITexture3D CreateTexture3D();
        ITextureCube CreateTextureCube();


    }
}
