using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    public class CanvasNode : FlexNode
    {
        protected override Vector2 MeasureCore(Vector2 availableSize)
        {
            base.MeasureCore(availableSize);
            return availableSize;
        }

        protected override void ArrangeCore(UITransform finalRect)
        {
            base.ArrangeCore(finalRect);
        }

        protected override void RenderCore()
        {

        }
    }
}
