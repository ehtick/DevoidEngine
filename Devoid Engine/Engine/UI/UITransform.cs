using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI
{
    public class UITransform
    {
        public Vector2 position;
        public Vector2 size;
        public Vector2 pivot; // (0,0) top left, (0.5,0.5) center
        public Vector2 anchormin;
        public Vector2 anchormax;

        public UITransform() { }
        public UITransform(Vector2 start, Vector2 end)
        {
            this.position = start;
            this.size = end;
        }
    }
}
