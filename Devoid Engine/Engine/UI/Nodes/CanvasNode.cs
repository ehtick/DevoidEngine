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
        public override Vector2 Measure(Vector2 availableSize)
        {
            foreach (var child in _children)
                child.Measure(availableSize);

            return availableSize;
        }

        public override void Arrange(UITransform finalRect)
        {
            foreach (var child in _children)
            {
                if (child.ParticipatesInLayout)
                {
                    // Normal layout flow
                    child.Arrange(new UITransform(
                        finalRect.position,
                        child.DesiredSize
                    ));
                }
                else
                {
                    // SMART DEFAULT: absolute positioning
                    child.Arrange(new UITransform(
                        finalRect.position + child.Position,
                        child.Size ?? child.DesiredSize
                    ));
                }
            }
        }

    }
}
