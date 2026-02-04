using DevoidEngine.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    class PanelNode : UINode
    {
        public override Vector2 Measure(Vector2 availableSize)
        {
            Vector2 desired = Size ?? Vector2.Zero;

            foreach (var child in _children)
            {
                Vector2 childSize = child.Measure(availableSize);
                desired.X = MathF.Max(desired.X, childSize.X);
                desired.Y = MathF.Max(desired.Y, childSize.Y);
            }

            return desired;
        }

        public override void Arrange(UITransform finalRect)
        {
            foreach (var child in _children)
            {
                child.Arrange(finalRect);
            }

            UIRenderer.DrawRect(new UITransform()
            {
                position = new Vector2(0, 0),
                size = Size ?? Vector2.Zero,
            });
        }

    }
}
