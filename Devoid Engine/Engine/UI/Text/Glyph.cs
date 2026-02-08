using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Text
{
    struct Glyph
    {
        public uint Codepoint;

        public Vector2 Size;
        public Vector2 Bearing;
        public float Advance;

        public Rect UV;
    }

}
