using DevoidEngine.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    class ContainerNode : UINode
    {
        public float Padding = 0.0f;


        protected override Vector2 MeasureCore(Vector2 availableSize)
        {
            Vector2 desired = Vector2.Zero;


            foreach (var child in _children)
            {
                Vector2 childSize = child.Measure(availableSize);

                // Parent cares about child's margin

                desired.X = MathF.Max(desired.X, childSize.X);
                desired.Y = MathF.Max(desired.Y, childSize.Y);
            }

            return desired;
        }
        protected override void ArrangeCore(UITransform finalRect)
        {
            Rect = finalRect;

            Vector2 contentPos = finalRect.position + new Vector2(Padding);
            Vector2 contentSize = finalRect.size - new Vector2(Padding * 2);

            contentSize.X = Math.Max(0, contentSize.X);
            contentSize.Y = Math.Max(0, contentSize.Y);

            float leftOffset = 0f;

            foreach (var child in _children)
            {

                Vector2 desired = child.Size ?? child.DesiredSize;

                Vector2 finalSize = new Vector2(
                    Math.Min(desired.X, contentSize.X),
                    Math.Min(desired.Y, contentSize.Y)
                );

                

                child.Arrange(new UITransform(
                    contentPos + child.Offset + new Vector2(leftOffset, 0),
                    finalSize
                ));
                leftOffset += finalSize.X;
            }

            UIRenderer.DrawRect(finalRect);
        }
    }
}
