using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevoidGPU
{
    public struct PresentationParameters
    {
        public int BackBufferWidth;
        public int BackBufferHeight;
        public int RefreshRate;
        public bool VSync;
        public bool Windowed;
        public int BufferCount;
        public TextureFormat ColorFormat;
    }
}
