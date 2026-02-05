using DevoidEngine.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    class VContainer : UINode
    {
        public float childGap = 0f;

        protected override Vector2 MeasureCore(Vector2 availableSize)
        {
            float height = 0f;
            float width = 0f;
            int count = 0;

            foreach (var child in _children)
            {
                if (!child.ParticipatesInLayout)
                    continue;

                // Measure only for width / internal needs
                child.Measure(availableSize);

                height += child.MinSize.Y;
                width = MathF.Max(width, child.MinSize.X);
                count++;
            }

            if (count > 1)
                height += childGap * (count - 1);

            return new Vector2(width, height);
        }

        protected override void ArrangeCore(UITransform finalRect)
        {
            Rect = finalRect;

            float fixedSize = 0;
            float expandCount = 0;
            float participatingCount = 0;

            foreach (var child in _children)
            {
                if (!child.ParticipatesInLayout)
                    continue;
                if (child.Layout.ExpandV)
                {
                    // expansion
                    expandCount++;
                    
                } else
                {
                    // fixed sizing
                    fixedSize += child.MinSize.Y;
                }
                participatingCount++;
            }

            // this is size that is available after giving every child a gap.
            float availableSize = Math.Max(0, finalRect.size.Y - Math.Max(0, (childGap * (participatingCount - 1))));
            // now we compute the size of every expanded child, which means we subtract the fixed ones
            // and divide the remaining space
            float expandedSize = Math.Max(0, availableSize - fixedSize) / expandCount;

            // keep track of the y position
            float cursorY = 0;

            foreach (var child in _children)
            {
                if (!child.ParticipatesInLayout)
                    continue;

                float childHeight = child.MinSize.Y;

                if (child.Layout.ExpandV)
                    childHeight = expandedSize;

                child.Arrange(new UITransform(
                    new Vector2(finalRect.position.X, finalRect.position.Y + cursorY),
                    new Vector2(finalRect.size.X, childHeight)
                ));
                
                cursorY += childHeight + childGap;
            }

            UIRenderer.DrawRect(finalRect, DEBUG_NUM_LOCAL);
        }

        protected override void RenderCore()
        {
            throw new NotImplementedException();
        }
    }
}
