using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    public class CanvasNode : UINode
    {
        protected override Vector2 MeasureCore(Vector2 availableSize)
        {
            foreach (var child in _children)
            {
                child.Measure(availableSize);
            }

            // Canvas desires exactly the available screen size
            return availableSize;
        }
        protected override void ArrangeCore(UITransform finalRect)
        {
            Rect = finalRect;

            foreach (var child in _children)
            {

                child.Arrange(new UITransform(
                    finalRect.position + child.Offset,
                    child.Size ?? child.DesiredSize
                ));
            }

        }
    }
}
