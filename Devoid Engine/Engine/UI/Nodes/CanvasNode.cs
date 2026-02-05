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
            // Canvas always fills screen
            return availableSize;
        }

        protected override void ArrangeCore(UITransform finalRect)
        {
            // Let FlexNode do ALL layout logic
            base.ArrangeCore(finalRect);
        }

        protected override void RenderCore()
        {
            // no-op
        }
    }
}
