using DevoidEngine.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DevoidEngine.Engine.UI.Nodes
{
    public class ScrollNode : FlexboxNode
    {
        public Vector2 ScrollOffset = Vector2.Zero;

        private Vector2 contentSize;

        public float ScrollSpeed = 30f;

        protected override Vector2 MeasureCore(Vector2 available)
        {
            contentSize = base.MeasureCore(available);

            // viewport size is fixed
            return Size ?? available;
        }

        protected override void ArrangeCore(UITransform finalRect)
        {
            Rect = finalRect;

            Vector2 shiftedPos = finalRect.position - ScrollOffset;

            UITransform contentRect = new UITransform(
                shiftedPos,
                finalRect.size
            );

            base.ArrangeCore(contentRect);
        }

        public override void Render(List<RenderItem> renderList, Matrix4x4 canvas)
        {
            UIScissorStack.Push(
                Rect.position.X,
                Rect.position.Y,
                Rect.size.X,
                Rect.size.Y
            );

            base.Render(renderList, canvas);

            UIScissorStack.Pop();
        }

        public override void OnMouseWheel(float delta)
        {
            ScrollOffset.Y -= delta * ScrollSpeed;

            float maxScroll = Math.Max(0, contentSize.Y - Rect.size.Y);

            ScrollOffset.Y = Math.Clamp(ScrollOffset.Y, 0, maxScroll);

            Console.WriteLine(ScrollOffset);
        }

    }
}
