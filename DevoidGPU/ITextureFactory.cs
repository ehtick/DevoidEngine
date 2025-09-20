using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public interface ITextureFactory
    {
        ITexture2D CreateTexture2D(Tex2DDescription description);
        ITexture3D CreateTexture3D();
        ITextureCube CreateTextureCube();


    }
}
