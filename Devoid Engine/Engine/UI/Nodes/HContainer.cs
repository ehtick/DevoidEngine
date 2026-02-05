using DevoidEngine.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    class HContainer : UINode
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

                width += child.MinSize.X;
                height = MathF.Max(height, child.MinSize.Y);
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

                if (child.Layout.ExpandH)
                {
                    expandCount++;
                }
                else
                {
                    fixedSize += child.MinSize.X;
                }

                participatingCount++;
            }

            // available horizontal space after gaps
            float availableSize = Math.Max(
                0,
                finalRect.size.X - Math.Max(0, childGap * (participatingCount - 1))
            );

            float expandedSize = expandCount > 0
                ? Math.Max(0, availableSize - fixedSize) / expandCount
                : 0;

            float cursorX = 0;

            foreach (var child in _children)
            {
                if (!child.ParticipatesInLayout)
                    continue;

                float childWidth = child.MinSize.X;

                if (child.Layout.ExpandH)
                    childWidth = expandedSize;

                child.Arrange(new UITransform(
                    new Vector2(finalRect.position.X + cursorX, finalRect.position.Y),
                    new Vector2(childWidth, finalRect.size.Y)
                ));

                cursorX += childWidth + childGap;
            }

            UIRenderer.DrawRect(finalRect, DEBUG_NUM_LOCAL);
        }


        protected override void RenderCore()
        {
            throw new NotImplementedException();
        }
    }
}
